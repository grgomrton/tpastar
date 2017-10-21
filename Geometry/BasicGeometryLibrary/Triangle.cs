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

        public void SetNeighbours(IEnumerable<Triangle> neighbours)
        {
            if (neighbours.Count() > 3)
            {
                throw new ArgumentOutOfRangeException("Maximum allowed amount of neighbour triangles is 3", nameof(neighbours));
            }
            if (neighbours.Any(triangle => triangle.GetCommonVerticesWith(this).Count() != 2))
            {
                throw new ArgumentException(
                    "One or more of the specified triangles are not adjacent with this triangle", nameof(neighbours));
            }
            
            var edgeSet = new Dictionary<ITriangle, Edge>();
            foreach (Triangle neighbour in neighbours)
            {
                var commonVertices = GetCommonVerticesWith(neighbour);
                var adjacentEdge = new Edge(commonVertices.ElementAt(0), commonVertices.ElementAt(1));
                edgeSet.Add(neighbour, adjacentEdge);
            }
            adjacentEdges = edgeSet;
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
            // Smaller modification - point can fall on the edge of the triangle
            // Here I need smaller than or equal with one in order to accept edge points
            return (u >= 0) && (v >= 0) && (u + v < 1.0 + VectorEqualityCheck.Tolerance); // return (u >= 0) && (v >= 0) && (u + v < 1)
        }

        public IEdge GetCommonEdgeWith(ITriangle other)
        {
            if (!adjacentEdges.ContainsKey(other))
            {
                throw new ArgumentException("The specified triangle cannot be found among the neighbours", nameof(other));
            }
            return adjacentEdges[other];
        }

        public override bool Equals(object other)
        {
            Triangle otherTriangle = other as Triangle;
            if (otherTriangle != null)
            {
                return GetCommonVerticesWith(otherTriangle).Count() == 3;
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

        /// <summary>
        /// Determines the set of vertices shared by this triangle and the specified one.
        /// </summary>
        /// <param name="other">The other triangle to compare this triangle with</param>
        /// <returns></returns>
        public IEnumerable<Vector> GetCommonVerticesWith(Triangle other)
        {
            var myTriangleVertices = new [] { a, b, c };
            var otherTriangleVertices = new [] { other.a, other.b, other.c };
            return myTriangleVertices.Intersect(otherTriangleVertices);
        }
               
    }
}