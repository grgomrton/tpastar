﻿using System;
using System.Collections.Generic;

namespace TriangulatedPolygonAStar
{
    /// <summary>
    /// Represents a triangle in the two-dimensional space.
    /// </summary>
    public interface ITriangle
    {
        /// <summary>
        /// Determines whether the specifified point falls inside the triangle.
        /// Points falling on the edge of the triangle are expected to determined
        /// as a contained point.
        /// </summary>
        /// <param name="point">To point to check</param>
        /// <returns>true if the specified point falls inside the triangle; otherwise, false.</returns>
        bool ContainsPoint(IVector point); 
        
        /// <summary>
        /// List of neighbour triangles. 
        /// A triangle is a neighbour of another if they share exactly two vertices.
        /// </summary>
        IEnumerable<ITriangle> Neighbours { get; }
        
        /// <summary>
        /// Determines whether the specified object represents the same triangle 
        /// as the current triangle.
        /// </summary>
        /// <param name="other">The other object to compare with</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        bool Equals(object other);
        
        /// <summary>
        /// Gets the common edge of this triangle and another adjacent triangle.
        /// </summary>
        /// <param name="other">The adjacent triangle</param>
        /// <returns>The common edge.</returns>
        /// <exception cref="ArgumentException">In case the triangles have no common edge</exception>
        IEdge GetCommonEdgeWith(ITriangle other);
        
    }
}