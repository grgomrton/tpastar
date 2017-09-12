#region Imports

using System;
using System.Drawing;
using System.Collections.Generic;
using TriangulatedPolygonAStar.Geometry;

#endregion

namespace TriangulatedPolygonAStar.Geometry
{

    /// <summary>
    /// Represents an edge by two endpoints.
    /// </summary>
    public class Edge : IEdge
    {
        private IVector v1;
        private IVector v2;
        private double length;

        /// <summary>
        /// Initializes a new instance of the <see cref="Edge"/> class.
        /// </summary>
        /// <param name="v1">The v1 endpoint.</param>
        /// <param name="v2">The v2 endpoint.</param>
        public Edge(IVector v1, IVector v2)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.length = Vector3.Distance(v1, v2);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Edge"/> class.
        /// </summary>
        /// <param name="e">The <see cref="Edge"/> to copy.</param>
        public Edge(Edge e)
        {
            this.v1 = e.A;
            this.v2 = e.B;
            this.length = e.length;
        }

        /// <summary>
        /// Gets the first endpoint of the edge.
        /// </summary>
        public IVector A
        {
            get { return v1; }
        }

        /// <summary>
        /// Gets the second endpoint of the edge.
        /// </summary>
        public IVector B
        {
            get { return v2; }
        }

        /// <summary>
        /// Gets the length of the edge.
        /// </summary>
        public double Length
        {
            get { return length; }
        }


        /// <summary>
        /// Returns the closest point on this edge which is the closest the specified point.
        /// Source: http://www.gamedev.net/topic/444154-closest-point-on-a-line/
        /// </summary>
        /// <param name="p">The point.</param>
        /// <returns>The closest point of the <see cref="Edge"/>.</returns>
        public IVector ClosestPointOnEdgeFrom(IVector p)      // Vector GetClosetPoint(Vector A, Vector B, Vector P, bool segmentClamp){
        {
            IVector closestPoint = new Vector3();
                                                            // segmentClamp = true
            IVector ap = p.Minus(A);                            // Vector AP = P - A:
            IVector ab = B.Minus(A);                           // Vector AB = B - A;
            double ab2 = ab.X * ab.X + ab.Y * ab.Y;         // float ab2 = AB.x*AB.x + AB.y*AB.y;
            if (ab2 == 0)                                   // our segment is only one point - that must be the closest point
            {
                closestPoint = ab;
            }
            else
            {
                double ap_ab = ap.X * ab.X + ap.Y * ab.Y;   // float ap_ab = AP.x*AB.x + AP.y*AB.y;
                double t = ap_ab / ab2;                     // float t = ap_ab / ab2;
                t = Math.Max(Math.Min(t, 1.0), 0.0);        // if (segmentClamp) {
                                                            //   if (t < 0.0f) t = 0.0f;
                                                            //   else if (t > 1.0f) t = 1.0f;    
                                                            // }
                closestPoint = A.Plus(ab.MultiplyByScalar(t));                 // Vector Closest = A + AB * t;
            }
            return closestPoint;                            // return Closest;
        }

        /// <summary>
        /// Returns the distances between the the specified point and the edge.
        /// Source: http://softsurfer.com/Archive/algorithm_0102/algorithm_0102.htm
        /// </summary>
        /// <param name="p">The point.</param>
        /// <returns>The distance.</returns>
        public double DistanceFromPoint(IVector p)  // float dist_Point_to_Segment(Point P, Segment S) {
        {
            double ret;

            IVector v = this.B.Minus(this.A);          // Vector v =S.P1 - S.P0;
            IVector w = p.Minus(this.A);                // Vector w = P - S.P0;

            double c1 = Vector3.DotProduct(w, v);   // double c1 = dot(w, v);
            double c2 = Vector3.DotProduct(v, v);   // double c2 = dot(v, v);
            if (c1 <= 0)                            // if (c1 <= 0)
            {
                ret = Vector3.Distance(p, this.A); //   return d(P, S.P0);
            }
            else if (c2 <= c1)                      // if (c2 <= c1)
            {
                ret = Vector3.Distance(p, this.B); // return d(P, S.P1);
            }
            else
            {
                double b = c1 / c2;                 // double b = c1 / c2;
                IVector pb = this.A.Plus(v.MultiplyByScalar(b));       // Point Pb = S.P0 + b * v;
                ret = Vector3.Distance(p, pb);      // return d(P, Pb);
            }

            return ret;
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if other is an instance of <see cref="Edge"/> and equals the value of this instance; otherwise, <c>false</c>
        /// </returns>
        public override bool Equals(object other)
        {
            // Check object other is an Edge object
            if (other is IEdge)
            {
                IEdge otherEdge = (IEdge)other;
                // Check for equality
                return Equals(otherEdge);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a value indicating whether this instance and a specified <see cref="Edge"/> object represent the same value.
        /// </summary>
        /// <param name="other">An <see cref="Edge"/> object compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if other is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(IEdge other)
        {
            return ((other.A.Equals(this.A) && other.B.Equals(this.B)) || (other.A.Equals(this.B) && other.B.Equals(this.A)));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return A.GetHashCode() ^ B.GetHashCode();
        }

    }
}