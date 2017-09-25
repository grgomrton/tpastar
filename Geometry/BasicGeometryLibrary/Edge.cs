using System;

namespace TriangulatedPolygonAStar.BasicGeometry
{
    public class Edge : IEdge
    {
        private Vector v1;
        private Vector v2;

        /// <summary>
        /// Initializes a new instance of the <see cref="Edge"/> class.
        /// </summary>
        /// <param name="v1">The first endpoint.</param>
        /// <param name="v2">The second endpoint.</param>
        public Edge(Vector v1, Vector v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        public IVector A
        {
            get { return v1; }
        }

        public IVector B
        {
            get { return v2; }
        }

        // source: http://www.gamedev.net/topic/444154-closest-point-on-a-line/
        public IVector ClosestPointTo(IVector p)    // Vector GetClosetPoint(Vector A, Vector B, Vector P, bool segmentClamp){
        {                                                   // segmentClamp = true
            IVector closest;
            var ap = p.Minus(A);                            // Vector AP = P - A:
            var ab = B.Minus(A);                            // Vector AB = B - A;
            var ab2 = ab.X * ab.X + ab.Y * ab.Y;            // float ab2 = AB.x*AB.x + AB.y*AB.y;
            if (ab2.Equals(0))                              // our segment is only one point - that must be the closest point
            {
                closest = ab;
            }
            else
            {
                var ap_ab = ap.X * ab.X + ap.Y * ab.Y;      // float ap_ab = AP.x*AB.x + AP.y*AB.y;
                var t = ap_ab / ab2;                        // float t = ap_ab / ab2;
                t = Math.Max(Math.Min(t, 1.0), 0.0);        // if (segmentClamp) {
                                                            //   if (t < 0.0f) t = 0.0f;
                                                            //   else if (t > 1.0f) t = 1.0f;    
                                                            // }
                closest = A.Plus(ab.MultiplyByScalar(t));   // Vector Closest = A + AB * t;
            }
            
            return closest;
        }

        // source: http://softsurfer.com/Archive/algorithm_0102/algorithm_0102.htm
        public double DistanceFromPoint(IVector p)  // float dist_Point_to_Segment(Point P, Segment S) {
        {
            double ret;

            var v = this.B.Minus(this.A);          // Vector v =S.P1 - S.P0;
            var w = p.Minus(this.A);                // Vector w = P - S.P0;

            double c1 = w.DotProduct(v);   // double c1 = dot(w, v);
            double c2 = v.DotProduct(v);   // double c2 = dot(v, v);
            if (c1 <= 0)                            // if (c1 <= 0)
            {
                ret = p.DistanceFrom(this.A); //   return d(P, S.P0);
            }
            else if (c2 <= c1)                      // if (c2 <= c1)
            {
                ret = p.DistanceFrom(this.B); // return d(P, S.P1);
            }
            else
            {
                double b = c1 / c2;                 // double b = c1 / c2;
                IVector pb = this.A.Plus(v.MultiplyByScalar(b));       // Point Pb = S.P0 + b * v;
                ret = p.DistanceFrom(pb);      // return d(P, Pb);
            }

            return ret;
        }

        public override bool Equals(object other)
        {
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
            return A.GetHashCode() ^ B.GetHashCode();
        }

    }
}