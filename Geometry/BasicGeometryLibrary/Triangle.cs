using System;
using System.Collections.Generic;
using System.Linq;

namespace TriangulatedPolygonAStar.BasicGeometry
{
    /// <summary>
    /// Represents a triangle by three corner point.
    /// </summary>
    public class Triangle : ITriangle
    {
        private IVector v1;
        private IVector v2;
        private IVector v3;
        private IEnumerable<Triangle> neighbours;

        // TODO add comment
        public Triangle(IVector v1, IVector v2, IVector v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
            this.neighbours = Enumerable.Empty<Triangle>();
        }
        
        public IVector A => v1; // TODO decide whether we use this syntactic sugar or not

        public IVector B => v2;

        public IVector C => v3;

        public IEnumerable<ITriangle> Neighbours => neighbours;

        public void SetNeighbours(params Triangle[] neighbours)
        {
            if (neighbours.Length > 3)
            {
                throw new ArgumentOutOfRangeException("Parameter 'neighbours' exceeds the maximum allowed size of 3");
            }
            this.neighbours = neighbours;
        }

        // Source: http://www.blackpawn.com/texts/pointinpoly/default.html
        public bool ContainsPoint(IVector p)
        {
            IVector a = this.A;
            IVector b = this.B;
            IVector c = this.C;

            // Compute vectors
            IVector v0 = c.Minus(a); // v0 = C - A
            IVector v1 = b.Minus(a); // v1 = B - A
            IVector v2 = p.Minus(a); // v2 = P - A

            // Compute dot products
            double dot00 = v0.DotProduct(v0); // dot00 = dot(v0, v0)
            double dot01 = v0.DotProduct(v1); // dot01 = dot(v0, v1)
            double dot02 = v0.DotProduct(v2); // dot02 = dot(v0, v2)
            double dot11 = v1.DotProduct(v1); // dot11 = dot(v1, v1)
            double dot12 = v1.DotProduct(v2); // dot12 = dot(v1, v2)

            // Compute barycentric coordinates
            double invDenom = 1 / (dot00 * dot11 - dot01 * dot01); // invDenom = 1 / (dot00 * dot11 - dot01 * dot01)
            double u = (dot11 * dot02 - dot01 * dot12) * invDenom; // u = (dot11 * dot02 - dot01 * dot12) * invDenom
            double v = (dot00 * dot12 - dot01 * dot02) * invDenom; // v = (dot00 * dot12 - dot01 * dot02) * invDenom

            // Check if point is in triangle
            // Smaller modification - point can fall to the edge of the triangle
            return (u >= 0) && (v >= 0) && (u + v <= 1); // return (u >= 0) && (v >= 0) && (u + v < 1)
        }

        public IEdge GetCommonEdge(ITriangle other)
        {
            var otherTriangleVertices = new [] { other.A, other.B, other.C };

            bool aIsEqual = otherTriangleVertices.Any(vertex => vertex.Equals(this.A));
            bool bIsEqual = otherTriangleVertices.Any(vertex => vertex.Equals(this.B));
            bool cIsEqual = otherTriangleVertices.Any(vertex => vertex.Equals(this.C));

            // TODO exception if every vertex is the same (same triangle) (under discussion)
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

        public override bool Equals(object other)
        {
            ITriangle otherTriangle = other as ITriangle; // TODO should i require the same type, or should i only require the common interface?
            if (otherTriangle != null)
            {
                return (
                    ((otherTriangle.A.Equals(A)) || (otherTriangle.A.Equals(B)) || (otherTriangle.A.Equals(C)))
                    &&
                    ((otherTriangle.B.Equals(A)) || (otherTriangle.B.Equals(B)) || (otherTriangle.B.Equals(C)))
                    &&
                    ((otherTriangle.C.Equals(A)) || (otherTriangle.C.Equals(B)) || (otherTriangle.C.Equals(C)))
                );
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return A.GetHashCode() ^ B.GetHashCode() ^ C.GetHashCode();
        }
        
    }
}