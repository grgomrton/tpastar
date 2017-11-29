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
    /// The visual representation of a triangle.
    /// </summary>
    public class DrawableTriangle : IDrawable
    {
        private static readonly Color FillColor;
        private static readonly Color TraversionShade;
        private static readonly Pen EdgePen;
        private readonly PointF[] corners;
        private readonly List<TriangleEvaluationResult> traversions;

        /// <summary>
        /// Initializes a new instance of <see cref="DrawableTriangle"/> which draws
        /// a triangle to the canvas.
        /// </summary>
        /// <param name="triangle">The triangle to draw</param>
        public DrawableTriangle(Triangle triangle)
        {
            corners = triangle.ToPointFs().ToArray();
            DisplayName = "t" + triangle.Id;
            BoundingBoxLow = new PointF(corners.Select(vertex => vertex.X).Min(), corners.Select(vertex => vertex.Y).Min());
            BoundingBoxHigh = new PointF(corners.Select(vertex => vertex.X).Max(), corners.Select(vertex => vertex.Y).Max());
            traversions = new List<TriangleEvaluationResult>();
        }

        /// <summary>
        /// The user friendly name of the triangle.
        /// </summary>
        public string DisplayName { get; private set; }
        
        /// <summary>
        /// The set of meta information about explorations of this triangle 
        /// since the last time the metadata was cleared.
        /// </summary>
        public IEnumerable<TriangleEvaluationResult> Traversions
        {
            get
            {
                return traversions;
            }
        }
        
        /// <inheritdoc />
        public PointF BoundingBoxHigh { get; private set; }

        /// <inheritdoc />
        public PointF BoundingBoxLow { get; private set; }
        
        /// <summary>
        /// Stores metadata about the traversal of this triangle.
        /// </summary>
        /// <param name="metadata">The result of the evaluation of the triangle</param>
        public void AddMetaData(TriangleEvaluationResult metadata)
        {
            traversions.Add(metadata);
        }

        /// <summary>
        /// Clears the stored information gathered during triangle map exploration.
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
        
        private static Color GetShade(int traversionCount)
        {
            var r = Convert.ToInt32(Math.Max(FillColor.R - traversionCount * TraversionShade.R, 0));
            var g = Convert.ToInt32(Math.Max(FillColor.G - traversionCount * TraversionShade.G, 0));
            var b = Convert.ToInt32(Math.Max(FillColor.B - traversionCount * TraversionShade.B, 0));
            return Color.FromArgb(r, g, b);
        }
        
        static DrawableTriangle()
        {
            FillColor = Color.White;
            TraversionShade = Color.FromArgb(30, 30, 30);
            var edgeWidth = 0.01f;
            EdgePen = new Pen(Color.Gray, edgeWidth);
        }
    }
}