using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TriangulatedPolygonAStar.UI
{
    /// <summary>
    /// An overlay layer that display metadata in the bottom-right corner of the canvas.
    /// </summary>
    public class MetaDisplay : IOverlay
    {
        private static readonly Font CaptionFont;
        private static readonly Brush CaptionBrush;
        private readonly Point offset;
        private readonly IEnumerable<ILocationMarker> goals;
        private readonly ILocationMarker start;
        private DrawableTriangle selectedTriangle;
        private IEnumerable<IVector> path;
        
        /// <summary>
        /// Initializes a new instance of <see cref="MetaDisplay" /> class which 
        /// displays metadata in the bottom-right corner of the canvas.
        /// </summary>
        /// <param name="start">The start point to display</param>
        /// <param name="goals">The set of goal points to display</param>
        /// <param name="distanceFromRightInPx">The horizontal offset from the edge of the canvas</param>
        /// <param name="distanceFromBottomInPx">The vertical offset from the edge of the canvas</param>
        public MetaDisplay(ILocationMarker start, IEnumerable<ILocationMarker> goals, int distanceFromRightInPx, int distanceFromBottomInPx) // TODO add start and maybe path
        {
            offset = new Point(-distanceFromRightInPx, -distanceFromBottomInPx);
            this.start = start;
            this.goals = goals;
            this.selectedTriangle = null;
            path = null;
        }

        /// <summary>
        /// Sets the triangle whose metadata is displayed.
        /// </summary>
        /// <param name="triangle">The triangle to display information about</param>
        public void SetSelectedTriangle(DrawableTriangle triangle)
        {
            this.selectedTriangle = triangle;
        }

        /// <summary>
        /// Removes the triangle whose details should be shown.
        /// </summary>
        public void ClearSelectedTriangle()
        {
            this.selectedTriangle = null;
        }

        /// <summary>
        /// Sets the points of the path to display.
        /// </summary>
        /// <param name="path">The path to display</param>
        public void SetPath(IEnumerable<IVector> path)
        {
            this.path = path;
        }

        /// <summary>
        /// Clears the meta-information about the path.
        /// </summary>
        public void ClearPath()
        {
            this.path = null;
        }

        /// <inheritdoc />
        public void Draw(Graphics canvas)
        {
            var canvasSize = canvas.ClipBounds;
            var builder = new StringBuilder();

            if (selectedTriangle != null)
            {
                builder.AppendFormat("{0,-22}", selectedTriangle.DisplayName).AppendLine();
                var traversionCount = selectedTriangle.Traversions.Count();
                builder.Append("traversed: ").Append(traversionCount).AppendLine(" time(s)");
                builder.AppendLine();

                foreach (var traversion in selectedTriangle.Traversions)
                {
                    builder.Append("       g_min: ").AppendFormat("{0:0.00}", traversion.ShortestPathToEdgeLength).AppendLine();
                    builder.Append("       g_max: ").AppendFormat("{0:0.00}", traversion.LongestPathToEdgeLength).AppendLine();
                    builder.Append("       f_min: ").AppendFormat("{0:0.00}", traversion.EstimatedMinimalCost).AppendLine();
                    builder.AppendLine();   
                }                
            }
            
            builder.AppendLine();
            builder.AppendLine("path:");
            if (path != null)
            {
                foreach (var point in path)
                {
                    builder.AppendFormat("       {0:0.00}, {1:0.00}", point.X, point.Y).AppendLine();
                }
            }
            
            builder.AppendLine();
            builder.AppendFormat("{0,-22}", "start:").AppendLine();
            builder.AppendFormat("       {0:0.00}, {1:0.00}", start.CurrentLocation.X, start.CurrentLocation.Y).AppendLine();
            builder.AppendLine();
            
            builder.AppendLine("goals:");
            foreach (var goal in goals)
            {
                builder.AppendFormat("       {0:0.00}, {1:0.00}", goal.CurrentLocation.X, goal.CurrentLocation.Y).AppendLine();
            }

            var caption = builder.ToString();
            var captionSize = canvas.MeasureString(caption, CaptionFont, canvas.ClipBounds.Size);
            canvas.DrawString(caption, CaptionFont, CaptionBrush, canvasSize.Right - captionSize.Width + offset.X, canvasSize.Bottom - captionSize.Height + offset.Y);
        }
        
        static MetaDisplay()
        {
            int captionFontSizeInPx = 11;
            CaptionFont = new Font(FontFamily.GenericMonospace, captionFontSizeInPx, GraphicsUnit.Pixel);
            CaptionBrush = Brushes.Black;
        }
    }
}