namespace TriangulatedPolygonAStar.BasicGeometry
{
    internal static class VectorEqualityCheck
    {
        /// <summary>
        /// Defines the higher bound of the cartesian distance between two
        /// equal points. If the distance is exactly the same as the 
        /// specified value, then the equality check should not pass.
        /// </summary>
        internal static double Tolerance = 0.00001;
    }
}