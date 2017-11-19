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
    /// Contains the result of an evaluation of a triangle during traversing the triangle graph.
    /// </summary>
    public class TriangleEvaluationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriangleEvaluationResult"/> class
        /// which contains the result of a triangle evaluation during exploring the triangle graph.
        /// </summary>
        /// <param name="path">The result of the evaluation of this triangle</param>
        public TriangleEvaluationResult(TPAPath path)
        {
            EstimatedMinimalCost = path.MinimalTotalCost;
            ShortestPathToEdgeLength = path.ShortestPathToEdgeLength;
            LongestPathToEdgeLength = path.LongestPathToEdgeLength;
        }  

        /// <summary>
        /// The length of the possibly shortest path from the start to the closest goal point along the set of triangles
        /// stepped over during building the path.
        /// </summary>
        public double EstimatedMinimalCost { get; private set; }
        
        /// <summary>
        /// The length of the possibly shortest path from the start to the current edge along the set of triangles
        /// stepped over during building the path.
        /// </summary>
        public double ShortestPathToEdgeLength { get; private set; }
        
        /// <summary>
        /// The length of the longest possible path from the start to the current edge along the set of triangles
        /// stepped over during building the path.
        /// </summary>
        public double LongestPathToEdgeLength { get; private set; }
    }
}