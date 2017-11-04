using System.Collections.Generic;
using System.Linq;
using TriangulatedPolygonAStar.BasicGeometry;

namespace TriangulatedPolygonAStar.UI.Resources
{
    public static partial class TriangleMaps
    {
        public static IEnumerable<Triangle> TrianglesOfPolygonWithOneHole { get; private set; }

        public static IEnumerable<Triangle> TrianglesOfPolygonWithTwoHoles { get; private set; }

        static TriangleMaps()
        {
            TrianglesOfPolygonWithOneHole = GetTrianglesOfPolygonWithOneHole();
            SetAdjacencySettingsBetween(TrianglesOfPolygonWithOneHole);
            TrianglesOfPolygonWithTwoHoles = GetTrianglesOfPolygonWithTwoHoles();
            SetAdjacencySettingsBetween(TrianglesOfPolygonWithTwoHoles);
        }

        private static void SetAdjacencySettingsBetween(IEnumerable<Triangle> triangles)
        {
            foreach (var triangle in triangles)
            {
                var neighbours = triangles.Where(other => triangle.GetCommonVerticesWith(other).Count() == 2).ToArray();
                triangle.SetNeighbours(neighbours);
            }
        }
    }
}