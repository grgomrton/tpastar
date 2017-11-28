/**
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
    /// The user interface which can be used to set-up and execute 
    /// pathfindings.
    /// </summary>
    public partial class Demo : Form
    {
        private static readonly double StartX = 1.0;
        private static readonly double StartY = 3.25;
        private static readonly double GoalX = 6.0;
        private static readonly double GoalY = 1.75;
        private static readonly int TimeoutInMillseconds = 1000;
        
        private readonly ILocationMarker startMarker;
        private readonly List<ILocationMarker> goalMarkers;
        private readonly IEnumerable<Triangle> triangles;
        private readonly Dictionary<ITriangle, DrawableTriangle> drawableTriangles;
        private readonly TPAStarPathFinder pathFinder;
        private readonly PoseDisplay poseDiplay;
        private readonly MetaDisplay metaDisplay;
        private readonly PolyLine pathDisplay;
        private ILocationMarker currentlyEditedMarker;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Demo"/> class which can be 
        /// used to set-up and execute pathfindings.
        /// </summary>
        public Demo()
        {
            InitializeComponent();

            var startPosition = new Vector(StartX, StartY);
            startMarker = new StartMarker(startPosition);
            goalMarkers = new List<ILocationMarker> { new GoalMarker(new Vector(GoalX, GoalY)) };
            currentlyEditedMarker = null;
            triangles = TriangleMaps.CreateTriangleMapOfPolygonWithTwoPolygonHoles();
            drawableTriangles = CreateTrianglesToDraw(triangles);
            pathFinder = new TPAStarPathFinder();
            pathFinder.TriangleExplored += PathFinderOnTriangleExplored;
            pathDisplay = new PolyLine();
            
            foreach (var triangle in drawableTriangles.Values)
            {
                display.AddDrawable(triangle);
            }
            display.AddDrawable(startMarker);
            foreach (var goalPoint in goalMarkers)
            {
                display.AddDrawable(goalPoint);
            }
            display.AddDrawable(pathDisplay);
            var title = new Title(15, 5);
            display.AddOverlay(title);
            var legend = new Legend(15, 75);
            display.AddOverlay(legend);
            metaDisplay = new MetaDisplay(startMarker, goalMarkers, pathDisplay, 15, 15);
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
                var cancellationToken = new CancellationTokenSource(TimeoutInMillseconds).Token;  
                try
                {
                    Task<LinkedList<IVector>>.Factory
                        .StartNew(() => PathFindingExecution(pathFinder, startMarker.CurrentLocation, startTriangle, goalMarkers.Select(point => point.CurrentLocation)), cancellationToken)
                        .ContinueWith(result => AssertPathFindingOutcome(pathDisplay, result), cancellationToken)
                        .Wait(2 * TimeoutInMillseconds);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Pathfinding failed due to the reason below.\n\n" + e);
                }   
            }
            else
            {
                pathDisplay.SetVertices(Enumerable.Empty<IVector>());
            }
            
            display.Invalidate();
        }

        private static LinkedList<IVector> PathFindingExecution(TPAStarPathFinder pathFinder, IVector start, ITriangle startTriangle, IEnumerable<IVector> goals)
        {
            return pathFinder.FindPath(start, startTriangle, goals);
        }

        private static void AssertPathFindingOutcome(PolyLine pathDisplay, Task<LinkedList<IVector>> result)
        {
            if (result.IsFaulted)
            {
                throw result.Exception.InnerException;
            }
            else
            {
                pathDisplay.SetVertices(result.Result);
            }
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