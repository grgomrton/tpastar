using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommonTools.Geometry;
using PathFinder.TPAStar;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace TPAStarGUI
{
    public partial class TPAStarDemonstration : Form
    {
        private Vector3 start;
        private Triangle[] triangles;
        private List<Vector3> goals;
        private Curve path;
        
        private TPAStarSolver solver;
        private Dictionary<ITriangle, TriangleIcon> trianglesToDraw;
        
        private int editingIndex = 0;
        private bool goalEditing = false;
        private bool startEditing = false;
        private Triangle startTriangle;
        
        public TPAStarDemonstration()
        {
            InitializeComponent();
            
            CreateTriangleMap();
            AddDrawMethods();
            solver = new TPAStarSolver();
            solver.TriangleExplored += SolverOnTriangleExplored;

            FindPathToGoal();
        }

        private void ResetDisplayMetaData()
        {
            foreach (var triangle in triangles)
            {
                trianglesToDraw[triangle].ResetMetaData();
            }
        }

        private void SolverOnTriangleExplored(ITriangle triangle, TriangleEvaluationResult result)
        {
            trianglesToDraw[triangle].IncreaseTraversionCount(result);
            
        }

        private void SetSartTriangle()
        {
            startTriangle = null;
            
            for (int i = 0; i < triangles.Length; i++)
            {
                if (triangles[i].ContainsPoint(start))
                {
                    startTriangle = triangles[i];
                }
            }
            if (startTriangle == null)
            {
                double mindistance = 0;
                for (int i = 0; i < triangles.Length; i++)
                {
                    double dist = triangles[i].MinDistanceFromOuterPoint(start);
                    if ((mindistance == 0) || (dist < mindistance))
                    {
                        startTriangle = triangles[i];
                        mindistance = dist;
                    }
                }
            }
        }

        private void SetEditingIndex(MouseEventArgs e)
        {
            bool found = false;
            for (int i = 0; (i < goals.Count) && (!found); i++)
            {
                if (mouseOnPoint(goals[i], e))
                {
                    found = true;
                    editingIndex = i;
                }
            }
            if (!found) // new goalPoint have to be added
            {
                goals.Add(display.GetAbsolutePosition(e.X, e.Y));
                editingIndex = goals.Count - 1;
            }
        }

        private bool mouseOnStartPoint(MouseEventArgs e)
        {
            return mouseOnPoint(start, e);
        }

        private bool mouseOnPoint(Vector3 point, MouseEventArgs e) 
        {
            if (point.Distance(display.GetAbsolutePosition(e.X, e.Y)) < 0.15) // TODO: param
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
            ResetDisplayMetaData();
            
            path = solver.FindPath(start, startTriangle, goals.ToArray());
        }

        private void TPAStarDemonstration_Load(object sender, EventArgs e)
        {
            display.Invalidate();
        }

        private void display_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (mouseOnStartPoint(e))
                {
                    startEditing = true;
                }
                else
                {
                    SetEditingIndex(e);
                    goalEditing = true;
                }
            }
        }

        private void display_MouseMove(object sender, MouseEventArgs e)
        {
            Vector3 newVector = display.GetAbsolutePosition(e.X, e.Y);

            if (goalEditing)
            {
                goals[editingIndex] = newVector;
            }
            else if (startEditing)
            {
                start = newVector;
                SetSartTriangle();
            }
            FindPathToGoal();
            display.Invalidate();
        }

        private void display_MouseUp(object sender, MouseEventArgs e)
        {
            goalEditing = false;
            startEditing = false;
            if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < goals.Count; i++)
                {
                    if (mouseOnPoint(goals[i], e) && goals.Count > 1)
                    {
                        goals.RemoveAt(i);
                    }
                }
            }
            FindPathToGoal();
            display.Invalidate();
        }

        private void CreateTriangleMap() {
            trianglesToDraw = new Dictionary<ITriangle, TriangleIcon>();
            
            Vector3 cr0 = new Vector3(2, 4, 0);
            Vector3 cr1 = new Vector3(2, 3, 0);
            Vector3 cr2 = new Vector3(3, 2, 0);
            Vector3 cr3 = new Vector3(5, 3, 0);
            Vector3 cr4 = new Vector3(7, 2, 0);
            Vector3 cl0 = new Vector3(0, 4, 0);
            Vector3 cl1 = new Vector3(0, 3, 0);
            Vector3 cl2 = new Vector3(3, 1, 0);
            Vector3 cl3 = new Vector3(5, 2.5, 0);
            Vector3 cl4 = new Vector3(6, 1, 0);

            Vector3 cp0 = new Vector3(1, 7, 0);
            Vector3 cp1 = new Vector3(6.5, 0, 0);
            Vector3 cp2 = new Vector3(0, 9, 0);
            Vector3 cp3 = new Vector3(1, 10, 0);
            Vector3 cp4 = new Vector3(0, 11, 0);

            start = new Vector3(1, 5, 0);

            goals = new List<Vector3>();
            goals.Add(new Vector3(5.1, 2.6, 0));

            Triangle t0 = new Triangle(cp0, cl0, cr0, 0);
            CreateDrawableTriangle(t0, "t0");
            Triangle t1 = new Triangle(cl0, cr0, cl1, 1);
            CreateDrawableTriangle(t1, "t1");
            Triangle t2 = new Triangle(cl1, cr0, cr1, 2);
            CreateDrawableTriangle(t2, "t2");
            Triangle t3 = new Triangle(cl1, cr1, cl2, 3);
            CreateDrawableTriangle(t3, "t3");
            Triangle t4 = new Triangle(cr1, cl2, cr2, 4);
            CreateDrawableTriangle(t4, "t4");
            Triangle t5 = new Triangle(cr2, cl2, cr3, 5);
            CreateDrawableTriangle(t5, "t5");
            Triangle t6 = new Triangle(cl3, cl2, cr3, 6);
            CreateDrawableTriangle(t6, "t6");
            Triangle t7 = new Triangle(cl3, cl4, cr3, 7);
            CreateDrawableTriangle(t7, "t7");
            Triangle t8 = new Triangle(cr4, cl4, cr3, 8);
            CreateDrawableTriangle(t8, "t8");
            Triangle t9 = new Triangle(cr4, cl4, cp1, 9);
            CreateDrawableTriangle(t9, "t9");
            Triangle t10 = new Triangle(cr0, cp0, cr3, 10);
            CreateDrawableTriangle(t10, "t10");
            Triangle t11 = new Triangle(cr4, cp0, cr3, 11);
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
            
            startTriangle = t0;
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
            if (path.Length > 0)
            {
                List<PointF> nodes = new List<PointF>();
                foreach (Vector3 v in path)
                {
                    nodes.Add(v.ToPointF());
                }
                canvas.DrawLines(new Pen(colors["edge"], widths["edge"]), nodes.ToArray());
                float fontSize = widths["fontSize"];
                canvas.DrawString(path.Length.ToString("#.##"), new Font("Arial", fontSize, FontStyle.Bold), new SolidBrush(colors["data"]), (path.Last() - new Vector3(2 * fontSize, 3 * fontSize, 0)).ToPointF());
            }
        }

        private void DrawStart(Graphics canvas, Dictionary<string, Color> colors, Dictionary<string, float> widths)
        {
            DrawPoint(start, canvas, colors, widths);
        }

        private void DrawGoals(Graphics canvas, Dictionary<string, Color> colors, Dictionary<string, float> widths)
        {
            foreach (Vector3 goal in goals)
            {
                DrawPoint(goal, canvas, colors, widths);
            }
        }

        private void DrawPoint(Vector3 point, Graphics canvas, Dictionary<string, Color> colors, Dictionary<string, float> widths)
        {
            Brush brush = new SolidBrush(colors["fill"]);
            float radius = widths["radius"];

            float x = point.Xf - radius;
            float y = point.Yf - radius;
            float diameter = 2 * radius;

            canvas.FillEllipse(brush, x, y, diameter, diameter);
        }

    }
}
