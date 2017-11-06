using System;
using System.Drawing;
using TriangulatedPolygonAStar.BasicGeometry;

namespace TriangulatedPolygonAStar.UI
{
    /// <summary>
    /// Draws signs and instructions.
    /// </summary>
    public class Legend : IDrawable
    {
        private static Color EdgeColor = Color.DarkGray;
        private static float EdgeWidth = 0.01f;
        private static Pen EdgePen = new Pen(EdgeColor, EdgeWidth);
        private static Brush FillBrush = new SolidBrush(Color.White);
        private static float Width = 54.0f;
        private static float Height = 14.0f;
        private static Color TextColor = Color.Black;
        private static float TitleFontSize = 1.8f;
        private static float CaptionFontSize = 1.3f;
        private static Brush CaptionBrush = new SolidBrush(TextColor);

        private static string LegendTitle = "Legend";
        private static string InstructionsTitle = "Instructions";
        private static string StartCaption = "Start";
        private static string GoalCaption = "Goal";
        private static string PathCaption = "Path";
        private static string[] Instructions = { "Left-click: Add goal","Right-click: Remove goal","Left-down + Move: Relocate start or goal" };

        private static Font TitleFont = new Font(FontFamily.GenericSansSerif, TitleFontSize, FontStyle.Bold | FontStyle.Italic);
        private static Font CaptionFont = new Font(FontFamily.GenericSansSerif, CaptionFontSize, FontStyle.Italic);
        private static IVector IconBoxWidth = new Vector(2.5, 0.0);
        private static IVector IconBoxHeight = new Vector(0.0, 2.5);
        private static IVector VerticalTranslationOfInstruction = new Vector(0.0, 2.5);
        
        private IVector topLeft;
        private IVector legendTitleLocation;
        private IVector instructionTitleLocation;
        private StartPoint startPointToDraw;
        private GoalPoint goalPointToDraw;
        private PolyLine pathIcon;
        private IVector instructionLocation;
        private IVector pathCaptionLocation;
        
        /// <summary>
        /// Instantiates a new instance of <see cref="Legend"/> class which
        /// displays the meaning of signs and instructions. 
        /// </summary>
        /// <param name="topLeftAbsolutePosition">The position in the world where the legend need to be drawn</param>
        public Legend(IVector topLeftAbsolutePosition)
        {
            topLeft = topLeftAbsolutePosition;
            legendTitleLocation = topLeft.Plus(IconBoxWidth.Times(0.75)).Plus(IconBoxHeight.Times(0.5));
            var startPointPosition = topLeft.Plus(IconBoxWidth.Times(1.25)).Plus(IconBoxHeight.Times(2.5));
            var goalPointPosition = topLeft.Plus(IconBoxWidth.Times(1.25)).Plus(IconBoxHeight.Times(3.5));
            var pathIconLeftEndpoint = topLeft.Plus(IconBoxWidth.Times(1.0)).Plus(IconBoxHeight.Times(4.5));
            pathCaptionLocation = topLeft.Plus(IconBoxWidth.Times(2.25)).Plus(IconBoxHeight.Times(4.5));
            instructionTitleLocation = topLeft.Plus(IconBoxWidth.Times(6.0)).Plus(IconBoxHeight.Times(0.5));
            instructionLocation = topLeft.Plus(IconBoxWidth.Times(6.0)).Plus(IconBoxHeight.Times(2.5));
            
            startPointToDraw = new StartPoint(startPointPosition);
            goalPointToDraw = new GoalPoint(goalPointPosition);
            var pathIconRightEndpoint = pathIconLeftEndpoint.Plus(IconBoxWidth.Times(0.5));
            var displayLengthOfPathIcon = false;
            pathIcon = new PolyLine(new[]{ pathIconLeftEndpoint, pathIconRightEndpoint }, displayLengthOfPathIcon );

            var topLeftF = topLeft.ToPointF();
            var maxX = Math.Max(topLeftF.X, topLeftF.X + Width);
            var minX = Math.Min(topLeftF.X, topLeftF.X + Width);
            var maxY = Math.Max(topLeftF.Y, topLeftF.Y + Height);
            var minY = Math.Min(topLeftF.Y, topLeftF.Y + Height);
            BoundingBoxHigh = new PointF(maxX, maxY);
            BoundingBoxLow = new PointF(minX, minY);
        }
        
        /// <inheritdoc />
        public PointF BoundingBoxHigh { get; private set; }

        /// <inheritdoc />
        public PointF BoundingBoxLow { get; private set; }

        /// <inheritdoc />
        public void Draw(Graphics canvas)
        {
            var topLeftF = topLeft.ToPointF();
            canvas.FillRectangle(FillBrush, topLeftF.X, topLeftF.Y, Width, Height);
            canvas.DrawRectangle(EdgePen, topLeftF.X, topLeftF.Y, Width, Height);
            
            canvas.DrawString(LegendTitle, TitleFont, CaptionBrush, legendTitleLocation.ToPointF());                     
            DrawPointWithCaption(startPointToDraw, StartCaption, canvas);
            DrawPointWithCaption(goalPointToDraw, GoalCaption, canvas);
            pathIcon.Draw(canvas);
            canvas.DrawString(PathCaption, CaptionFont, CaptionBrush, pathCaptionLocation.ToPointF());
            
            canvas.DrawString(InstructionsTitle, TitleFont, CaptionBrush, instructionTitleLocation.ToPointF());
            var instructionLineLocation = instructionLocation; 
            foreach (var instructionLine in Instructions)
            {
                canvas.DrawString(instructionLine, CaptionFont, CaptionBrush, instructionLineLocation.ToPointF());
                instructionLineLocation = instructionLineLocation.Plus(VerticalTranslationOfInstruction);
            }            
        }

        private static void DrawPointWithCaption(Point pointToDraw, String caption, Graphics canvas)
        {
            pointToDraw.Draw(canvas);
            var startCaptionLocation = pointToDraw.Position.Plus(IconBoxWidth).ToPointF();
            canvas.DrawString(caption, CaptionFont, CaptionBrush, startCaptionLocation);
        }

    }
}