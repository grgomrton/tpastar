namespace TriangulatedPolygonAStar.BasicGeometry
{
    internal static class VectorEqualityCheck
    {
        /// <summary>
        /// Defines the higher bound for the cartesian distance between two
        /// points defined to be equal. If the distance is exactly the same
        /// size as the specified value, then the equality check should not pass.
        /// </summary>
        internal static double Tolerance = 0.00001;
    }
}