using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommonTools.Geometry;
using PathFinder.Funnel;

namespace TPAStarGUI
{
    public partial class FunnelDemonstration : Form
    {
        Edge[] internalEdges;
        Vector3 start, goal;
        List<Vector3> path;
        double magnify = 40;

        private void FunnelDemonstration_Load(object sender, EventArgs e)
        {
            goal = new Vector3(1, 5, 0); 
            //start = new Vector3(1, 5, 0);
            start = new Vector3(8, 3, 0); 
            //goal = new Vector3(8, 3, 0);
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

            Edge e0 = new Edge(cl0, cr0);
            Edge e1 = new Edge(cl1, cr0);
            Edge e2 = new Edge(cl1, cr1);
            Edge e3 = new Edge(cl2, cr1);
            Edge e4 = new Edge(cr2, cl2);
            Edge e5 = new Edge(cr3, cl2);
            Edge e6 = new Edge(cl3, cr3);
            Edge e7 = new Edge(cr3, cl4);
            Edge e8 = new Edge(cl4, cr4);

            internalEdges = new Edge[] { e0, e1, e2, e3, e4, e5, e6, e7, e8 };
            internalEdges = new Edge[] { e8, e7, e6, e5, e4, e3, e2, e1, e0 };

            FindPath();
        }


        private void DrawEdges(Graphics canvas, Edge[] edges, Pen pen, double magnify)
        {
            for (int i = 0; i < edges.Length; i++)
            {
                PointF a = (magnify * edges[i].A).ToPointF();
                PointF b = (magnify * edges[i].B).ToPointF();

                canvas.DrawLine(pen, a, b);

                DrawDots(canvas, new Vector3[] { edges[i].A, edges[i].B }, Brushes.Aqua, magnify);
            }
        }

        private void FindPath()
        {
            path = FunnelAlgorithm.FindPath(start, goal, internalEdges).ToList();
        }

        // TODO: drawing to the triangle, curve, vector nodes...
        private void DrawVectors(Graphics canvas, LinkedList<Vector3> vectors, Pen pen, double magnify)
        {
            LinkedList<Vector3>.Enumerator enumerator = vectors.GetEnumerator();

            PointF[] points = new PointF[vectors.Count];

            int i = 0;
            while (enumerator.MoveNext())
            {
                Vector3 current = enumerator.Current;

                float x = Convert.ToSingle(current.X * magnify);
                float y = Convert.ToSingle(current.Y * magnify);

                points[i] = new PointF(x, y);

                i++;
            }

            canvas.DrawLines(pen, points);

        }

        private void DrawDots(Graphics canvas, Vector3[] vectors, Brush brush, double magnify)
        {
            for (int i = 0; i < vectors.Length; i++)
            {
                double dotRadius = 0.075;
                float dotDiameter = Convert.ToSingle(2 * dotRadius * magnify);
                PointF p = ((vectors[i] - new Vector3(dotRadius, dotRadius, 0)) * magnify).ToPointF();
                SizeF rect = new SizeF(dotDiameter, dotDiameter);

                canvas.FillEllipse(brush, new RectangleF(p, rect));
            }
        }

        public FunnelDemonstration()
        {
            InitializeComponent();
        }

        private void displayBox_MouseMove(object sender, MouseEventArgs e)
        {
            double x = e.X / magnify;
            double y = e.Y / magnify;
            this.Text = String.Format("Demonstration of Funnel algorithm [{0:00};{1:00}]", x, y);
            if (e.Button == MouseButtons.Left)
            {
                start = new Vector3(x, y, 0);
            }
            else if (e.Button == MouseButtons.Right)
            {
                goal = new Vector3(x, y, 0);
            }
            FindPath();
            displayBox.Invalidate();
        }

        private void displayBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            DrawDots(canvas, new Vector3[] { start }, Brushes.Blue, magnify);
            DrawDots(canvas, new Vector3[] { goal }, Brushes.Green, magnify);
            DrawEdges(canvas, internalEdges, Pens.Blue, magnify);

            if (path.Count > 1)
            {
                //DrawVectors(canvas, path, Pens.Green, magnify);
            }
        }

    }
}
