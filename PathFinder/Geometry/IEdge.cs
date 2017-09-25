namespace TriangulatedPolygonAStar
{
    /// <summary>
    /// Represents a line segment between two points in the two-dimensional space.
    /// </summary>
    public interface IEdge
    {
        /// <summary>
        /// Returns the distances between the the specified point and the edge.
        /// </summary>
        /// <param name="point">The point to measure the distance from.</param>
        /// <returns>The distance between the closest point of the edge and the specified point.</returns>
        double DistanceFromPoint(IVector point);

        /// <summary>
        /// Returns the closest point on this edge from the specified point.
        /// </summary>
        /// <param name="point">The point to measure the distance from.</param>
        /// <returns>The closest point of the edge to the specified point.</returns>
        IVector ClosestPointTo(IVector point);
        
        /// <summary>
        /// The first endpoint of the edge. 
        /// No specific geometrical property of the endpoints is expected.
        /// </summary>
        IVector A { get; }

        /// <summary>
        /// The second endpoint of the edge. 
        /// No specific geometrical property of the endpoints is expected.
        /// </summary>
        IVector B { get; }
        
        /// <summary>
        /// Determines whether the specified object represents the same edge 
        /// as the current edge.
        /// </summary>
        /// <param name="other">The other object to compare with</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        bool Equals(object other);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        int GetHashCode();
    }
}