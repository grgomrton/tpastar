namespace TriangulatedPolygonAStar.Geometry
{
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
        /// <returns>The closest point to the point of the <see cref="Edge"/>.</returns>
        IVector ClosestPointOnEdgeFrom(IVector point);
        
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
        /// Determines whether the specified edge represents the same edge 
        /// as the current edge.
        /// </summary>
        /// <param name="other">The other edge to compare with</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        bool Equals(IEdge other);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        int GetHashCode();
    }
}