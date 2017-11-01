using System.Drawing;

namespace TriangulatedPolygonAStar.UI
{
    public class StartPoint : Point
    {
        private static float Radius = 0.08f;
        private static Color Color = Color.Blue;

        public StartPoint(IVector position)
            : base(Radius, Color, position)
        {
        }
    }
}