using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices.ComTypes;
using TriangulatedPolygonAStar;
using TriangulatedPolygonAStar.BasicGeometry;
using TriangulatedPolygonAStar.UI.Resources;

namespace TriangulatedPolygonAStar.UI
{
    public partial class TPAStarDemonstration : Form
    {
        private IPoint start;
        private List<IPoint> goals;
        private IEnumerable<Triangle> triangles;
        
        private TPAStarPathFinder pathFinder;
        private PolyLine path;
        
        private Dictionary<ITriangle, DrawableTriangle> trianglesToDraw;
        private IPoint currentlyEditedPoint;
        
        public TPAStarDemonstration()
        {
            InitializeComponent();
            
            start = new StartPoint(new Vector(1, 5));
            goals = new List<IPoint> { new GoalPoint(new Vector(5.1, 2.6)) };
            currentlyEditedPoint = null;
            
            triangles = TriangleMaps.TrianglesOfPolygonWithOneHole;
            trianglesToDraw = CreateTrianglesToDraw(triangles);
            
            path = new PolyLine(Enumerable.Empty<IVector>());
            
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
            display.AddDrawable(path);
            
            FindPathToGoal();
        }

        private void PathFinderOnTriangleExplored(ITriangle triangle, TriangleEvaluationResult result)
        {
            trianglesToDraw[triangle].IncreaseTraversionCount(result);
        }

        private bool IsPointUnderCursor(IPoint point, MouseEventArgs cursorState)
        {
            var cursorAbsolutePosition = display.GetAbsolutePosition(cursorState.X, cursorState.Y);
            return cursorAbsolutePosition.DistanceFrom(point.Position) < 2*point.Radius; // TODO why 2 times?
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
                var builtPath = pathFinder.FindPath(start.Position, startTriangle, goals.Select(point => point.Position));
                path.SetPoints(builtPath);
            }
            else
            {
                path.SetPoints(Enumerable.Empty<IVector>());
            }
            display.Invalidate();
        }

        private void TPAStarDemonstration_Load(object sender, EventArgs e)
        {
            display.Invalidate();
        }

        private void display_MouseDown(object sender, MouseEventArgs cursorState)
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
        
        private void display_MouseMove(object sender, MouseEventArgs cursorState)
        {
            var absolutePosition = GetAbsoluteCoordinateSystemFromMouseState(cursorState);
            Text = absolutePosition.ToString();
            
            var configurationChanged = false;
            if (currentlyEditedPoint != null)
            {
                currentlyEditedPoint.SetPosition(absolutePosition);
                configurationChanged = true;
            }
            
            if (configurationChanged)
            {
                FindPathToGoal();   
            }
        }

        private void display_MouseUp(object sender, MouseEventArgs cursorState)
        {
            var configurationChanged = false;
            currentlyEditedPoint = null;
            
            if (cursorState.Button == MouseButtons.Right)
            {
                var goalToDelete = goals.FirstOrDefault(goal => IsPointUnderCursor(goal, cursorState));
                if (goalToDelete != null && goals.Count > 1)
                {
                    goals.Remove(goalToDelete);
                    display.RemoveDrawable(goalToDelete);
                    configurationChanged = true;
                }
            }
            
            if (configurationChanged)
            {
                FindPathToGoal();   
            }
        }

        private IVector GetAbsoluteCoordinateSystemFromMouseState(MouseEventArgs mouseState)
        {
            return display.GetAbsolutePosition(mouseState.X, mouseState.Y); // it works only because canvas starts at (0,0)
        }

        private static Dictionary<ITriangle, DrawableTriangle> CreateTrianglesToDraw(IEnumerable<Triangle> triangles)
        {
            var trianglesToDraw = new Dictionary<ITriangle, DrawableTriangle>();
            var id = 0;
            foreach (var triangle in triangles)
            {
                var triangleIcon = new DrawableTriangle(triangle, "t" + id);
                trianglesToDraw.Add(triangle, triangleIcon);
                id++;
            }
            return trianglesToDraw;
        }

    }
}
