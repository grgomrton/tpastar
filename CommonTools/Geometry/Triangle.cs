#region Imports

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

#endregion

namespace CommonTools.Geometry
{
    /// <summary>
    /// Represents a triangle by three corner point.
    /// </summary>
    public class Triangle : ITriangle
    {
        private Vector3 v1;
        private Vector3 v2;
        private Vector3 v3;
        private Vector3 centroid;
        private Triangle[] neighbours;

        /// <summary>
        /// Initializes a new instance of the <see cref="Triangle"/> class.
        /// </summary>
        /// <param name="v1">The v1 corner point.</param>
        /// <param name="v2">The v2 corner point.</param>
        /// <param name="v3">The v3 corner point.</param>
        public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
            : this(v1, v2, v3, -1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Triangle"/> class that stores a triangle id.
        /// </summary>
        /// <param name="v1">The v1 corner point.</param>
        /// <param name="v2">The v2 corner point.</param>
        /// <param name="v3">The v3 corner point.</param>
        /// <param name="id">The id displayed in metadata.</param>
        public Triangle(Vector3 v1, Vector3 v2, Vector3 v3, int id)
        {
            // Initialisation
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
            centroid = (v1 + v2 + v3) / 3.0;
            neighbours = new Triangle[0];
        }

        /// <summary>
        /// Gets the v1 corner point of the triangle.
        /// </summary>
        public Vector3 A => v1;

        /// <summary>
        /// Gets the v2 corner point of the triangle.
        /// </summary>
        public Vector3 B => v2;

        /// <summary>
        /// Gets the v3 corner point of the triangle.
        /// </summary>
        public Vector3 C => v3;

        /// <summary>
        /// Gets the centroid of the triangle.
        /// </summary>
        public Vector3 Centroid => centroid;

        /// <summary>
        /// Gets the neighbours.
        /// </summary>
        public IEnumerable<ITriangle> Neighbours => neighbours;

        /// <summary>
        /// Sets the neighbours.
        /// </summary>
        /// <param name="neighbours">The neighbours.</param>
        public void SetNeighbours(params Triangle[] neighbours)
        {
            if (neighbours.Length > 3)
            {
                throw new ArgumentOutOfRangeException("Parameter 'neighbours' exceeds the maximum allowed size of 3");
            }
            this.neighbours = neighbours;
        }

        /// <summary>
        /// Implements the reference equality test operator.
        /// </summary>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Triangle t1, Triangle t2)
        {
            return (((object) t1) == ((object) t2));
        }

        /// <summary>
        /// Implements the operator reference not-equal operator.
        /// </summary>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Triangle t1, Triangle t2)
        {
            return (((object) t1) != ((object) t2));
        }

        /// <summary>
        /// Determines whether the triangle contains the specified p point
        /// using the Barycentric Technique.
        /// Points that fall on the edge are inside the triangle.
        /// Source: http://www.blackpawn.com/texts/pointinpoly/default.html
        /// </summary>
        /// <param name="p">The point.</param>
        /// <returns>
        ///   <c>true</c> if the triangle contains specified p point; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsPoint(Vector3 p)
        {
            Vector3 a = this.A;
            Vector3 b = this.B;
            Vector3 c = this.C;

            // Compute vectors
            Vector3 v0 = c - a; // v0 = C - A
            Vector3 v1 = b - a; // v1 = B - A
            Vector3 v2 = p - a; // v2 = P - A

            // Compute dot products
            double dot00 = Vector3.DotProduct(v0, v0); // dot00 = dot(v0, v0)
            double dot01 = Vector3.DotProduct(v0, v1); // dot01 = dot(v0, v1)
            double dot02 = Vector3.DotProduct(v0, v2); // dot02 = dot(v0, v2)
            double dot11 = Vector3.DotProduct(v1, v1); // dot11 = dot(v1, v1)
            double dot12 = Vector3.DotProduct(v1, v2); // dot12 = dot(v1, v2)

            // Compute barycentric coordinates
            double invDenom = 1 / (dot00 * dot11 - dot01 * dot01); // invDenom = 1 / (dot00 * dot11 - dot01 * dot01)
            double u = (dot11 * dot02 - dot01 * dot12) * invDenom; // u = (dot11 * dot02 - dot01 * dot12) * invDenom
            double v = (dot00 * dot12 - dot01 * dot02) * invDenom; // v = (dot00 * dot12 - dot01 * dot02) * invDenom

            // Check if point is in triangle
            // Smaller modification - point can fall to the edge of the triangle
            return (u >= 0) && (v >= 0) && (u + v <= 1); // return (u >= 0) && (v >= 0) && (u + v < 1)
        }

        /// <summary>
        /// Returns the minimal distance of the triangle from a point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public double MinDistanceFromOuterPoint(Vector3 point)
        {
            Edge e1 = new Edge(this.A, this.B);
            Edge e2 = new Edge(this.A, this.C);
            Edge e3 = new Edge(this.B, this.C);
            double dist1 = e1.DistanceFromPoint(point);
            double dist2 = e2.DistanceFromPoint(point);
            double dist3 = e3.DistanceFromPoint(point);
            return Math.Min(dist1, Math.Min(dist2, dist3));
        }

        /// <summary>
        /// Gets the common edge of this triangle and another adjacent triangle.
        /// </summary>
        /// <param name="other">The adjacent triangle.</param>
        /// <returns>The common edge.</returns>
        public Edge GetCommonEdge(ITriangle other)
        {
            var otherTriangleVertices = new Vector3[3];
            otherTriangleVertices[0] = other.A;
            otherTriangleVertices[1] = other.B;
            otherTriangleVertices[2] = other.C;

            bool aIsEqual = otherTriangleVertices.Any(vertex => vertex.Equals(A));
            bool bIsEqual = otherTriangleVertices.Any(vertex => vertex.Equals(B));
            bool cIsEqual = otherTriangleVertices.Any(vertex => vertex.Equals(C));

            if (aIsEqual && bIsEqual)
            {
                return new Edge(A, B);
            }
            else if (aIsEqual && cIsEqual)
            {
                return new Edge(A, C);
            }
            else if (bIsEqual && cIsEqual)
            {
                return new Edge(B, C);
            }
            else
            {
                throw new ArgumentException("The specified triangle has no common edge with this one", "other");
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other)
        {
            ITriangle otherTriangle = other as ITriangle;
            if (otherTriangle != null)
            {
                return Equals(otherTriangle);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="CommonTools.Geometry.Triangle"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="CommonTools.Geometry.Triangle"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="CommonTools.Geometry.Triangle"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(ITriangle other)
        {
            return (
                ((other.A == A) || (other.A == B) || (other.A == C))
                &&
                ((other.B == A) || (other.B == B) || (other.B == C))
                &&
                ((other.C == A) || (other.C == B) || (other.C == C))
            );
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return A.GetHashCode() ^ B.GetHashCode() ^ C.GetHashCode();
        }
    }
}