using System.Drawing;

namespace TriangulatedPolygonAStar.UI
{
    /// <summary>
    /// An overlay which displays the short name of the algoritm.
    /// </summary>
    public class Title : IOverlay
    {
        private static readonly string Text = "tpastar";
        private static readonly Brush TitleBrush;
        private static readonly Font TitleFont;
        private readonly System.Drawing.Point offset;

        /// <summary>
        /// Instantiates a new instance of <see cref="Title"/> class, which displays
        /// the short name of the algorithm.
        /// </summary>
        /// <param name="distanceFromLeftInPx">The distance from the left edge of the canvas</param>
        /// <param name="distanceFromTopInPx">The distance from the top edge of the canvas</param>
        public Title(int distanceFromLeftInPx, int distanceFromTopInPx)
        {
                offset = new System.Drawing.Point(distanceFromLeftInPx, distanceFromTopInPx);
        }

        static Title()
        {
            var titleSizeInPx = 50;
            TitleBrush = Brushes.Black;
            TitleFont = new Font(FontFamily.GenericSansSerif, titleSizeInPx, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Pixel);
        }

        /// <inheritdoc />
        public void Draw(Graphics canvas)
        {
            canvas.DrawString(Text, TitleFont, TitleBrush, offset.X, offset.Y);
        }
    }
}