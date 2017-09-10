using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using PathFinder.TPAStar;
using TriangulatedPolygonAStar.Geometry;

namespace TPAStarGUI
{
    public class TriangleIcon
    {
        private static Dictionary<string, Color> colors = new Dictionary<string, Color>
        {
            {"fill", Color.White},
            {"traversionShade", Color.FromArgb(50, 50, 50)},
            {"edge", Color.Gray},
            {"data", Color.Black}
        };

        private static Dictionary<string, float> widths = new Dictionary<string, float>
        {
            {"edge", 0.01f},
            {"fontSize", 0.12f}
        };

        private string id;
        
        private int traversionCount;
        private TriangleEvaluationResult lastEvaluationResult;

        private PointF[] points;
        private PointF centroid;

        public TriangleIcon(Triangle triangle, string id)
        {
            this.id = id;
            points = new PointF[3];
            points[0] = VectorConverter.ToPointF(triangle.A);
            points[1] = VectorConverter.ToPointF(triangle.A);
            points[2] = VectorConverter.ToPointF(triangle.A);

            centroid = triangle.Centroid.ToPointF();
        }
        
        internal void IncreaseTraversionCount()
        {
            IncreaseTraversionCount(null);
        }

        internal void IncreaseTraversionCount(TriangleEvaluationResult metadata)
        {
            traversionCount++;
            lastEvaluationResult = metadata;
        }

        internal void ResetMetaData()
        {
            traversionCount = 0;
            lastEvaluationResult = null;
        }
        
        internal void Draw(Graphics canvas, Dictionary<string, Color> colors_, Dictionary<string, float> widths_)
        {
            Color baseFillColor = colors["fill"];
            Color fillColor = baseFillColor;
            if (colors.ContainsKey("traversionShade"))
            {
                Color shadeColor = colors["traversionShade"];
                int r = Convert.ToInt32(Math.Max(baseFillColor.R - traversionCount * shadeColor.R, 0));
                int g = Convert.ToInt32(Math.Max(baseFillColor.G - traversionCount * shadeColor.G, 0));
                int b = Convert.ToInt32(Math.Max(baseFillColor.B - traversionCount * shadeColor.B, 0));
                fillColor = Color.FromArgb(r, g, b);
            }

            Brush brush = new SolidBrush(fillColor);
            Pen pen = new Pen(colors["edge"], widths["edge"]);

            canvas.FillPolygon(brush, points);
            canvas.DrawPolygon(pen, points);
        }

        internal void DrawMetaData(Graphics canvas, Dictionary<string, Color> colors_, Dictionary<string, float> widths_)
        {
            string formatString = "{0} [{1}] {2:0.00}";
            String label = String.Format(formatString, id, traversionCount, lastEvaluationResult?.GMin);
            canvas.DrawString(label, new Font("Arial", widths["fontSize"]), new SolidBrush(colors["data"]), centroid);

        }
    }
}