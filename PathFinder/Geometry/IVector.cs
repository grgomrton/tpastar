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
    /// Represents a point in the two-dimensional space.
    /// </summary>
    public interface IVector
    {
        /// <summary>
        /// The horizontal component of the vector.
        /// </summary>
        double X { get; }

        /// <summary>
        /// The vertical component of the vector.
        /// </summary>
        double Y { get; }

        /// <summary>
        /// Returns the result of adding the specified vector to the current one. 
        /// </summary>
        /// <param name="other">The vector to add</param>
        /// <returns>The result of the addition</returns>
        IVector Plus(IVector other);

        /// <summary>
        /// Returns the result of subtracting the specified vector from the current one. 
        /// </summary>
        /// <param name="other">The vector to subtract</param>
        /// <returns>The result of the subtraction</returns>
        IVector Minus(IVector other);

        /// <summary>
        /// Returns the result of multiplying this vector with a scalar value.
        /// </summary>
        /// <param name="scalar">The value to multiply with</param>
        /// <returns>The result of the multiplication</returns>
        IVector Times(double scalar);

        /// <summary>
        /// Returns the cartesian distance from the specified point.
        /// </summary>
        /// <param name="other">The point to measure the distance from</param>
        /// <returns>The distance between the two points</returns>
        double DistanceFrom(IVector other);

        /// <summary>
        /// Returns whether the current vector is in clockwise direction from <paramref name="other"/>.
        /// Parallel vectors should pass the clockwise test.
        /// </summary>
        /// <param name="other">The vector to determine the orientation from</param>
        /// <returns>Result of the clockwise test</returns>
        bool IsInClockWiseDirectionFrom(IVector other);

        /// <summary>
        /// Returns whether the current vector is in counter-clockwise direction from <paramref name="other"/>.
        /// Parallel vectors should pass the counter-clockwise test.
        /// </summary>
        /// <param name="other">The vector to determine the orientation from</param>
        /// <returns>Result of the counter-clockwise test</returns>
        bool IsInCounterClockWiseDirectionFrom(IVector other);

        /// <summary>
        /// Determines whether the specified object represents 
        /// the same point as this one.
        /// </summary>
        /// <param name="other">The other object to compare with</param>
        /// <returns>true if the specified object is equal to the current object, otherwise false</returns>
        bool Equals(object other);
    }
}