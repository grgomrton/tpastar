using System;
using System.Drawing;
using System.Linq;
using TriangulatedPolygonAStar.BasicGeometry;

namespace TriangulatedPolygonAStar.UI
{
    /// <summary>
    /// The visual representation of a <see cref="Triangle"/>.
    /// </summary>
    public class DrawableTriangle : IDrawable
    {
        private static Color FillColor = Color.White;
        private static Color TraversionShade = Color.FromArgb(30, 30, 30);
        private static Color EdgeColor = Color.Gray;
        private static float EdgeWidth = 0.01f;
        private static Pen EdgePen = new Pen(EdgeColor, EdgeWidth); 
        private static Color TextColor = Color.Black;
        private static float FontSize = 0.12f;
        private static Brush CaptionBrush = new SolidBrush(TextColor);
        private static Font CaptionFont = new Font(FontFamily.GenericSansSerif, FontSize);
        private static IVector CaptionTranslation = new Vector(-0.8, -0.8);
        
        private readonly string displayName;
        private readonly PointF[] corners;
        private readonly PointF captionPosition;
        private int traversionCount;
        private TriangleEvaluationResult lastEvaluationResult;
        
        /// <summary>
        /// Initializes a new instance of <see cref="DrawableTriangle"/> which draws
        /// a <see cref="Triangle"/> to the canvas.
        /// </summary>
        /// <param name="triangle">The triangle to draw</param>
        public DrawableTriangle(Triangle triangle)
        {
            this.displayName = "t" + triangle.Id;
            corners = triangle.ToPointFs().ToArray();
            BoundingBoxLow = new PointF(corners.Select(vertex => vertex.X).Min(), corners.Select(vertex => vertex.Y).Min());
            BoundingBoxHigh = new PointF(corners.Select(vertex => vertex.X).Max(), corners.Select(vertex => vertex.Y).Max());
            captionPosition = triangle.GetCentroid().Plus(CaptionTranslation).ToPointF();
        }

        /// <summary>
        /// Increases the amount of time this triangle have been stepped into during
        /// exploring the triangle graph.
        /// </summary>
        /// <param name="metadata">The result of the evaluation of the triangle</param>
        public void IncreaseTraversionCount(TriangleEvaluationResult metadata)
        {
            traversionCount++;
            lastEvaluationResult = metadata;
        }

        /// <summary>
        /// Clears the stored information which was gathered during triangle map exploration.
        /// </summary>
        public void ResetMetaData()
        {
            traversionCount = 0;
            lastEvaluationResult = null;
        }

        /// <inheritdoc />
        public void Draw(Graphics canvas)
        {
            DrawTriangle(canvas);
            DrawMetaData(canvas);
        }

        /// <inheritdoc />
        public PointF BoundingBoxHigh { get; }

        /// <inheritdoc />
        public PointF BoundingBoxLow { get; }

        private void DrawTriangle(Graphics canvas)
        {
            var fillBrush = new SolidBrush(GetShade(traversionCount));
            canvas.FillPolygon(fillBrush, corners);
            canvas.DrawPolygon(EdgePen, corners);
        }

        private void DrawMetaData(Graphics canvas)
        {
            var caption = String.Format("{0} ({1}) gMin: {2:0.00}, f: {3:0.00}", 
                displayName, 
                traversionCount, 
                lastEvaluationResult?.ShortestPathToEdgeLength,
                lastEvaluationResult?.EstimatedMinimalCost);
            canvas.DrawString(caption, CaptionFont, CaptionBrush, captionPosition);
        }
        
        private static Color GetShade(int traversionCount)
        {
            var r = Convert.ToInt32(Math.Max(FillColor.R - traversionCount * TraversionShade.R, 0));
            var g = Convert.ToInt32(Math.Max(FillColor.G - traversionCount * TraversionShade.G, 0));
            var b = Convert.ToInt32(Math.Max(FillColor.B - traversionCount * TraversionShade.B, 0));
            return Color.FromArgb(r, g, b);
        }
    }
}