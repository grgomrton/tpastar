using System.Drawing;

namespace TriangulatedPolygonAStar.UI
{
    /// <summary>
    /// An object which can be drawn on a <see cref="Canvas "/>.
    /// </summary>
    public interface IDrawable
    {
        /// <summary>
        /// Draws the visual representation of the object to the specified canvas.
        /// </summary>
        /// <param name="canvas">The canvas to draw onto</param>
        void Draw(Graphics canvas);
        
        /// <summary>
        /// The highest coordinates of the rectangle in which the entire object fits.
        /// </summary>
        PointF BoundingBoxHigh { get; }
        
        /// <summary>
        /// The lowest coordinates of the rectangle in which the entire object fits.
        /// </summary>
        PointF BoundingBoxLow { get; }
    }
}