using System;

namespace TriangulatedPolygonAStar.BasicGeometry
{
    /// <summary>
    /// Extension methods for <see cref="IVector"/> instances. 
    /// </summary>
    public static class VectorExtensions
    {
        /// <summary>
        /// Returns the dot product of this vector with the specified one.
        /// </summary>
        /// <param name="a">The vector on the left side of the operation</param>
        /// <param name="b">The vector on the right side of the operation</param>
        /// <returns></returns>
        public static double DotProduct(this IVector a, IVector b)
        {
            return a.X * b.X +
                   a.Y * b.Y;
        }

        /// <summary>
        /// Returns the distance between the two endpoints of this vector.
        /// </summary>
        /// <param name="a">The vector to calculate the length of</param>
        /// <returns>The length of this vector</returns>
        public static double Length(this IVector a)
        {
            return Math.Sqrt(Math.Pow(a.X, 2) + Math.Pow(a.Y, 2));
        }
    }
}