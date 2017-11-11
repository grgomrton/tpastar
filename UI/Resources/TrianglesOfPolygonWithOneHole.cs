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
        private static IEnumerable<Triangle> GetTrianglesOfPolygonWithOneHole()
        {
            var p0 = new Vector(2.0, 4.0);
            var p1 = new Vector(2.0, 3.0);
            var p2 = new Vector(3.0, 2.0);
            var p3 = new Vector(5.0, 3.0);
            var p4 = new Vector(7.0, 2.0);
            var p5 = new Vector(0.0, 4.0);
            var p6 = new Vector(0.0, 3.0);
            var p7 = new Vector(3.0, 1.0);
            var p8 = new Vector(5.0, 2.5);
            var p9 = new Vector(6.0, 1.0);
            var p10 = new Vector(1.0, 7.0);
            var p11 = new Vector(6.5, 0.0);

            Triangle t0 = new Triangle(p10, p5, p0, 0);
            Triangle t1 = new Triangle(p5, p0, p6, 1);
            Triangle t2 = new Triangle(p6, p0, p1, 2);
            Triangle t3 = new Triangle(p6, p1, p7, 3);
            Triangle t4 = new Triangle(p1, p7, p2, 4);
            Triangle t5 = new Triangle(p2, p7, p3, 5);
            Triangle t6 = new Triangle(p8, p7, p3, 6);
            Triangle t7 = new Triangle(p8, p9, p3, 7);
            Triangle t8 = new Triangle(p4, p9, p3, 8);
            Triangle t9 = new Triangle(p4, p9, p11, 9);
            Triangle t10 = new Triangle(p0, p10, p3, 10);
            Triangle t11 = new Triangle(p4, p10, p3, 11);

            return new [] {t0, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11};
        }
    }
}