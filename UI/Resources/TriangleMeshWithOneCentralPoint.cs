using System.Collections.Generic;
using TriangulatedPolygonAStar.BasicGeometry;

namespace TriangulatedPolygonAStar.UI.Resources
{
    public static partial class TriangleMaps
    {
        public static IEnumerable<Triangle> CreateTriangleMapOfPolygonMeshWithOneCentralPoint()
        {
            var a = new Vector(0.0, 0.0);
            var b = new Vector(0.0, -2.0);
            var c = new Vector(1.0, -1.0);
            var d = new Vector(2.0, 0.0);
            var e = new Vector(1.0, 1.0);
            var f = new Vector(0.0, 2.0);
            var g = new Vector(-1.0, 1.0);
            var h = new Vector(-2.0, 0.0);
            var i = new Vector(-1.0, -1.0);
            var j = new Vector(2.0, 3.0);
            
            var t0 = new Triangle(a, b, c, 0);
            var t1 = new Triangle(a, c, d, 1);
            var t2 = new Triangle(a, d, e, 3);
            var t3 = new Triangle(a, e, f, 4);
            var t4 = new Triangle(a, e, f, 5);
            var t5 = new Triangle(a, f, g, 6);
            var t6 = new Triangle(a, g, h, 7);
            var t7 = new Triangle(a, h, i, 8);
            var t8 = new Triangle(a, i, b, 9);
            var t9= new Triangle(e, j, f, 10);
            var triangles = new[] {t0, t1, t2, t3, t4, t5, t6, t7, t8, t9};
            SetNeighboursForAll(triangles);
            
            return triangles;
        }
    }
}