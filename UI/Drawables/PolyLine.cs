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
        private static float LineWidth = 0.04f;
        private static Pen LinePen = new Pen(LineColor, LineWidth);
        private static Color TextColor = Color.Black;
        private static float FontSize = 0.12f;
        private static string CaptionFormat = "{0:0.00}";
        private static Brush CaptionBrush = new SolidBrush(TextColor);
        private static Font CaptionFont = new Font(FontFamily.GenericSansSerif, FontSize, FontStyle.Bold);
        private static SizeF CaptionTranslation = new SizeF(-2 * FontSize, -3 * FontSize);

        private IEnumerable<PointF> vertices;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolyLine"/> class
        /// which draws a line defined by line segments on a canvas.
        /// </summary>
        /// <param name="vertices">The set of positions which define the line</param>
        public PolyLine(IEnumerable<IVector> vertices)
        {
            SetVertices(vertices);
        }

        /// <inheritdoc />
        public PointF BoundingBoxHigh { get; private set; }

        /// <inheritdoc />
        public PointF BoundingBoxLow { get; private set; }

        /// <summary>
        /// Sets the set of points which define the line.
        /// Empty sets are not allowed. One-elements sets are allowed.
        /// </summary>
        /// <param name="vertices">The set of points of the line</param>
        public void SetVertices(IEnumerable<IVector> vertices)
        {
            if (!vertices.Any())
            {
                throw new ArgumentOutOfRangeException("Empty sets are not allowed", nameof(vertices));
            }
            this.vertices = vertices.Select(point => point.ToPointF());
            BoundingBoxLow = new PointF(this.vertices.Select(vertex => vertex.X).Min(), this.vertices.Select(vertex => vertex.Y).Min());
            BoundingBoxHigh = new PointF(this.vertices.Select(vertex => vertex.X).Max(), this.vertices.Select(vertex => vertex.Y).Max());
        }
        
        /// <inheritdoc />
        public void Draw(Graphics canvas)
        {
            var length = 0.0;
            if (vertices.Count() > 1)
            {
                length = this.vertices.Zip(this.vertices.Skip(1), (v1, v2) => Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2))).Sum();
                canvas.DrawLines(LinePen, vertices.ToArray());     
            }
            
            var captionPosition = vertices.Last() + CaptionTranslation;
            canvas.DrawString(String.Format(CaptionFormat, length), CaptionFont, CaptionBrush, captionPosition);
        }
    }
}