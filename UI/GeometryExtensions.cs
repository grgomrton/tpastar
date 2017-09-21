using System;
using System.Drawing;
using System.Linq;
using TriangulatedPolygonAStar;
using TriangulatedPolygonAStar.BasicGeometry;

namespace TPAStarGUI
{
    public static class GeometryExtensions
    {
       
        public static PointF ToPointF(this IVector source)
        {
            float x = Convert.ToSingle(source.X);
            float y = Convert.ToSingle(source.Y);
            return new PointF(x, y);
        }

        public static Vector CalculateCentroid(this Triangle triangle)
        {
            var sum = triangle.A.Plus(triangle.B).Plus(triangle.C);
            return new Vector(sum.X / 3.0, sum.Y / 3.0);
        }    
    }
}