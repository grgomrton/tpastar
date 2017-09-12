using System;
using System.Collections.Generic;

namespace TriangulatedPolygonAStar
{
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
        IEdge GetCommonEdge(ITriangle other);
        
        
        int GetHashCode(); // TODO do we really require the hashcode in pathfinder? or is it due to the equals method?
                                   // note: if we only need the equality check, let's remove the hashcode from the interface
                                   // in such case the hashcode is a requirement from the language and not the algorithm.
                                   // we define here the requirements of the algorithm. Optionally we can mention the
                                   // hashcode requirement in the equals method comment.

        /// <summary>
        /// The first vertex of the triangle. No specific order of the vertices is expected.
        /// </summary>
        IVector A { get; }
        
        /// <summary>
        /// The second vertex of the triangle. No specific order of the vertices is expected.
        /// </summary>
        IVector B { get; }

        /// <summary>
        /// The third vertex of the triangle. No specific order of the vertices is expected.
        /// </summary>
        IVector C { get; }
    }
}