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
using System.Drawing.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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
        private static readonly double StartX = 1.8;
        private static readonly double StartY = 2.4;
        private static readonly double GoalX = 6.0;
        private static readonly double GoalY = 1.75;
        private static readonly int TimeOutInMillseconds = 1000;
        
        private Point start;
        private List<Point> goals;
        private IEnumerable<Triangle> triangles;
        private TPAStarPathFinder pathFinder;
        
        private PolyLine path;
        private PoseDisplay poseDiplay;
        private MetaDisplay metaDisplay;
        private Dictionary<ITriangle, DrawableTriangle> drawableTriangles;
        private Point currentlyEditedPoint;
        private ITriangle triangleUnderCursor;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Demo"/> class which can be 
        /// used to set-up and perform pathfinding operations.
        /// </summary>
        public Demo()
        {
            InitializeComponent();

            var startPosition = new Vector(StartX, StartY);
            start = new StartPoint(startPosition);
            goals = new List<Point> { new GoalPoint(new Vector(GoalX, GoalY)) };
            currentlyEditedPoint = null;
            path = null;

            triangles = TriangleMaps.TrianglesOfPolygonWithTwoPolygonHoles;
            drawableTriangles = CreateTrianglesToDraw(triangles);
            triangleUnderCursor = null;
            
            pathFinder = new TPAStarPathFinder();
            pathFinder.TriangleExplored += PathFinderOnTriangleExplored;
            
            foreach (var triangle in drawableTriangles.Values)
            {
                display.AddDrawable(triangle);
            }
            display.AddDrawable(start);
            foreach (var goalPoint in goals)
            {
                display.AddDrawable(goalPoint);
            }
            var title = new Title(15, 5);
            display.AddOverlay(title);
            var legend = new Legend(15, 75);
            display.AddOverlay(legend);
            metaDisplay = new MetaDisplay(start, goals, 15, 15);
            poseDiplay = new PoseDisplay(15, 15);

            this.KeyUp += ShowHideMeta;
            display.KeyUp += ShowHideMeta;
            
            display.ScaleToFit();
            FindPathToGoal();
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

        private void PathFinderOnTriangleExplored(ITriangle triangle, TriangleEvaluationResult result)
        {
            drawableTriangles[triangle].AddMetaData(result);
        }

        private bool IsPointUnderCursor(Point point, MouseEventArgs cursorState)
        {
            var cursorAbsolutePosition = display.GetAbsolutePosition(cursorState.X, cursorState.Y);
            return cursorAbsolutePosition.DistanceFrom(point.Position) < 2*point.Radius;
        }
        
        private void FindPathToGoal()
        {
            foreach (var triangle in drawableTriangles.Values)
            {
                triangle.ClearMetaData();
            }
            
            var startTriangle = triangles.FirstOrDefault(triangle => triangle.ContainsPoint(start.Position));
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
                                pathFinder.FindPath(start.Position, startTriangle,
                                    goals.Select(point => point.Position)),
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

        private void DemoOnLoad(object sender, EventArgs e)
        {
            display.Invalidate();
        }

        private void DisplayOnMouseDown(object sender, MouseEventArgs cursorState)
        {
            if (cursorState.Button == MouseButtons.Left)
            {
                var points = goals.Concat(new[] {start});
                currentlyEditedPoint = points.FirstOrDefault(point => IsPointUnderCursor(point, cursorState));
                if (currentlyEditedPoint == null)
                {
                    AddNewGoalPointAndSetToEdit(cursorState);
                }
            }
        }

        private void AddNewGoalPointAndSetToEdit(MouseEventArgs cursorState)
        {
            var newGoalPoint = display.GetAbsolutePosition(cursorState.X, cursorState.Y);
            var newGoal = new GoalPoint(newGoalPoint);
            goals.Add(newGoal);
            display.AddDrawable(newGoal);
            currentlyEditedPoint = newGoal;
            FindPathToGoal();
        }
        
        private void DisplayOnMouseMove(object sender, MouseEventArgs cursorState)
        {
            var absolutePosition = GetAbsoluteCoordinateFromMousePosition(cursorState);
            poseDiplay.SetCurrentPosition(absolutePosition);
            triangleUnderCursor = triangles.FirstOrDefault(triangle => triangle.ContainsPoint(absolutePosition));
            if (triangleUnderCursor != null)
            {
                metaDisplay.SetSelectedTriangle(drawableTriangles[triangleUnderCursor]);
            }
            else
            {
                metaDisplay.ClearSelectedTriangle();
            }
            display.Invalidate();
            
            if (currentlyEditedPoint != null)
            {
                currentlyEditedPoint.SetPosition(absolutePosition);
                FindPathToGoal();
            }
        }

        private void DisplayOnMouseUp(object sender, MouseEventArgs cursorState)
        {
            if (cursorState.Button == MouseButtons.Right)
            {
                var goalToDelete = goals.FirstOrDefault(goal => IsPointUnderCursor(goal, cursorState));
                if (goalToDelete != null)
                {
                    goals.Remove(goalToDelete);
                    display.RemoveDrawable(goalToDelete);
                    FindPathToGoal();
                }
            }
            currentlyEditedPoint = null;
        }

        private IVector GetAbsoluteCoordinateFromMousePosition(MouseEventArgs cursorState)
        {
            return display.GetAbsolutePosition(cursorState.X, cursorState.Y);
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
