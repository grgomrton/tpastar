using System;
using System.Collections.Generic;

namespace TriangulatedPolygonAStar
{
    /// <summary>
    /// Represents a triangle in the two-dimensional space.
    /// </summary>
    public interface ITriangle
    {
        /// <summary>
        /// The triangles which share exactly two vertices with this triangle.
        /// </summary>
        IEnumerable<ITriangle> Neighbours { get; }
        
        /// <summary>
        /// Returns the edge shared by this triangle and the specified one.
        /// </summary>
        /// <param name="other">The adjacent triangle</param>
        /// <returns>The common edge of the two triangles.</returns>
        /// <exception cref="ArgumentException">In case the triangles have no common edge</exception>
        IEdge GetCommonEdgeWith(ITriangle other);
        
        /// <summary>
        /// Determines whether the specifified point falls inside the triangle.
        /// Points that lie on the edges are expected to be determined as 
        /// contained points.
        /// </summary>
        /// <param name="point">To point to check</param>
        /// <returns>true if the specified point falls inside the triangle; otherwise, false.</returns>
        bool ContainsPoint(IVector point); 
        
        /// <summary>
        /// Determines whether the specified object represents the same triangle 
        /// as this one.
        /// </summary>
        /// <param name="other">The other object to compare with</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        bool Equals(object other);
    }
}