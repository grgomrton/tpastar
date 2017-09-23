using System.Collections.Generic;
using TriangulatedPolygonAStar.BasicGeometry;

namespace TriangulatedPolygonAStar.UI.Resources
{
    public static partial class TriangleMaps
    {
        private static IEnumerable<Triangle> GetTrianglesOfPolygonWithOneHole()
        {
            var cr0 = new Vector(2, 4);
            var cr1 = new Vector(2, 3);
            var cr2 = new Vector(3, 2);
            var cr3 = new Vector(5, 3);
            var cr4 = new Vector(7, 2);
            var cl0 = new Vector(0, 4);
            var cl1 = new Vector(0, 3);
            var cl2 = new Vector(3, 1);
            var cl3 = new Vector(5, 2.5);
            var cl4 = new Vector(6, 1);
            var cp0 = new Vector(1, 7);
            var cp1 = new Vector(6.5, 0);

            Triangle t0 = new Triangle(cp0, cl0, cr0);
            Triangle t1 = new Triangle(cl0, cr0, cl1);
            Triangle t2 = new Triangle(cl1, cr0, cr1);
            Triangle t3 = new Triangle(cl1, cr1, cl2);
            Triangle t4 = new Triangle(cr1, cl2, cr2);
            Triangle t5 = new Triangle(cr2, cl2, cr3);
            Triangle t6 = new Triangle(cl3, cl2, cr3);
            Triangle t7 = new Triangle(cl3, cl4, cr3);
            Triangle t8 = new Triangle(cr4, cl4, cr3);
            Triangle t9 = new Triangle(cr4, cl4, cp1);
            Triangle t10 = new Triangle(cr0, cp0, cr3);
            Triangle t11 = new Triangle(cr4, cp0, cr3);

            return new [] {t0, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11};
        }
    }
}