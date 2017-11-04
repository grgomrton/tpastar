using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TriangulatedPolygonAStar.BasicGeometry;

namespace TriangulatedPolygonAStar.UI
{
    /// <summary>
    /// The visual representation of a line defined by line segments.
    /// </summary>
    public class PolyLine : IDrawable
    {
        private static Color LineColor = Color.Green;
        private static Color TextColor = Color.Black;
        private static float LineWidth = 0.04f;
        private static float FontSize = 0.12f;
        private static string CaptionFormat = "{0:0.00}";
        private static IVector CaptionTranslation = new Vector(-2 * FontSize, -3 * FontSize);

        private IEnumerable<IVector> vertices;
        private Pen linePen;
        private Brush captionBrush;
        private Font captionFont;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolyLine"/> class
        /// which draws a line defined by line segments on a canvas.
        /// </summary>
        /// <param name="vertices">The set of positions which define the line</param>
        public PolyLine(IEnumerable<IVector> vertices)
        {
            this.vertices = vertices;
            linePen = new Pen(LineColor, LineWidth);
            captionBrush = new SolidBrush(TextColor);
            captionFont = new Font(FontFamily.GenericSansSerif, FontSize, FontStyle.Bold);
        }

        /// <inheritdoc />
        public void Draw(Graphics canvas)
        {
            if (vertices.Count() > 1)
            {
                var drawableVertices = vertices.Select(point => point.ToPointF()).ToArray();
                var captionPosition = vertices.Last().Plus(CaptionTranslation).ToPointF();
                var lineLength = this.vertices.Zip(this.vertices.Skip(1), (v1, v2) => Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2))).Sum();
                
                canvas.DrawLines(linePen, drawableVertices);                
                canvas.DrawString(String.Format(CaptionFormat, lineLength), captionFont, captionBrush, captionPosition);
            }
        }

        /// <summary>
        /// Sets the set of points which define the line. 
        /// Empty and one-element sets are allowed but they will not be drawn.
        /// </summary>
        /// <param name="vertices">The set of points of the line</param>
        public void SetVertices(IEnumerable<IVector> vertices)
        {
            this.vertices = vertices;
        }
    }
}