using System.Drawing;

namespace TriangulatedPolygonAStar.UI
{
    /// <summary>
    /// An object which can draw itself on a canvas without considering its' current 
    /// translation and magnification.
    /// </summary>
    public interface IOverlay
    {
        /// <summary>
        /// Draws the visual representation of the object to the specified canvas.
        /// </summary>
        /// <param name="canvas">The canvas to draw onto</param>
        void Draw(Graphics canvas);
    }
}