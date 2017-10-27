using System;

namespace TriangulatedPolygonAStar.BasicGeometry
{
    /// <inheritdoc />
    public class Edge : IEdge
    {
        private readonly Vector a;
        private readonly Vector b;

        /// <summary>
        /// Initializes a new instance of the <see cref="Edge"/> class by two endpoints. 
        /// Distorted segments which consist of overlapping points cannot be created.
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

            this.a = a;
            this.b = b;
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
        /// A point is considered to be lying on the edge if the closest point on the edge equals with the point itself.
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>true if the point falls on this edge, otherwise false</returns>
        public bool PointLiesOnEdge(IVector point)
        {
            CheckForNullArgument(point, nameof(point));

            return DistanceFrom(point) < VectorEqualityCheck.Tolerance;
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

        /// <inheritdoc cref="IEdge.Equals(object)" />
        public override bool Equals(object other)
        {
            CheckForNullArgument(other, nameof(other));

            Edge otherEdge = other as Edge;
            if (otherEdge != null)
            {
                return ((otherEdge.A.Equals(this.A) && otherEdge.B.Equals(this.B)) ||
                        (otherEdge.A.Equals(this.B) && otherEdge.B.Equals(this.A)));
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc cref="IEdge.GetHashCode" />
        public override int GetHashCode()
        {
            return a.GetHashCode() ^ b.GetHashCode();
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