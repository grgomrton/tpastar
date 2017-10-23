using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TriangulatedPolygonAStar.UI
{
    public class PolyLine : IDrawable
    {
        private static Color LineColor = Color.Green;
        private static Color TextColor = Color.Black;
        private static float LineWidth = 0.04f;
        private static float FontSize = 0.12f;
        private static string CaptionFormat = "{0:0.00}";
        private static PointF CaptionTranslation = new PointF(-2 * FontSize, -3 * FontSize);

        private IEnumerable<IVector> points;
        private Pen linePen;
        private Brush captionBrush;
        private Font captionFont;

        public PolyLine(IEnumerable<IVector> points)
        {
            this.points = points;
            linePen = new Pen(LineColor, LineWidth);
            captionBrush = new SolidBrush(TextColor);
            captionFont = new Font("Arial", FontSize, FontStyle.Bold);
        }
        
        public void Draw(Graphics canvas)
        {
            if (points.Count() > 1)
            {
                var vertices = points.Select(point => point.ToPointF()).ToArray();
                canvas.DrawLines(linePen, vertices);                
                
                var lastPoint = vertices.Last(); 
                var captionPosition = new PointF(lastPoint.X + CaptionTranslation.X, lastPoint.Y + CaptionTranslation.Y);
            
                var lineLength = points.Zip(points.Skip(1), (v1, v2) => Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2))).Sum();
            
                canvas.DrawString(String.Format(CaptionFormat, lineLength), captionFont, captionBrush, captionPosition);
            }
        }

        public void SetPoints(IEnumerable<IVector> points)
        {
            this.points = points;
        }
    }
}