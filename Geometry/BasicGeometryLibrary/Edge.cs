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

using System;
using System.Linq;

namespace TriangulatedPolygonAStar.BasicGeometry
{
    /// <inheritdoc />
    public class Edge : IEdge
    {
        private readonly Vector a;
        private readonly Vector b;
        private readonly int firstNeighbourId;
        private readonly int secondNeighbourId;

        /// <summary>
        /// Initializes a new instance of the <see cref="Edge"/> class by two endpoints. 
        /// Distorted segments which consist of overlapping points are not supported.
        /// Edges instantiated without neighbour triangles have identical hashes.
        /// </summary>
        /// <param name="a">The first endpoint of the edge.</param>
        /// <param name="b">The second endpoint of the edge.</param>
        public Edge(Vector a, Vector b)
        {
            CheckForNullArgument(a, nameof(a));
            CheckForNullArgument(b, nameof(b));
            if (a.Equals(b))
            {
                throw new ArgumentException("The specified endpoints are equal");
            }

            Set(
                ref this.a, a, 
                ref this.b, b, 
                ref this.firstNeighbourId, 0, 
                ref this.secondNeighbourId, 0);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Edge"/> class by two adjacent triangles. 
        /// The edge will represent the common edge shared by these triangles. 
        /// </summary>
        /// <param name="firstNeighbourTriangle">The first adjacent triangle</param>
        /// <param name="secondNeighbourTriangle">The second adjacent triangle</param>
        public Edge(Triangle firstNeighbourTriangle, Triangle secondNeighbourTriangle)
        {
            CheckForNullArgument(firstNeighbourTriangle, nameof(firstNeighbourTriangle));
            CheckForNullArgument(secondNeighbourTriangle, nameof(secondNeighbourTriangle));
            var commonVertices = firstNeighbourTriangle.GetCommonVerticesWith(secondNeighbourTriangle).ToArray();
            if (commonVertices.Length != 2)
            {
                throw new ArgumentException("The specified triangles are not adjacent");
            }

            Set(
                ref this.a, commonVertices[0], 
                ref this.b, commonVertices[1], 
                ref this.firstNeighbourId, firstNeighbourTriangle.Id, 
                ref this.secondNeighbourId, secondNeighbourTriangle.Id);
        }

        private static void Set(ref Vector firstEndpoint, Vector a, ref Vector secondEndPoint, Vector b, ref int firstNeighbourId, int firstNeighbourIdValue, ref int secondNeighbourId, int secondNeighbourIdValue)
        {
            firstEndpoint = a;
            secondEndPoint = b;
            firstNeighbourId = firstNeighbourIdValue;
            secondNeighbourId = secondNeighbourIdValue;
        }
        
        /// <inheritdoc />
        public IVector A
        {
            get { return a; }
        }

        /// <inheritdoc />
        public IVector B
        {
            get { return b; }
        }

        /// <summary>
        /// Indicates, whether the specified point lies on this edge. 
        /// A point is considered to be lying on the edge if the 
        /// closest point on the edge is equal with the point itself.
        /// It also means that the point is closer to the edge than the value defined in
        /// <see cref="VectorEqualityCheck.Tolerance"/>.
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>true if the point falls on this edge, otherwise false</returns>
        public bool PointLiesOnEdge(IVector point)
        {
            CheckForNullArgument(point, nameof(point));

            return ClosestPointTo(point).Equals(point);
        }

        /// <inheritdoc />
        public double DistanceFrom(IVector point)
        {
            CheckForNullArgument(point, nameof(point));

            return point.DistanceFrom(ClosestPointTo(point));
        }

        // source: http://www.gamedev.net/topic/444154-closest-point-on-a-line/
        /// <inheritdoc />
        public IVector ClosestPointTo(IVector point)        // Vector GetClosetPoint(Vector A, Vector B, Vector P, bool segmentClamp){
        {
            CheckForNullArgument(point, nameof(point));      
                                                            // segmentClamp = true
            var ap = point.Minus(A);                        // Vector AP = P - A:
            var ab = B.Minus(A);                            // Vector AB = B - A;
            var ab2 = ab.X * ab.X + ab.Y * ab.Y;            // float ab2 = AB.x*AB.x + AB.y*AB.y;
            var ap_ab = ap.X * ab.X + ap.Y * ab.Y;          // float ap_ab = AP.x*AB.x + AP.y*AB.y;
            var t = ap_ab / ab2;                            // float t = ap_ab / ab2;
            t = Math.Max(Math.Min(t, 1.0), 0.0);            // if (segmentClamp) {
                                                            //   if (t < 0.0f) t = 0.0f;
                                                            //   else if (t > 1.0f) t = 1.0f;    
                                                            // }
            var closest = A.Plus(ab.Times(t));              // Vector Closest = A + AB * t;

            return closest;
        }

        /// <summary>
        /// Determines whether the specified object represents the same edge as this one.
        /// Two edges are considered to be equal if their endpoints are closer than the value 
        /// specified in <see cref="VectorEqualityCheck.Tolerance"/>.
        /// In such case the highest distance between the two edges is also lower than the tolerance value.
        /// Please note, that since <see cref="Vector"/> instances are compared with an absolute
        /// tolerance, the <see cref="Equals"/> implementation will not be transitive, meaning
        /// a.equals(b) && b.equals(c) => a.equals(c) will not necessarily hold.
        /// </summary>
        /// <param name="other">The other object to compare with</param>
        /// <returns>true if the specified object is equal to the current object, otherwise false</returns>
        public override bool Equals(object other)
        {
            Edge otherEdge = other as Edge;
            if (otherEdge != null)
            {
                return ((otherEdge.a.Equals(this.a) && otherEdge.b.Equals(this.b)) ||
                        (otherEdge.b.Equals(this.a) && otherEdge.a.Equals(this.b)));
            }
            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance. 
        /// <see cref="GetHashCode"/> provides a useful distribution only for edges that have been created with 
        /// <see cref="Edge(TriangulatedPolygonAStar.BasicGeometry.Triangle,TriangulatedPolygonAStar.BasicGeometry.Triangle)"/>.
        /// </summary>
        /// <returns>An integer value that specifies a hash value for this instance</returns>
        public override int GetHashCode()
        {
            return unchecked(firstNeighbourId + secondNeighbourId);
        }

        private static void CheckForNullArgument(object value, string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }
    }
}