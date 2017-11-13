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
    /// The graphical user interface which can be used to set-up and perform 
    /// pathfinding operations.
    /// </summary>
    public partial class Demo : Form
    {
        private Point start;
        private List<Point> goals;
        private Point currentlyEditedPoint;
        
        private IEnumerable<Triangle> triangles;
        private Dictionary<ITriangle, DrawableTriangle> trianglesToDraw;
        
        private TPAStarPathFinder pathFinder;
        private PolyLine path;

        private static int TimeOutInMillseconds = 1000;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Demo"/> class which can be 
        /// used to set-up and perform pathfinding operations.
        /// </summary>
        public Demo()
        {
            InitializeComponent();

            var startPosition = new Vector(1.0, 5.0);
            start = new StartPoint(startPosition);
            goals = new List<Point> { new GoalPoint(new Vector(5.1, 2.6)) };
            currentlyEditedPoint = null;
            path = null;

            triangles = TriangleMaps.TrianglesOfPolygonWithTwoPolygonHoles;
            trianglesToDraw = CreateTrianglesToDraw(triangles);
            
            pathFinder = new TPAStarPathFinder();
            pathFinder.TriangleExplored += PathFinderOnTriangleExplored;
            
            foreach (var triangle in trianglesToDraw.Values)
            {
                display.AddDrawable(triangle);
            }
            display.AddDrawable(start);
            foreach (var goalPoint in goals)
            {
                display.AddDrawable(goalPoint);
            }
            var legend = new Legend(10, 10);
            display.AddOverlay(legend);
            
            display.ScaleToFit();
            FindPathToGoal();
        }

        private void PathFinderOnTriangleExplored(ITriangle triangle, TriangleEvaluationResult result)
        {
            trianglesToDraw[triangle].IncreaseTraversionCount(result);
        }

        private bool IsPointUnderCursor(Point point, MouseEventArgs cursorState)
        {
            var cursorAbsolutePosition = display.GetAbsolutePosition(cursorState.X, cursorState.Y);
            return cursorAbsolutePosition.DistanceFrom(point.Position) < 2*point.Radius;
        }
        
        private void FindPathToGoal()
        {
            foreach (var triangle in trianglesToDraw.Values)
            {
                triangle.ResetMetaData();
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
            Text = "Triangulated Polygon A-star demo " + absolutePosition;
            
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
            return display.GetAbsolutePosition(cursorState.X, cursorState.Y); // it works only because canvas starts at (0,0)
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
