using System;
using System.Drawing;
using TriangulatedPolygonAStar.Geometry;

namespace TPAStarGUI
{
    internal static class VectorConverter
    {
        internal static PointF ToPointF(IVector source)
        {
            float x = Convert.ToSingle(source.X);
            float y = Convert.ToSingle(source.Y);
            return new PointF(x, y);
        }
    }
}