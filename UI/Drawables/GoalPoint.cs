using System.Drawing;

namespace TriangulatedPolygonAStar.UI
{
    /// <summary>
    /// The visual representation of a destination point.
    /// </summary>
    public class GoalPoint : Point
    {
        private static float GoalRadius = 0.08f;
        private static Color Color = Color.Green; 
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GoalPoint"/> class which
        /// draws a goal point to the canvas.
        /// </summary>
        /// <param name="position">The position of this point in absolute coordinate system</param>
        public GoalPoint(IVector position) 
            : base(GoalRadius, Color, position)
        {
        }

    }
}