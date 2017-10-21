﻿namespace TriangulatedPolygonAStar
{
    /// <summary>
    /// Represents a line segment between two points in the two-dimensional space.
    /// </summary>
    public interface IEdge
    {
        /// <summary>
        /// The first endpoint of the edge. 
        /// No specific order of the endpoints is expected.
        /// </summary>
        IVector A { get; }

        /// <summary>
        /// The second endpoint of the edge. 
        /// No specific order of the endpoints is expected.
        /// </summary>
        IVector B { get; }
        
        /// <summary>
        /// Returns the distances between the the specified point and the edge.
        /// </summary>
        /// <param name="point">The point to measure the distance from.</param>
        /// <returns>The distance between the closest point of the edge and the specified point.</returns>
        double DistanceFrom(IVector point);

        /// <summary>
        /// Returns the closest point on this edge from the specified point.
        /// </summary>
        /// <param name="point">The point to measure the distance from.</param>
        /// <returns>The closest point of the edge to the specified point.</returns>
        IVector ClosestPointTo(IVector point);

        /// <summary>
        /// Indicates, whether the specified point lies on this edge. A point is
        /// considered to by lying on the edge if the closest point to the edge from
        /// this point considered to be equal with the point itself.
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns><c>true</c> if the point falls on this edge, otherwise <c>false</c></returns>
        bool PointLiesOnEdge(IVector point);
        
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