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

using System.Collections.Generic;
using TriangulatedPolygonAStar.BasicGeometry;

namespace TriangulatedPolygonAStar.UI.Resources
{
    public static partial class TriangleMaps
    {
        /// <summary>
        /// Builds a map of a triangulated polygon with one polygon hole.
        /// </summary>
        public static IEnumerable<Triangle> CreateTriangleMapOfPolygonWithOnePolygonHole()
        {
            var a = new Vector(2.0, 4.0);
            var b = new Vector(2.0, 3.0);
            var c = new Vector(3.0, 2.0);
            var d = new Vector(5.0, 3.0);
            var e = new Vector(7.0, 2.0);
            var f = new Vector(0.0, 4.0);
            var g = new Vector(0.0, 3.0);
            var h = new Vector(3.0, 1.0);
            var i = new Vector(5.0, 2.5);
            var j = new Vector(6.0, 1.0);
            var k = new Vector(1.0, 7.0);
            var l = new Vector(6.5, 0.0);

            var t0 = new Triangle(k, f, a, 0);
            var t1 = new Triangle(f, a, g, 1);
            var t2 = new Triangle(g, a, b, 2);
            var t3 = new Triangle(g, b, h, 3);
            var t4 = new Triangle(b, h, c, 4);
            var t5 = new Triangle(c, h, d, 5);
            var t6 = new Triangle(i, h, d, 6);
            var t7 = new Triangle(i, j, d, 7);
            var t8 = new Triangle(e, j, d, 8);
            var t9 = new Triangle(e, j, l, 9);
            var t10 = new Triangle(a, k, d, 10);
            var t11 = new Triangle(e, k, d, 11);
            var triangles = new[] {t0, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11};
            SetNeighboursForAll(triangles);

            return triangles;
        }
    }
}