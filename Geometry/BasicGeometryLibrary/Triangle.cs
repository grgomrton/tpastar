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
        private Vector a;
        private Vector b;
        private Vector c;
        private Dictionary<ITriangle, Edge> adjacentEdges;

        // TODO add comment
        public Triangle(Vector a, Vector b, Vector c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.adjacentEdges = new Dictionary<ITriangle, Edge>();
        }

        public IVector A
        {
            get { return a; }
        }

        public IVector B
        {
            get { return b; }
        }

        public IVector C
        {
            get { return c; }
        }

        public IEnumerable<ITriangle> Neighbours
        {
            get { return adjacentEdges.Keys; }
        }

        public void SetNeighbours(params Triangle[] neighbours)
        {
            if (neighbours.Length > 3)
            {
                throw new ArgumentOutOfRangeException("Maximum allowed amount of neighbour triangles is 3", nameof(neighbours));
            }
            if (!neighbours.All(HasCommonEdgeWithThisTriangle))
            {
                throw new ArgumentException(
                    "One or more of the specified triangles has no common edge with this triangle", nameof(neighbours));
            }
            
            adjacentEdges.Clear();
            foreach (Triangle neighbour in neighbours)
            {
                Edge adjacentEdge = DetermineCommonEdgeWith(neighbour);
                adjacentEdges.Add(neighbour, adjacentEdge);
            }
        }

        // Source: http://www.blackpawn.com/texts/pointinpoly/default.html
        public bool ContainsPoint(IVector p)
        {
            // Compute vectors
            IVector v0 = C.Minus(A); // v0 = C - A
            IVector v1 = B.Minus(A); // v1 = B - A
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
            if (!adjacentEdges.ContainsKey(other))
            {
                throw new ArgumentException("The specified triangle cannot be found amoung the neighbours", nameof(other));
            }
            return adjacentEdges[other];
        }

        public override bool Equals(object other)
        {
            Triangle otherTriangle = other as Triangle;
            if (otherTriangle != null)
            {
                return CommonVerticesWith(otherTriangle).Count() == 3;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return a.GetHashCode() ^ b.GetHashCode() ^ c.GetHashCode();
        }
        
        private bool HasCommonEdgeWithThisTriangle(Triangle other)
        {
            return CommonVerticesWith(other).Count() == 2;
        }

        private IEnumerable<Vector> CommonVerticesWith(Triangle other)
        {
            var myTriangleVertices = new [] { a, b, c };
            var otherTriangleVertices = new [] { other.a, other.b, other.c };
            return myTriangleVertices.Intersect(otherTriangleVertices);
        }
        
        private Edge DetermineCommonEdgeWith(Triangle other)
        {
            var commonVertices = CommonVerticesWith(other);
            return new Edge(commonVertices.ElementAt(0), commonVertices.ElementAt(1));
        }
        
    }
}