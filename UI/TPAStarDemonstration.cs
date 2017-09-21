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

namespace TPAStarGUI
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

            currentlyEditedGoalPointIndex = null;
            startPointIsBeingModified = false;
            CreateTriangleMap();
            AddDrawMethods();
            pathFinder = new TPAStarPathFinder();
            pathFinder.TriangleExplored += PathFinderOnTriangleExplored;
        }

        private void ClearMetaData()
        {
            foreach (var triangle in triangles)
            {
                trianglesToDraw[triangle].ResetMetaData();
            }
        }

        private void PathFinderOnTriangleExplored(ITriangle triangle, TriangleEvaluationResult result)
        {
            trianglesToDraw[triangle].IncreaseTraversionCount(result);
        }

        private bool IsStartPointUnderCursor(MouseEventArgs e)
        {
            return IsPointUnderCursor(start, e);
        }

        private bool IsPointUnderCursor(IVector point, MouseEventArgs e) 
        {
            if (point.DistanceFrom(display.ConvertCanvasPositionToAbsolutePosition(e.X, e.Y)) < 0.15) // TODO: param
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
            ClearMetaData();
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
            FindPathToGoal();
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

        private void CreateTriangleMap() {
            trianglesToDraw = new Dictionary<ITriangle, TriangleIcon>();
            
            Vector cr0 = new Vector(2, 4);
            Vector cr1 = new Vector(2, 3);
            Vector cr2 = new Vector(3, 2);
            Vector cr3 = new Vector(5, 3);
            Vector cr4 = new Vector(7, 2);
            Vector cl0 = new Vector(0, 4);
            Vector cl1 = new Vector(0, 3);
            Vector cl2 = new Vector(3, 1);
            Vector cl3 = new Vector(5, 2.5);
            Vector cl4 = new Vector(6, 1);

            Vector cp0 = new Vector(1, 7);
            Vector cp1 = new Vector(6.5, 0);
            Vector cp2 = new Vector(0, 9);
            Vector cp3 = new Vector(1, 10);
            Vector cp4 = new Vector(0, 11);

            start = new Vector(1, 5);

            goals = new List<IVector>();
            goals.Add(new Vector(5.1, 2.6));

            Triangle t0 = new Triangle(cp0, cl0, cr0);
            CreateDrawableTriangle(t0, "t0");
            Triangle t1 = new Triangle(cl0, cr0, cl1);
            CreateDrawableTriangle(t1, "t1");
            Triangle t2 = new Triangle(cl1, cr0, cr1);
            CreateDrawableTriangle(t2, "t2");
            Triangle t3 = new Triangle(cl1, cr1, cl2);
            CreateDrawableTriangle(t3, "t3");
            Triangle t4 = new Triangle(cr1, cl2, cr2);
            CreateDrawableTriangle(t4, "t4");
            Triangle t5 = new Triangle(cr2, cl2, cr3);
            CreateDrawableTriangle(t5, "t5");
            Triangle t6 = new Triangle(cl3, cl2, cr3);
            CreateDrawableTriangle(t6, "t6");
            Triangle t7 = new Triangle(cl3, cl4, cr3);
            CreateDrawableTriangle(t7, "t7");
            Triangle t8 = new Triangle(cr4, cl4, cr3);
            CreateDrawableTriangle(t8, "t8");
            Triangle t9 = new Triangle(cr4, cl4, cp1);
            CreateDrawableTriangle(t9, "t9");
            Triangle t10 = new Triangle(cr0, cp0, cr3);
            CreateDrawableTriangle(t10, "t10");
            Triangle t11 = new Triangle(cr4, cp0, cr3);
            CreateDrawableTriangle(t11, "t11");
            
            t0.SetNeighbours(t1, t10);
            t1.SetNeighbours(t2, t0);
            t2.SetNeighbours(t3, t1);
            t3.SetNeighbours(t4, t2);
            t4.SetNeighbours(t5, t3);
            t5.SetNeighbours( t6, t4);
            t6.SetNeighbours(t7, t5);
            t7.SetNeighbours(t8, t6);
            t8.SetNeighbours(t9, t7, t11);
            t9.SetNeighbours(t8);
            t10.SetNeighbours(t0, t11);
            t11.SetNeighbours(t8, t10);

            triangles = new [] { t0, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11 };

            foreach (var t in trianglesToDraw.Values)
            {
                display.AddDrawMethod(t.DrawMetaData, null, null);
            }
        }

        private void CreateDrawableTriangle(Triangle triangle, String displayName)
        {
            TriangleIcon triangleIcon = new TriangleIcon(triangle, displayName);
            trianglesToDraw.Add(triangle, triangleIcon);
            display.AddDrawMethod(triangleIcon.Draw, null, null);
        }
        
        private void AddDrawMethods()
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
