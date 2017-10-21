using System;

namespace TriangulatedPolygonAStar.BasicGeometry
{
    public class Edge : IEdge
    {
        private Vector a;
        private Vector b;

        /// <summary>
        /// Initializes a new instance of the <see cref="Edge"/> class.
        /// </summary>
        /// <param name="a">The first endpoint.</param>
        /// <param name="b">The second endpoint.</param>
        /// <exception cref="ArgumentException">If the specified endpoints are equal with each other</exception>
        public Edge(Vector a, Vector b)
        {
            if (a == null)
            {
                throw new ArgumentNullException(nameof(a));
            }
            if (b == null)
            {
                throw new ArgumentNullException(nameof(b));
            }
            if (a.Equals(b))
            {
                throw new ArgumentException("The specified endpoints are equal");
            }
            
            this.a = a;
            this.b = b;
        }

        public IVector A
        {
            get { return a; }
        }

        public IVector B
        {
            get { return b; }
        }

        public bool PointLiesOnEdge(IVector point)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }
            
            return this.DistanceFrom(point) < VectorEqualityCheck.Tolerance;
        }
        
        public double DistanceFrom(IVector point)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }
            
            return point.DistanceFrom(ClosestPointTo(point));
        }
        
        // source: http://www.gamedev.net/topic/444154-closest-point-on-a-line/
        public IVector ClosestPointTo(IVector point)            // Vector GetClosetPoint(Vector A, Vector B, Vector P, bool segmentClamp){
        {                                                   // segmentClamp = true
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }
            
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

        public override bool Equals(object other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            
            Edge otherEdge = other as Edge;
            if (otherEdge != null)
            {
                return ((otherEdge.A.Equals(this.A) && otherEdge.B.Equals(this.B)) || (otherEdge.A.Equals(this.B) && otherEdge.B.Equals(this.A)));
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return a.GetHashCode() ^ b.GetHashCode();
        }

    }
}