using System.Collections.Generic;
using TriangulatedPolygonAStar.BasicGeometry;

namespace TriangulatedPolygonAStar.UI.Resources
{
    public static partial class TriangleMaps
    {
        private static IEnumerable<Triangle> GetTrianglesOfPolygonWithTwoHoles()
        {
            var triangleSet = new List<Triangle>();
            
            var cr0 = new Vector(2, 4);
            var cr1 = new Vector(2, 3);
            var cr2 = new Vector(3, 2);
            var cr3 = new Vector(5, 3);
            var cr4 = new Vector(7, 2);
            var cl0 = new Vector(0, 4);
            var cl1 = new Vector(0, 3);
            var cl2 = new Vector(4, 1.5);
            var cl3 = new Vector(5, 2.5);
            var cl4 = new Vector(6, 1);
            var cl5 = new Vector(3, 1);    
            var cp0 = new Vector(1, 7);
            var cp1 = new Vector(6.5, 0);
            
            Triangle t0 = new Triangle(cp0, cl0, cr0, 0);
            triangleSet.Add(t0);
            Triangle t1 = new Triangle(cl0, cr0, cl1, 1);
            triangleSet.Add(t1);
            Triangle t2 = new Triangle(cl1, cr0, cr1, 2);
            triangleSet.Add(t2);
            Triangle t3 = new Triangle(cl1, cr1, cl5, 3);
            triangleSet.Add(t3);
            Triangle t4 = new Triangle(cr1, cl5, cr2, 4);
            triangleSet.Add(t4);
            Triangle t5 = new Triangle(cr2, cl2, cr3, 5);
            triangleSet.Add(t5);
            Triangle t6 = new Triangle(cl3, cl2, cr3, 6);
            triangleSet.Add(t6);
            Triangle t7 = new Triangle(cl3, cl4, cr3, 7);
            triangleSet.Add(t7);
            Triangle t8 = new Triangle(cr4, cl4, cr3, 8);
            triangleSet.Add(t8);
            Triangle t9 = new Triangle(cr4, cl4, cp1, 9);
            triangleSet.Add(t9);
            Triangle t10 = new Triangle(cr0, cp0, cr3, 10);
            triangleSet.Add(t10);
            Triangle t11 = new Triangle(cr4, cp0, cr3, 11);
            triangleSet.Add(t11);
            Triangle t12 = new Triangle(cl5, cr2, cl2, 12);
            triangleSet.Add(t12);
            Triangle t13 = new Triangle(cl2, cl5, cp1, 13);
            triangleSet.Add(t13);
            Triangle t14 = new Triangle(cl4, cl2, cp1, 14);
            triangleSet.Add(t14);

            return triangleSet;
        }
    }
}