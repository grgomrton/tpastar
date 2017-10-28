using System;

namespace TriangulatedPolygonAStar.BasicGeometry
{
    /// <inheritdoc />
    public class Vector : IVector
    {
        private readonly double x;
        private readonly double y;

        /// <summary>
        /// Initializes a new instance of <see cref="Vector"/> class by its two coordinates.
        /// </summary>
        /// <param name="x">The value of the horizontal component of the vector</param>
        /// <param name="y">The value of the vertical component of the vector</param>
        public Vector(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        /// <inheritdoc />
        public double X
        {
            get { return x; }
        }

        /// <inheritdoc />
        public double Y
        {
            get { return y; }
        }

        /// <inheritdoc />
        public IVector Plus(IVector other)
        {
            CheckForNullArgument(other, nameof(other));

            return new Vector(X + other.X, Y + other.Y);
        }

        /// <inheritdoc />
        public IVector Minus(IVector other)
        {
            CheckForNullArgument(other, nameof(other));

            return new Vector(X - other.X, Y - other.Y);
        }

        /// <inheritdoc />
        public IVector Times(double scalar)
        {
            return new Vector(scalar * X, scalar * Y);
        }

        /// <inheritdoc />
        public double DistanceFrom(IVector other)
        {
            CheckForNullArgument(other, nameof(other));

            return other.Minus(this).Length();
        }

        /// <inheritdoc />
        public bool IsInCounterClockWiseDirectionFrom(IVector other)
        {
            CheckForNullArgument(other, nameof(other));

            return ZComponentOfCrossProductWith(other) <= 0.0;
        }

        /// <inheritdoc />
        public bool IsInClockWiseDirectionFrom(IVector other)
        {
            CheckForNullArgument(other, nameof(other));

            return ZComponentOfCrossProductWith(other) >= 0.0;
        }

        /// <summary>
        /// Determines whether the specified object represents the same point in the space as this vector.
        /// Two vectors are considered to represent the same point if the cartesian distance between them is smaller 
        /// than the tolerance set in <see cref="VectorEqualityCheck.Tolerance"/>.
        /// </summary>
        /// <param name="other">The other object to compare this vector with</param>
        /// <returns>true if the two object represent the same point, otherwise false</returns>
        public override bool Equals(object other)
        {
            Vector otherVector = other as Vector;
            if (otherVector != null)
            {
                return otherVector.DistanceFrom(this) < VectorEqualityCheck.Tolerance;
            }
            return false;
        }

        /// <summary>
        /// Converts this vector to a textual representation with the coordinates rounded by two digits. 
        /// </summary>
        /// <returns>The coordinates of this vector as text</returns>
        public override string ToString()
        {
            return String.Format("({0:0.00}, {1:0.00})", X, Y);
        }

        private double ZComponentOfCrossProductWith(IVector other)
        {
            return X * other.Y - Y * other.X;
        }

        private static void CheckForNullArgument(object value, string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }
    }

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
        /// Returns the magnitude of the vector.
        /// </summary>
        /// <param name="a">The vector to calculate the magnitude of</param>
        /// <returns>The distance between the two endpoints of this vector</returns>
        public static double Length(this IVector a)
        {
            return Math.Sqrt(Math.Pow(a.X, 2) + Math.Pow(a.Y, 2));
        }
    }
}