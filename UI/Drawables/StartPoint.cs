using System.Drawing;

namespace TriangulatedPolygonAStar.UI
{
    /// <summary>
    /// The visual representation of a start point.
    /// </summary>
    public class StartPoint : Point
    {
        private static float StartRadius = 0.08f;
        private static Color Color = Color.Blue; 
        
        /// <summary>
        /// Initializes a new instance of the <see cref="StartPoint"/> class which
        /// draws a start point on the canvas.
        /// </summary>
        /// <param name="position">The position of this point in an absolute coordinate system</param>
        public StartPoint(IVector position)
        : base(StartRadius, Color, position)
        {
        }
    }
}