/**
 * Copyright 2017 Márton Gergó
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
        /// Returns the edge shared by this triangle and the specified neighbour triangle.
        /// </summary>
        /// <param name="other">The neighbour triangle</param>
        /// <returns>The common edge of the two triangles</returns>
        IEdge GetCommonEdgeWith(ITriangle other);
        
        /// <summary>
        /// Determines whether the specifified point falls inside the triangle.
        /// Points that lie on the edges are expected to be determined as contained points.
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>true if the specified point falls inside the triangle, otherwise false</returns>
        bool ContainsPoint(IVector point); 
        
        /// <summary>
        /// Determines whether the specified object represents 
        /// the same triangle as this one.
        /// </summary>
        /// <param name="other">The other object to compare with</param>
        /// <returns>true if the specified object is equal to the current triangle, otherwise false</returns>
        bool Equals(object other);
    }
}