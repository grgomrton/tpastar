﻿/**
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
        private static float EdgeWidth = 0.1f;
        private static Pen EdgePen = new Pen(EdgeColor, EdgeWidth);

        private readonly string displayName;
        private readonly PointF[] corners;
        private List<TriangleEvaluationResult> traversions;
        
        /// <summary>
        /// Initializes a new instance of <see cref="DrawableTriangle"/> which draws
        /// a <see cref="Triangle"/> to the canvas.
        /// </summary>
        /// <param name="triangle">The triangle to draw</param>
        public DrawableTriangle(Triangle triangle)
        {
            displayName = "t" + triangle.Id;
            corners = triangle.ToPointFs().ToArray();
            BoundingBoxLow = new PointF(corners.Select(vertex => vertex.X).Min(), corners.Select(vertex => vertex.Y).Min());
            BoundingBoxHigh = new PointF(corners.Select(vertex => vertex.X).Max(), corners.Select(vertex => vertex.Y).Max());
            traversions = new List<TriangleEvaluationResult>();
        }

        /// <summary>
        /// Increases the amount of time this triangle have been stepped into during
        /// exploring the triangle graph.
        /// </summary>
        /// <param name="metadata">The result of the evaluation of the triangle</param>
        public void AddMetaData(TriangleEvaluationResult metadata)
        {
            traversions.Add(metadata);
        }

        /// <summary>
        /// Clears the stored information which was gathered during triangle map exploration.
        /// </summary>
        public void ClearMetaData()
        {
            traversions.Clear();
        }

        /// <inheritdoc />
        public void Draw(Graphics canvas)
        {
            var fillBrush = new SolidBrush(GetShade(traversions.Count));
            canvas.FillPolygon(fillBrush, corners);
            canvas.DrawPolygon(EdgePen, corners);
        }

        /// <inheritdoc />
        public PointF BoundingBoxHigh { get; }

        /// <inheritdoc />
        public PointF BoundingBoxLow { get; }

        /// <summary>
        /// The user friendly name of the triangle which constains its' id.
        /// </summary>
        public string DisplayName { get { return displayName; } }

        /// <summary>
        /// The set of meta information about explorations of this triangle 
        /// since the metadata was cleared.
        /// </summary>
        public IEnumerable<TriangleEvaluationResult> Traversions { get { return traversions; } }
        
        private static Color GetShade(int traversionCount)
        {
            var r = Convert.ToInt32(Math.Max(FillColor.R - traversionCount * TraversionShade.R, 0));
            var g = Convert.ToInt32(Math.Max(FillColor.G - traversionCount * TraversionShade.G, 0));
            var b = Convert.ToInt32(Math.Max(FillColor.B - traversionCount * TraversionShade.B, 0));
            return Color.FromArgb(r, g, b);
        }
    }
}