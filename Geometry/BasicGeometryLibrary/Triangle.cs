using System;
using System.Collections.Generic;
using System.Linq;

namespace TriangulatedPolygonAStar.BasicGeometry
{
    /// <inheritdoc />
    public class Triangle : ITriangle
    {
        private readonly int id;
        private readonly Vector[] vertices;
        private IEnumerable<Triangle> neighbours;

        /// <summary>
        /// Initializes a new instance of <see cref="Triangle"/> class by its three corner points.
        /// No specific order of the points is expected. 
        /// Distorted triangles which has two or more identical corner are not supported.
        /// Triangles that lie between the same points should have identical ids,
        /// otherwise edges acquired from <see cref="GetCommonEdgeWith"/> might produce different hashes for equal edges.
        /// </summary>
        /// <param name="a">The first corner point</param>
        /// <param name="b">The second corner point</param>
        /// <param name="c">The third corner point</param>
        /// <param name="id">The unique identifier of the triangle</param>
        public Triangle(Vector a, Vector b, Vector c, int id)
        {
            CheckForNullArgument(a, nameof(a));
            CheckForNullArgument(b, nameof(b));
            CheckForNullArgument(c, nameof(c));
            if (a.Equals(b) || a.Equals(c) || b.Equals(c))
            {
                throw new ArgumentOutOfRangeException("One or more of the specified vertices overlap each other");
            }

            this.id = id;
            vertices = new[] {a, b, c};
            neighbours = Enumerable.Empty<Triangle>();
        }

        /// <summary>
        /// The first corner of the triangle.
        /// </summary>
        public IVector A
        {
            get { return vertices[0]; }
        }

        /// <summary>
        /// The second corner of the triangle.
        /// </summary>
        public IVector B
        {
            get { return vertices[1]; }
        }

        /// <summary>
        /// The third corner of the triangle.
        /// </summary>
        public IVector C
        {
            get { return vertices[2]; }
        }

        /// <summary>
        /// The unique identifier of this triangle.
        /// </summary>
        public int Id
        {
            get { return id; }
        }
        
        /// <inheritdoc />
        public IEnumerable<ITriangle> Neighbours
        {
            get { return neighbours; }
        }

        /// <summary>
        /// Sets the neighbours of this triangle. 
        /// Every neighbour triangle is expected to share exactly two vertices with this one. 
        /// The maximum amount of neighbours is three.
        /// </summary>
        /// <param name="neighbours">The neighbours to be set</param>
        public void SetNeighbours(IEnumerable<Triangle> neighbours)
        {
            CheckForNullArgument(neighbours, nameof(neighbours));
            if (neighbours.Any(item => item == null))
            {
                throw new ArgumentException("One or more of the specified neighbours is null", nameof(neighbours));
            }
            if (neighbours.Count() > 3)
            {
                throw new ArgumentOutOfRangeException(
                    "The amount of specified neighbours exceed the maximual amount of three", nameof(neighbours));
            }
            if (neighbours.Any(triangle => triangle.GetCommonVerticesWith(this).Count() != 2))
            {
                throw new ArgumentException(
                    "One or more of the specified triangles are not adjacent with this one", nameof(neighbours));
            }
            
            this.neighbours = neighbours;
        }

        /// <inheritdoc />
        public IEdge GetCommonEdgeWith(ITriangle other)
        {
            CheckForNullArgument(other, nameof(other));
            if (!neighbours.Any(triangle => triangle.Equals(other)))
            {
                throw new ArgumentException("The specified triangle cannot be found among the neighbours",
                    nameof(other));
            }
            
            var neighbour = neighbours.First(triangle => triangle.Equals(other));
            return new Edge(this, neighbour);
        }

        /// <summary>
        /// Determines the set of vertices shared by this triangle and the specified one.
        /// If no shared vertex exists, an empty set is returned.
        /// </summary>
        /// <param name="other">The other triangle to compare this triangle with</param>
        /// <returns>The set of common vertices</returns>
        public IEnumerable<Vector> GetCommonVerticesWith(Triangle other)
        {
            CheckForNullArgument(other, nameof(other));
            
            return vertices.Where(point => other.vertices.Any(otherVertex => otherVertex.Equals(point)));
        }

        // Source: http://www.blackpawn.com/texts/pointinpoly/default.html
        /// <inheritdoc />
        public bool ContainsPoint(IVector point)
        {
            CheckForNullArgument(point, nameof(point));

            // Compute vectors
            IVector v0 = C.Minus(A); // v0 = C - A
            IVector v1 = B.Minus(A); // v1 = B - A
            IVector v2 = point.Minus(A); // v2 = P - A

            // Lower bounds taking into consideration vector equality check parameters
            double boundaryWidth = VectorEqualityCheck.Tolerance;
            double mgn0 = v0.Length();
            double mgn1 = v1.Length();
            double lowU = boundaryWidth / mgn0;
            double lowV = boundaryWidth / mgn1;

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
            return (u > -lowU) && (v > -lowV) &&
                   (u + v < 1.0 + lowU * u + lowV * v); // return (u >= 0) && (v >= 0) && (u + v < 1)
        }

        /// <inheritdoc cref="ITriangle.Equals(object)" />
        public override bool Equals(object other)
        {
            Triangle otherTriangle = other as Triangle;
            if (otherTriangle != null)
            {
                return GetCommonVerticesWith(otherTriangle).Count() == 3;
            }
            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance. 
        /// <see cref="GetHashCode"/> provides a useful distribution only if the uniqueness requirement holds in the system
        /// described in 
        /// <see cref="Triangle(TriangulatedPolygonAStar.BasicGeometry.Vector,TriangulatedPolygonAStar.BasicGeometry.Vector,TriangulatedPolygonAStar.BasicGeometry.Vector,int)"/>.
        /// </summary>
        /// <returns>An integer value that specifies a hash value for this instance</returns>
        public override int GetHashCode()
        {
            return id;
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