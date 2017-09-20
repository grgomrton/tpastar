using System.Collections.Generic;
using System.Linq;
using TriangulatedPolygonAStar;

namespace TPAStarGUI
{
    public static class PathFinderExtensions
    {
        public static double Length(this IEnumerable<IVector> curve)
        {
            return curve.Zip(curve.Skip(1), (v1, v2) => v1.DistanceFrom(v2)).Sum();
        }
    }
}