/**
 * Copyright 2017 Márton Gergó
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TriangulatedPolygonAStar.UI
{
    /// <summary>
    /// The visual representation of a line defined by line segments.
    /// </summary>
    public class PolyLine : IDrawable
    {
        private static Color LineColor = Color.Green;
        private static float LineWidth = 0.4f;
        private static Pen LinePen = new Pen(LineColor, LineWidth);
        private static Color TextColor = Color.Black;
        private static float FontSize = 1.2f;
        private static string CaptionFormat = "{0:0.00}";
        private static Brush CaptionBrush = new SolidBrush(TextColor);
        private static Font CaptionFont = new Font(FontFamily.GenericSansSerif, FontSize, FontStyle.Bold);
        private static SizeF CaptionTranslation = new SizeF(-2 * FontSize, 3 * FontSize);

        private IEnumerable<PointF> vertices;
        private bool displayLength;

        /// <inheritdoc />
        public PolyLine(IEnumerable<IVector> vertices): this(vertices, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolyLine"/> class
        /// which draws a line defined by line segments on a canvas.
        /// </summary>
        /// <param name="vertices">The set of positions which define the line</param>
        /// <param name="displayLength">Indicates whether the length of the line is calculated and displayed</param>
        public PolyLine(IEnumerable<IVector> vertices, bool displayLength) 
        {
            SetVertices(vertices);
            this.displayLength = displayLength;
        }

        /// <inheritdoc />
        public PointF BoundingBoxHigh { get; private set; }

        /// <inheritdoc />
        public PointF BoundingBoxLow { get; private set; }

        /// <summary>
        /// Sets the set of points which define the line.
        /// One-element sets are allowed.
        /// Empty sets are not allowed. 
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
            if (vertices.Count() > 1)
            {
                canvas.DrawLines(LinePen, vertices.ToArray());     
            }
            if (displayLength)
            {
                var length = vertices.Count() > 1
                    ? this.vertices.Zip(this.vertices.Skip(1),
                        (v1, v2) => Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2))).Sum()
                    : 0.0;
                var captionPosition = vertices.Last() + CaptionTranslation;
                canvas.DrawString(String.Format(CaptionFormat, length), CaptionFont, CaptionBrush, captionPosition);
            }
        }
    }
}