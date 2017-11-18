﻿﻿/**
 * Copyright 2017 Márton Gergó
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TriangulatedPolygonAStar.BasicGeometry;
using TriangulatedPolygonAStar.UI.Resources;

namespace TriangulatedPolygonAStar.UI
{
    /// <summary>
    /// The graphical user interface which can be used to set-up and perform 
    /// pathfinding operations.
    /// </summary>
    public partial class Demo : Form
    {
        private static readonly double StartX = 1.3;
        private static readonly double StartY = 3.23;
        private static readonly double GoalX = 6.0;
        private static readonly double GoalY = 1.75;
        private static readonly int TimeOutInMillseconds = 1000;
        
        private readonly ILocationMarker startMarker;
        private readonly List<ILocationMarker> goalMarkers;
        private readonly IEnumerable<Triangle> triangles;
        private readonly Dictionary<ITriangle, DrawableTriangle> drawableTriangles;
        private readonly TPAStarPathFinder pathFinder;
        private readonly PoseDisplay poseDiplay;
        private readonly MetaDisplay metaDisplay;
        private PolyLine path;
        private ILocationMarker currentlyEditedMarker;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Demo"/> class which can be 
        /// used to set-up and perform pathfinding operations.
        /// </summary>
        public Demo()
        {
            InitializeComponent();

            var startPosition = new Vector(StartX, StartY);
            startMarker = new StartMarker(startPosition);
            goalMarkers = new List<ILocationMarker> { new GoalMarker(new Vector(GoalX, GoalY)) };
            currentlyEditedMarker = null;
            triangles = TriangleMaps.TrianglesOfPolygonWithTwoPolygonHoles;
            drawableTriangles = CreateTrianglesToDraw(triangles);
            pathFinder = new TPAStarPathFinder();
            pathFinder.TriangleExplored += PathFinderOnTriangleExplored;
            path = null;
            
            foreach (var triangle in drawableTriangles.Values)
            {
                display.AddDrawable(triangle);
            }
            display.AddDrawable(startMarker);
            foreach (var goalPoint in goalMarkers)
            {
                display.AddDrawable(goalPoint);
            }
            var title = new Title(15, 5);
            display.AddOverlay(title);
            var legend = new Legend(15, 75);
            display.AddOverlay(legend);
            metaDisplay = new MetaDisplay(startMarker, goalMarkers, 15, 15);
            poseDiplay = new PoseDisplay(15, 15);
            display.ScaleToFit();
            
            this.KeyUp += ShowHideMeta;
            display.KeyUp += ShowHideMeta;
            
            FindPathToGoal();
        }
        
        private void DisplayOnMouseDown(object sender, MouseEventArgs cursorState)
        {
            if (cursorState.Button == MouseButtons.Left)
            {
                var cursorAbsolutePosition = display.GetAbsolutePosition(cursorState.X, cursorState.Y);
                if (startMarker.IsPositionUnderMarker(cursorAbsolutePosition))
                {
                    currentlyEditedMarker = startMarker;
                }
                else
                {
                    currentlyEditedMarker = goalMarkers.FirstOrDefault(marker => marker.IsPositionUnderMarker(cursorAbsolutePosition));
                }
                
                if (currentlyEditedMarker == null)
                {
                    AddNewGoalPointAndSetToEdit(cursorState);
                }
            }
        }
        
        private void DisplayOnMouseMove(object sender, MouseEventArgs cursorState)
        {
            var absolutePosition = display.GetAbsolutePosition(cursorState.X, cursorState.Y);
            poseDiplay.SetCurrentPosition(absolutePosition);
            var triangleUnderCursor = triangles.FirstOrDefault(triangle => triangle.ContainsPoint(absolutePosition));
            if (triangleUnderCursor != null)
            {
                metaDisplay.SetSelectedTriangle(drawableTriangles[triangleUnderCursor]);
            }
            else
            {
                metaDisplay.ClearSelectedTriangle();
            }
            display.Invalidate();
            
            if (currentlyEditedMarker != null)
            {
                currentlyEditedMarker.SetLocation(absolutePosition);
                FindPathToGoal();
            }
        }

        private void DisplayOnMouseUp(object sender, MouseEventArgs cursorState)
        {
            if (cursorState.Button == MouseButtons.Right)
            {
                var cursorAbsolutePosition = display.GetAbsolutePosition(cursorState.X, cursorState.Y);
                var goalToDelete = goalMarkers.FirstOrDefault(marker => marker.IsPositionUnderMarker(cursorAbsolutePosition));
                if (goalToDelete != null)
                {
                    goalMarkers.Remove(goalToDelete);
                    display.RemoveDrawable(goalToDelete);
                    FindPathToGoal();
                }
            }
            currentlyEditedMarker = null;
        }
        
        private void PathFinderOnTriangleExplored(ITriangle triangle, TriangleEvaluationResult result)
        {
            drawableTriangles[triangle].AddMetaData(result);
        }
        
        private void DemoOnLoad(object sender, EventArgs e)
        {
            display.Invalidate();
        }

        private void ShowHideMeta(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.KeyCode == Keys.Space)
            {
                if (display.Draws(metaDisplay) && display.Draws(poseDiplay))
                {
                    display.RemoveOverlay(metaDisplay);
                    display.RemoveOverlay(poseDiplay);
                }
                else
                {
                    display.AddOverlay(metaDisplay);
                    display.AddOverlay(poseDiplay);
                }
            }
        }
        
        private void AddNewGoalPointAndSetToEdit(MouseEventArgs cursorState)
        {
            var newGoalPoint = display.GetAbsolutePosition(cursorState.X, cursorState.Y);
            var newGoal = new GoalMarker(newGoalPoint);
            goalMarkers.Add(newGoal);
            display.AddDrawable(newGoal);
            currentlyEditedMarker = newGoal;
            FindPathToGoal();
        }
        
        private void FindPathToGoal()
        {
            foreach (var triangle in drawableTriangles.Values)
            {
                triangle.ClearMetaData();
            }
            
            var startTriangle = triangles.FirstOrDefault(triangle => triangle.ContainsPoint(startMarker.CurrentLocation));
            if (startTriangle != null)
            {
                var cancellationToken = new CancellationTokenSource(TimeOutInMillseconds).Token;  
                
                Action<Task<IEnumerable<IVector>>> visualizePath = pathFindingOutcome =>
                {
                    if (pathFindingOutcome.IsFaulted)
                    {
                        throw pathFindingOutcome.Exception;
                    }
                    else
                    {
                        if (path != null)
                        {
                            path.SetVertices(pathFindingOutcome.Result);
                        }
                        else
                        {
                            path = new PolyLine(pathFindingOutcome.Result);
                            display.AddDrawable(path);
                        }
                        metaDisplay.SetPath(pathFindingOutcome.Result);
                    }
                };

                try
                {
                    Task<IEnumerable<IVector>>.Factory
                        .StartNew(() =>
                                pathFinder.FindPath(startMarker.CurrentLocation, startTriangle,
                                    goalMarkers.Select(point => point.CurrentLocation)),
                            cancellationToken)
                        .ContinueWith(visualizePath, cancellationToken)
                        .Wait(2 * TimeOutInMillseconds);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Finding path failed due to timeout or unexpected configuration. Details: \n\n" + e.ToString());
                }

            }
            else
            {
                display.RemoveDrawable(path);
                path = null;
                metaDisplay.ClearPath();
            }
            
            display.Invalidate();
        }
        
        private static Dictionary<ITriangle, DrawableTriangle> CreateTrianglesToDraw(IEnumerable<Triangle> triangles)
        {
            var trianglesToDraw = new Dictionary<ITriangle, DrawableTriangle>();
            foreach (var triangle in triangles)
            {
                var triangleIcon = new DrawableTriangle(triangle);
                trianglesToDraw.Add(triangle, triangleIcon);
            }
            return trianglesToDraw;
        }
    }
}
