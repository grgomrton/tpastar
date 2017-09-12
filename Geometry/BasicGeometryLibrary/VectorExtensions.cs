namespace TriangulatedPolygonAStar.BasicGeometry
{
    public static class VectorExtensions
    {
        public static double DotProduct(this IVector a, IVector b)
        {
            return a.X * b.X +
                   a.Y * b.Y;
        }
    }
}