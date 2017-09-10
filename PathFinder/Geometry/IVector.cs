namespace TriangulatedPolygonAStar.Geometry
{
    public interface IVector
    {
        /// <summary>
        /// Returns the cartesian distance from the specified point.
        /// </summary>
        /// <param name="other">The point to measure the distance from</param>
        /// <returns>The cartesian distance between the two points.</returns>
        double Distance(IVector other);

        /// <summary>
        /// Returns the result of adding the specified vector to the current one. 
        /// </summary>
        /// <param name="other">The vector to add</param>
        /// <returns>The result of the addition.</returns>
        IVector Plus(IVector other);
        
        /// <summary>
        /// Returns the result of subtracting the specified vector from the current one. 
        /// </summary>
        /// <param name="other">The vector to subtract to</param>
        /// <returns>The result of the subtraction.</returns>
        IVector Minus(IVector other);

        /// <summary>
        /// Returns the result of multiplying this vector with a floating point number.
        /// </summary>
        /// <param name="scalar">The number to multiply with</param>
        /// <returns>The result of the multiplication.</returns>
        IVector MultiplyByScalar(double scalar);
        
        double X { get; }
        
        double Y { get; }
        
        double Z { get; } // TODO: refactor clockwise test so third coordinate will not be needed on the interface - cw test can be done with the x and y coordinates
        
        /// <summary>
        /// Determines whether the specified object represents the same point 
        /// as the current point.
        /// </summary>
        /// <param name="other">The other object to compare with</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        bool Equals(object other);

        /// <summary>
        /// Determines whether the specified point represents the same point 
        /// as the current point.
        /// </summary>
        /// <param name="other">The other point to compare with</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        bool Equals(IVector other);

        /// <summary>
        /// Returns whether <paramref name="other"/> is in clockwise direction from the current vector.
        /// Paralel vectors are treated as non-clockwise.
        /// </summary>
        /// <param name="other">The vector to determine the orientation to.</param>
        /// <returns>Result of clockwise test.</returns>
        bool ClockWise(IVector other);

        /// <summary>
        /// Returns whether <paramref name="other"/> is in counter-clockwise direction from the current vector.
        /// Paralel vectors are treated as non-counter-clockwise.
        /// </summary>
        /// <param name="other">The vector to determine the orientation to.</param>
        /// <returns>Result of counter-clockwise test.</returns>
        bool CounterClockWise(IVector other);
    }
}