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

namespace TriangulatedPolygonAStar
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
        /// Returns the cartesian distances between the specified point and the edge.
        /// </summary>
        /// <param name="point">The point to measure the distance from.</param>
        /// <returns>The distance between the closest point of the edge and the specified point</returns>
        double DistanceFrom(IVector point);

        /// <summary>
        /// Returns the closest point on this edge from the specified point.
        /// </summary>
        /// <param name="point">The point to measure the distance from.</param>
        /// <returns>The closest point of the edge to the specified point</returns>
        IVector ClosestPointTo(IVector point);

        /// <summary>
        /// Indicates, whether the specified point lies on this edge.
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>true if the point falls on this edge, otherwise false</returns>
        bool PointLiesOnEdge(IVector point);
        
        /// <summary>
        /// Determines whether the specified object represents the same edge 
        /// as this one.
        /// </summary>
        /// <param name="other">The other object to compare with</param>
        /// <returns>true if the specified object is equal to the current object, otherwise false</returns>
        bool Equals(object other);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>An integer value that specifies a hash value for this instance</returns>
        int GetHashCode();
    }
}