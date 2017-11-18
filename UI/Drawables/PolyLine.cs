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
        private static readonly Pen LinePen;
        private static readonly Brush CaptionBrush;
        private static readonly Font CaptionFont;
        private static readonly SizeF CaptionTranslation;
        private static readonly string CaptionFormat;
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
            BoundingBoxLow = new PointF(this.vertices.Select(vertex => vertex.X).Min(),
                this.vertices.Select(vertex => vertex.Y).Min());
            BoundingBoxHigh = new PointF(this.vertices.Select(vertex => vertex.X).Max(),
                this.vertices.Select(vertex => vertex.Y).Max());
        }

        /// <inheritdoc />
        public void Draw(Graphics canvas)
        {
            var length = 0.0;
            if (vertices.Count() > 1)
            {
                length = this.vertices.Zip(this.vertices.Skip(1),
                    (v1, v2) => Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2))).Sum();
                canvas.DrawLines(LinePen, vertices.ToArray());
            }

            var captionPosition = vertices.Last() + CaptionTranslation;
            canvas.DrawString(String.Format(CaptionFormat, length), CaptionFont, CaptionBrush, captionPosition);
        }

        static PolyLine()
        {
            var width = 0.04f;
            LinePen = new Pen(Color.Green, width);
            CaptionBrush = Brushes.Black;
            var fontSize = 0.12f;
            CaptionFont = new Font(FontFamily.GenericSansSerif, fontSize, FontStyle.Bold);
            CaptionTranslation = new SizeF(-2 * fontSize, -3 * fontSize);
            CaptionFormat = "{0:0.00}";
        }
    }
}