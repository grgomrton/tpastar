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
using TriangulatedPolygonAStar.BasicGeometry;

namespace TriangulatedPolygonAStar.UI
{
    /// <summary>
    /// Extension methods for the <see cref="TriangulatedPolygonAStar.BasicGeometry"/> library. 
    /// </summary>
    public static class GeometryExtensions
    {
        /// <summary>
        /// Converts the specified <see cref="IVector"/> instance to a floating point representation.
        /// </summary>
        /// <param name="source">The vector to convert</param>
        /// <returns>The vector in floating point representation</returns>
        public static PointF ToPointF(this IVector source)
        {
            float x = Convert.ToSingle(source.X);
            float y = Convert.ToSingle(source.Y);
            return new PointF(x, y);
        }
        
        /// <summary>
        /// Converts the specified <see cref="Triangle"/> instance to a floating point representation.
        /// </summary>
        /// <param name="source">The triangle to convert</param>
        /// <returns>The triangle in floating point representation</returns>        
        public static IEnumerable<PointF> ToPointFs(this Triangle source)
        {
            yield return source.A.ToPointF();
            yield return source.B.ToPointF();
            yield return source.C.ToPointF();
        }
        
        /// <summary>
        /// Calculates the centroid of the specified triangle.
        /// </summary>
        /// <param name="triangle">The triangle to calculate the centroid of</param>
        /// <returns>The centroid of the triangle</returns>
        public static Vector GetCentroid(this Triangle triangle)
        {
            var sum = triangle.A.Plus(triangle.B).Plus(triangle.C);
            return new Vector(sum.X / 3.0, sum.Y / 3.0);
        }    
    }
}