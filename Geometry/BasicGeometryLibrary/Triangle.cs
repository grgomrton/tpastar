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
        private Vector[] vertices;
        private Dictionary<ITriangle, Edge> adjacentEdges;

        public Triangle(Vector a, Vector b, Vector c)
        {
            if (a == null)
            {
                throw new ArgumentNullException(nameof(a));
            }
            if (b == null)
            {
                throw new ArgumentNullException(nameof(b));
            }
            if (c == null)
            {
                throw new ArgumentNullException(nameof(c));
            }
            if (a.Equals(b) || a.Equals(c) || b.Equals(c))
            {
                throw new ArgumentOutOfRangeException("One or more of the specified vertices are equal with each other");
            }
            
            this.vertices = new[] {a, b, c};
            this.adjacentEdges = new Dictionary<ITriangle, Edge>();
        }

        public IVector A
        {
            get { return vertices[0]; }
        }

        public IVector B
        {
            get { return vertices[1]; }
        }

        public IVector C
        {
            get { return vertices[2]; }
        }

        public IEnumerable<ITriangle> Neighbours
        {
            get { return adjacentEdges.Keys; }
        }

        public void SetNeighbours(IEnumerable<Triangle> neighbours)
        {
            if (neighbours == null)
            {
                throw new ArgumentNullException(nameof(neighbours));
            }
            if (neighbours.Any(item => item == null))
            {
                throw new ArgumentException("One or more of the specified neighbours is null", nameof(neighbours));
            }
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

        public IEdge GetCommonEdgeWith(ITriangle other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            if (!adjacentEdges.ContainsKey(other))
            {
                throw new ArgumentException("The specified triangle cannot be found among the neighbours", nameof(other));
            }
            
            return adjacentEdges[other];
        }
        
        /// <summary>
        /// Determines the set of vertices shared by this triangle and the specified one.
        /// </summary>
        /// <param name="other">The other triangle to compare this triangle with</param>
        /// <returns></returns>
        public IEnumerable<Vector> GetCommonVerticesWith(Triangle other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            
            return vertices.Intersect(other.vertices);
        }
        
        // Source: http://www.blackpawn.com/texts/pointinpoly/default.html
        public bool ContainsPoint(IVector point)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }
            
            // Compute vectors
            IVector v0 = C.Minus(A); // v0 = C - A
            IVector v1 = B.Minus(A); // v1 = B - A
            IVector v2 = point.Minus(A); // v2 = P - A

            // Lower bounds taking into consideration vector equality check parameters
            double borderWidth = VectorEqualityCheck.Tolerance; 
            double abs0 = v0.Length();
            double abs1 = v1.Length();
            double lowU = borderWidth / abs0;
            double lowV = borderWidth / abs1;
            
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
            // The higher bound is increased by the applicable border size for the u and v weights
            return (u > -lowU) && (v > -lowV) && (u + v < 1.0 + lowU * u + lowV * v); // return (u >= 0) && (v >= 0) && (u + v < 1)
        }
        
        public override bool Equals(object other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            
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
            return vertices[0].GetHashCode() ^ vertices[1].GetHashCode() ^ vertices[2].GetHashCode();
        }
               
    }
}