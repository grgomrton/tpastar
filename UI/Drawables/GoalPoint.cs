using System.Drawing;

namespace TriangulatedPolygonAStar.UI
{
    public class GoalPoint : Point
    {
        private static float Radius = 0.08f;
        private static Color FillColor = Color.Green; 
        
        public GoalPoint(IVector position) 
            : base(Radius, FillColor, position)
        {
        }

    }
}