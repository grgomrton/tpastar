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

namespace TPAStarGUI
{
    public partial class TPAStarDemonstration : Form
    {
        Vector3 start;
        Triangle[] triangles;
        List<Vector3> goals;
        Curve path;

        int editingIndex = 0;
        bool goalEditing = false, startEditing = false;
        Triangle startTriangle;

        Dictionary<string, Color> triangleColors;
        Dictionary<string, float> triangleWidths;

        public TPAStarDemonstration()
        {
            InitializeComponent();
        }

        private void DrawDots(Graphics canvas, Vector3[] vectors, Dictionary<string, Color> colors, Dictionary<string, float> widths)
        {
            for (int i = 0; i < vectors.Length; i++)
            {
                vectors[i].Draw(canvas, colors, widths);
            }
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
            for (int i = 0; i < triangles.Length; i++)
            {
                triangles[i].ResetMetaData();
            }
            path = TPAStarAlgorithm.FindPath(start, startTriangle, goals.ToArray());
        }

        private void TPAStarDemonstration_Load(object sender, EventArgs e)
        {
            InitTriangles();

            FindPathToGoal();

            AddDrawMethods();

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
                    if (mouseOnPoint(goals[i], e))
                    {
                        goals.RemoveAt(i);
                    }
                }
            }
            FindPathToGoal();
            display.Invalidate();
        }

        private void InitTriangles() {
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
            Triangle t1 = new Triangle(cl0, cr0, cl1, 1);
            Triangle t2 = new Triangle(cl1, cr0, cr1, 2);
            Triangle t3 = new Triangle(cl1, cr1, cl2, 3);
            Triangle t4 = new Triangle(cr1, cl2, cr2, 4);
            Triangle t5 = new Triangle(cr2, cl2, cr3, 5);
            Triangle t6 = new Triangle(cl3, cl2, cr3, 6);
            Triangle t7 = new Triangle(cl3, cl4, cr3, 7);
            Triangle t8 = new Triangle(cr4, cl4, cr3, 8);
            Triangle t9 = new Triangle(cr4, cl4, cp1, 9);
            Triangle t10 = new Triangle(cr0, cp0, cr3, 10);
            Triangle t11 = new Triangle(cr4, cp0, cr3, 11);

            Triangle[] n0 = { t1, t10 };
            t0.SetNeighbours(n0);
            Triangle[] n1 = { t2, t0 };
            t1.SetNeighbours(n1);
            Triangle[] n2 = { t3, t1 };
            t2.SetNeighbours(n2);
            Triangle[] n3 = { t4, t2 };
            t3.SetNeighbours(n3);
            Triangle[] n4 = { t5, t3 };
            t4.SetNeighbours(n4);
            Triangle[] n5 = { t6, t4 };
            t5.SetNeighbours(n5);
            Triangle[] n6 = { t7, t5 };
            t6.SetNeighbours(n6);
            Triangle[] n7 = { t8, t6 };
            t7.SetNeighbours(n7);
            Triangle[] n8 = { t9, t7, t11 };
            t8.SetNeighbours(n8);
            Triangle[] n9 = { t8 };
            t9.SetNeighbours(n9);
            Triangle[] n10 = { t0, t11 };
            t10.SetNeighbours(n10);
            Triangle[] n11 = { t8, t10 };
            t11.SetNeighbours(n11);

            triangles = new Triangle[] { t0, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11 };

            startTriangle = t0;
        }

        private void AddDrawMethods()
        {
            triangleColors = new Dictionary<string, Color>();
            triangleColors.Add("fill", Color.White);
            triangleColors.Add("traversionShade", Color.FromArgb(50, 50, 50));
            triangleColors.Add("edge", Color.Gray);
            triangleColors.Add("data", Color.Black);
            triangleWidths = new Dictionary<string, float>();
            triangleWidths.Add("edge", 0.01f);
            triangleWidths.Add("fontSize", 0.12f);

            foreach (Triangle t in triangles)
            {
                display.AddDrawMethod(t.Draw, triangleColors, triangleWidths);
            }
            foreach (Triangle t in triangles)
            {
                display.AddDrawMethod(t.DrawMeta, triangleColors, triangleWidths);
            }
            Dictionary<string, Color> pathColors = new Dictionary<string, Color>();
            pathColors.Add("edge", Color.Green);
            pathColors.Add("data", Color.Black);
            Dictionary<string, float> pathWidths = new Dictionary<string, float>();
            pathWidths.Add("edge", 0.04f);
            pathWidths.Add("data", 0.12f);

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
                path.Draw(canvas, colors, widths);
                path.DrawMeta(canvas, triangleColors, triangleWidths);
            }
        }

        private void DrawStart(Graphics canvas, Dictionary<string, Color> colors, Dictionary<string, float> widths)
        {
            start.Draw(canvas, colors, widths);
        }

        private void DrawGoals(Graphics canvas, Dictionary<string, Color> colors, Dictionary<string, float> widths)
        {
            foreach (Vector3 goal in goals)
            {
                goal.Draw(canvas, colors, widths);
            }
        }

    }
}
