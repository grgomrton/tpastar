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
        private IVector start;
        private List<IVector> goals;
        private IEnumerable<Triangle> triangles;
        
        private TPAStarPathFinder pathFinder;
        private IEnumerable<IVector> path;
        
        private Dictionary<ITriangle, TriangleIcon> trianglesToDraw;
        private int? currentlyEditedGoalPointIndex;
        private bool startPointIsBeingModified;
        
        public TPAStarDemonstration()
        {
            InitializeComponent();
            start = new Vector(1, 5);
            startPointIsBeingModified = false;
            var goal = new Vector(5.1, 2.6);
            goals = new List<IVector> { goal };
            currentlyEditedGoalPointIndex = null;
            triangles = TriangleMaps.TrianglesOfPolygonWithOneHole;
            trianglesToDraw = CreateTrianglesToDraw(triangles);
            AddTriangleIconsToCanvas(trianglesToDraw.Values, display);
            AddPointAndPathToCanvas();
            pathFinder = new TPAStarPathFinder();
            pathFinder.TriangleExplored += PathFinderOnTriangleExplored;
            FindPathToGoal();
        }

        private void PathFinderOnTriangleExplored(ITriangle triangle, TriangleEvaluationResult result)
        {
            trianglesToDraw[triangle].IncreaseTraversionCount(result);
        }

        private bool IsStartPointUnderCursor(MouseEventArgs cursorState)
        {
            return IsPointUnderCursor(start, cursorState);
        }

        private bool IsPointUnderCursor(IVector point, MouseEventArgs cursorState) 
        {
            if (point.DistanceFrom(display.ConvertCanvasPositionToAbsolutePosition(cursorState.X, cursorState.Y)) < 0.15) // TODO: param
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void FindPathToGoal()
        {
            foreach (var triangle in trianglesToDraw.Values)
            {
                triangle.ResetMetaData();
            }
            var startTriangle = triangles.FirstOrDefault(triangle => triangle.ContainsPoint(start));
            if (startTriangle != null)
            {
                path = pathFinder.FindPath(start, startTriangle, goals);
            }
            else
            {
                path = Enumerable.Empty<IVector>();
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
                if (IsStartPointUnderCursor(cursorState))
                {
                    startPointIsBeingModified = true;
                }
                else
                {
                    var goalToEdit = goals.FirstOrDefault(point => IsPointUnderCursor(point, cursorState));
                    if (goalToEdit != null)
                    {
                        currentlyEditedGoalPointIndex = goals.IndexOf(goalToEdit);
                    }
                    else
                    {
                        AddNewGoalPointAndSetToEdit(cursorState);
                    }
                }
            }
        }

        private void AddNewGoalPointAndSetToEdit(MouseEventArgs cursorState)
        {
            var newGoalPoint = display.ConvertCanvasPositionToAbsolutePosition(cursorState.X, cursorState.Y); 
            goals.Add(newGoalPoint);
            currentlyEditedGoalPointIndex = goals.IndexOf(newGoalPoint);
            FindPathToGoal();
        }
        
        private void display_MouseMove(object sender, MouseEventArgs cursorState)
        {
            this.Text = display.ConvertCanvasPositionToAbsolutePosition(cursorState.X, cursorState.Y).ToString();
            
            var configurationChanged = false;
            
            if (currentlyEditedGoalPointIndex.HasValue)
            {
                goals[currentlyEditedGoalPointIndex.Value] = display.ConvertCanvasPositionToAbsolutePosition(cursorState.X, cursorState.Y);
                configurationChanged = true;
            }
            else if (startPointIsBeingModified)
            {
                start = display.ConvertCanvasPositionToAbsolutePosition(cursorState.X, cursorState.Y); // it works only because canvas starts at (0,0)
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

            currentlyEditedGoalPointIndex = null;
            startPointIsBeingModified = false;
            if (cursorState.Button == MouseButtons.Right)
            {
                var goalToDelete = goals.FirstOrDefault(point => IsPointUnderCursor(point, cursorState));
                if (goalToDelete != null && goals.Count > 1)
                {
                    goals.Remove(goalToDelete);
                    configurationChanged = true;
                }
            }
            
            if (configurationChanged)
            {
                FindPathToGoal();   
            }
        }


        private static Dictionary<ITriangle, TriangleIcon> CreateTrianglesToDraw(IEnumerable<Triangle> triangles)
        {
            var trianglesToDraw = new Dictionary<ITriangle, TriangleIcon>();
            var id = 0;
            foreach (var triangle in triangles)
            {
                var triangleIcon = new TriangleIcon(triangle, "t" + id);
                trianglesToDraw.Add(triangle, triangleIcon);
                id++;
            }
            return trianglesToDraw;
        }

        private static void AddTriangleIconsToCanvas(IEnumerable<TriangleIcon> triangles, Canvas canvas)
        {
            foreach (var triangle in triangles)
            {
                canvas.AddDrawMethod(triangle.Draw, null, null);
                canvas.AddDrawMethod(triangle.DrawMetaData, null, null);
            }
        }
        
        private void AddPointAndPathToCanvas()
        {
            Dictionary<string, Color> pathColors = new Dictionary<string, Color>();
            pathColors.Add("edge", Color.Green);
            pathColors.Add("data", Color.Black);
            Dictionary<string, float> pathWidths = new Dictionary<string, float>();
            pathWidths.Add("edge", 0.04f);
            pathWidths.Add("data", 0.12f);
            pathWidths.Add("fontSize", 0.12f);

            display.AddDrawMethod(this.DrawPath, pathColors, pathWidths);

            Dictionary<string, Color> startPoseColors = new Dictionary<string, Color>();
            startPoseColors.Add("fill", Color.Blue);
            Dictionary<string, float> poseWidths = new Dictionary<string, float>();
            poseWidths.Add("radius", 0.08f);
            display.AddDrawMethod(this.DrawStart, startPoseColors, poseWidths);

            Dictionary<string, Color> goalPoseColors = new Dictionary<string, Color>();
            goalPoseColors.Add("fill", Color.Green);
            display.AddDrawMethod(this.DrawGoals, goalPoseColors, poseWidths);
        }

        private void DrawPath(Graphics canvas, Dictionary<string, Color> colors, Dictionary<string, float> widths)
        {
            if (path.Count() > 0)
            {
                List<PointF> nodes = new List<PointF>();
                foreach (Vector v in path)
                {
                    nodes.Add(v.ToPointF());
                }
                canvas.DrawLines(new Pen(colors["edge"], widths["edge"]), nodes.ToArray());
                float fontSize = widths["fontSize"];
                var position = path.Last().Minus(new Vector(2 * fontSize, 3 * fontSize));
                var positionFloat = new PointF(Convert.ToSingle(position.X), Convert.ToSingle(position.Y));
                canvas.DrawString(path.Length().ToString("#.##"), new Font("Arial", fontSize, FontStyle.Bold), new SolidBrush(colors["data"]), positionFloat);
            }
        }

        private void DrawStart(Graphics canvas, Dictionary<string, Color> colors, Dictionary<string, float> widths)
        {
            DrawPoint(start, canvas, colors, widths);
        }

        private void DrawGoals(Graphics canvas, Dictionary<string, Color> colors, Dictionary<string, float> widths)
        {
            foreach (Vector goal in goals)
            {
                DrawPoint(goal, canvas, colors, widths);
            }
        }

        private void DrawPoint(IVector point, Graphics canvas, Dictionary<string, Color> colors, Dictionary<string, float> widths)
        {
            Brush brush = new SolidBrush(colors["fill"]);
            float radius = widths["radius"];

            float x = Convert.ToSingle(point.X) - radius;
            float y = Convert.ToSingle(point.Y) - radius;
            float diameter = 2 * radius;

            canvas.FillEllipse(brush, x, y, diameter, diameter);
        }

    }
}
