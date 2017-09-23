using System;
using System.Drawing;
using TriangulatedPolygonAStar.BasicGeometry;

namespace TriangulatedPolygonAStar.UI
{
    public class DrawableTriangle : IDrawable
    {
        private static Color FillColor = Color.White;
        private static Color TraversionShade = Color.FromArgb(30, 30, 30);
        private static Color EdgeColor = Color.Gray;
        private static float EdgeWidth = 0.01f;
        private static Color TextColor = Color.Black;
        private static float FontSize = 0.12f;
        private static string MetaDataFormat = "{0} ({1}) {2:0.00}";
        
        private string id;
        private int traversionCount;
        private TriangleEvaluationResult lastEvaluationResult;
        private PointF[] points;
        private PointF centroid;
        private Pen edgePen;
        private Brush captionBrush;
        private Font captionFont;
        
        public DrawableTriangle(Triangle triangle, string id)
        {
            this.id = id;
            points = new PointF[3];
            points[0] = triangle.A.ToPointF();
            points[1] = triangle.B.ToPointF();
            points[2] = triangle.C.ToPointF();
            centroid = triangle.CalculateCentroid().ToPointF();
            edgePen = new Pen(EdgeColor, EdgeWidth);
            captionBrush = new SolidBrush(TextColor);
            captionFont = new Font("Arial", FontSize); // TODO os dependent
        }
        
        public void IncreaseTraversionCount(TriangleEvaluationResult metadata)
        {
            traversionCount++;
            lastEvaluationResult = metadata;
        }

        public void ResetMetaData()
        {
            traversionCount = 0;
            lastEvaluationResult = null;
        }
        
        private void DrawTriangle(Graphics canvas)
        {
            var r = Convert.ToInt32(Math.Max(FillColor.R - traversionCount * TraversionShade.R, 0));
            var g = Convert.ToInt32(Math.Max(FillColor.G - traversionCount * TraversionShade.G, 0));
            var b = Convert.ToInt32(Math.Max(FillColor.B - traversionCount * TraversionShade.B, 0));
            var fillColor = Color.FromArgb(r, g, b);
            
            var brush = new SolidBrush(fillColor);
            
            canvas.FillPolygon(brush, points);
            canvas.DrawPolygon(edgePen, points);
        }

        private void DrawMetaData(Graphics canvas)
        {
            var caption = String.Format(MetaDataFormat, id, traversionCount, lastEvaluationResult?.GMin); // TODO is this really useful information?
            canvas.DrawString(caption, captionFont, captionBrush, centroid);
        }

        public void Draw(Graphics canvas)
        {
            DrawTriangle(canvas);
            DrawMetaData(canvas);
        }
    }
}