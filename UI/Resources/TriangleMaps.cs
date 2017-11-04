using System.Collections.Generic;
using System.Linq;
using TriangulatedPolygonAStar.BasicGeometry;

namespace TriangulatedPolygonAStar.UI.Resources
{
    /// <summary>
    /// Maps of triangulated polygons.
    /// </summary>
    public static partial class TriangleMaps
    {
        /// <summary>
        /// A map of a triangulated polygon with one polygon hole.
        /// </summary>
        public static IEnumerable<Triangle> TrianglesOfPolygonWithOneHole { get; private set; }
        
        /// <summary>
        /// A map of a triangulated polygon with two polygon holes.
        /// </summary>
        public static IEnumerable<Triangle> TrianglesOfPolygonWithTwoHoles { get; private set; }
        
        static TriangleMaps()
        {
            TrianglesOfPolygonWithOneHole = GetTrianglesOfPolygonWithOneHole();
            SetNeighboursForAll(TrianglesOfPolygonWithOneHole);
            TrianglesOfPolygonWithTwoHoles = GetTrianglesOfPolygonWithTwoHoles();
            SetNeighboursForAll(TrianglesOfPolygonWithTwoHoles);
        }

        private static void SetNeighboursForAll(IEnumerable<Triangle> triangles)
        {
            foreach (var triangle in triangles)
            {
                var neighbours = triangles.Where(other => triangle.GetCommonVerticesWith(other).Count() == 2).ToArray();
                triangle.SetNeighbours(neighbours);
            }
        }
    }
}