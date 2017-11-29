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
using System.Globalization;
using System.Linq;
using TriangulatedPolygonAStar.BasicGeometry;

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
        private static readonly IVector CaptionTranslation;
        private static readonly string CaptionFormat;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolyLine"/> class
        /// which draws a line defined by line segments on a canvas.
        /// </summary>
        /// <param name="vertices">The set of vertices that define the line</param>
        public PolyLine(IEnumerable<IVector> vertices)
        {
            SetVertices(vertices);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolyLine"/> class
        /// that contains no points. 
        /// </summary>
        public PolyLine() : this(Enumerable.Empty<IVector>())
        {
        }

        /// <inheritdoc />
        public PointF BoundingBoxHigh { get; private set; }

        /// <inheritdoc />
        public PointF BoundingBoxLow { get; private set; }
        
        /// <summary>
        /// The vertices of the line.
        /// </summary>
        public IEnumerable<IVector> Vertices { get; private set; }

        /// <summary>
        /// Sets the set of points which define the line.
        /// Both one-element and empty sets are allowed.
        /// In case of an empty set, the bounding boxes will remain unchanged and nothing will be drawn.
        /// </summary>
        /// <param name="vertices">The set of points of the line</param>
        public void SetVertices(IEnumerable<IVector> vertices)
        {
            Vertices = vertices;
            if (Vertices.Any())
            {
                var minX = Vertices.Select(point => point.X).Min();
                var minY = Vertices.Select(point => point.Y).Min();
                var maxX = Vertices.Select(point => point.X).Max();
                var maxY = Vertices.Select(point => point.Y).Max();
                BoundingBoxLow = new PointF(Convert.ToSingle(minX), Convert.ToSingle(minY));
                BoundingBoxHigh = new PointF(Convert.ToSingle(maxX), Convert.ToSingle(maxY));
            }
        }

        /// <inheritdoc />
        public void Draw(Graphics canvas)
        {
            if (Vertices.Any())
            {
                var length = 0.0;
                if (Vertices.Count() > 1)
                {
                    length = Vertices.Zip(Vertices.Skip(1),
                        (v1, v2) => Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2))).Sum();
                    canvas.DrawLines(LinePen, Vertices.Select(point => point.ToPointF()).ToArray());
                }
                var captionPosition = Vertices.Last().Plus(CaptionTranslation).ToPointF();
                canvas.DrawString(String.Format(CultureInfo.InvariantCulture, CaptionFormat, length), CaptionFont, CaptionBrush, captionPosition);                
            }
        }

        static PolyLine()
        {
            var width = 0.03f;
            LinePen = new Pen(Color.Green, width);
            CaptionBrush = Brushes.Black;
            var fontSize = 0.12f;
            CaptionFont = new Font(FontFamily.GenericSansSerif, fontSize, FontStyle.Bold);
            CaptionTranslation = new Vector(-2 * fontSize, -3 * fontSize);
            CaptionFormat = "{0:0.00}";
        }
    }
}