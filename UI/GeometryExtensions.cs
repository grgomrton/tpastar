using System;
using System.Drawing;
using TriangulatedPolygonAStar;
using TriangulatedPolygonAStar.BasicGeometry;

namespace TPAStarGUI
{
    public static class GeometryExtensions
    {
        /// <summary>
        /// Returns the minimal distance of the triangle from a point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public static double MinDistanceFromOuterPoint(this Triangle triangle, IVector point)
        {
            Vector a = new Vector(triangle.A.X, triangle.A.Y);
            Vector b = new Vector(triangle.B.X, triangle.B.Y);
            Vector c = new Vector(triangle.C.X, triangle.C.Y);
            Edge e1 = new Edge(a, b);
            Edge e2 = new Edge(a, c);
            Edge e3 = new Edge(b, c);
            double dist1 = e1.DistanceFromPoint(point);
            double dist2 = e2.DistanceFromPoint(point);
            double dist3 = e3.DistanceFromPoint(point);
            return Math.Min(dist1, Math.Min(dist2, dist3));
        }
        
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