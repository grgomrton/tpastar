using System;
using System.Drawing;

namespace TriangulatedPolygonAStar.UI
{
    /// <summary>
    /// Draws signs and instructions.
    /// </summary>
    public class Legend : IOverlay
    {
        private static int WidthInPx = 280;
        private static int HeightInPx = 110;
        private static int PaddingLeftRightInPx = 16;
        private static int PaddingTopBottomInPx = 10;
        private static int IconBoxWidthHeight = 20;
        private static int EdgeWidthInPx = 1;
        private static Pen EdgePen = new Pen(Color.DarkGray, EdgeWidthInPx);
        private static Brush FillBrush = new SolidBrush(Color.White);
        
        private static int TitleFontSizeInPx = 14;
        private static int CaptionFontSizeInPx = 11;
        private static Brush CaptionBrush = new SolidBrush(Color.Black);
        private static Font TitleFont = new Font(FontFamily.GenericSansSerif, TitleFontSizeInPx, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Pixel);
        private static Font CaptionFont = new Font(FontFamily.GenericSansSerif, CaptionFontSizeInPx, FontStyle.Italic, GraphicsUnit.Pixel);
        
        private static string LegendTitle = "Legend";
        private static string InstructionsTitle = "Interactions";
        private static string StartCaption = "Start";
        private static string GoalCaption = "Goal";
        private static string PathCaption = "Path";
        private static string[] Instructions = { "Left-click: Add","Right-click: Remove","Left-down + Move: Relocate" };

        private static Brush GoalBrush = new SolidBrush(Color.Green);
        private static Brush StartBrush = new SolidBrush(Color.Blue);
        private static Pen PathPen = new Pen(Color.Green);
        private static int InstructionsAlignmentFromLeftInPx = 100;
        
        private Rectangle container;
        private Rectangle legendTitleBox;
        private Rectangle startIconBox;
        private Rectangle goalIconBox;
        private Rectangle pathIconBox;
        private Rectangle instructionTitleBox;
        private Rectangle firstInstructionBox;
        private StringFormat captionStringFormat;

        /// <summary>
        /// Instantiates a new instance of <see cref="Legend"/> class which
        /// displays the meaning of signs and instructions. 
        /// </summary>
        /// <param name="topCoordinateOnCanvas">The vertical coordinate of the on the canvas</param>
        /// <param name="leftCoordinateOnCanvas">The horizontal coordinate of the on the canvas</param>
        public Legend(int topCoordinateOnCanvas, int leftCoordinateOnCanvas)
        {
            container = new Rectangle(topCoordinateOnCanvas, leftCoordinateOnCanvas, WidthInPx, HeightInPx);
            
            captionStringFormat = new StringFormat(StringFormat.GenericTypographic);
            captionStringFormat.Alignment = StringAlignment.Near;
            captionStringFormat.LineAlignment = StringAlignment.Center;
            
            legendTitleBox = new Rectangle(container.X + PaddingLeftRightInPx, container.Y + PaddingTopBottomInPx, WidthInPx, 2 * TitleFontSizeInPx);
            startIconBox = new Rectangle(legendTitleBox.Left, legendTitleBox.Bottom, 
                IconBoxWidthHeight, IconBoxWidthHeight);
            goalIconBox = new Rectangle(legendTitleBox.Left, startIconBox.Bottom, IconBoxWidthHeight, IconBoxWidthHeight);
            pathIconBox = new Rectangle(legendTitleBox.Left, goalIconBox.Bottom, IconBoxWidthHeight, IconBoxWidthHeight);
            instructionTitleBox = new Rectangle(container.X + InstructionsAlignmentFromLeftInPx, container.Y + PaddingTopBottomInPx, WidthInPx, 2 * TitleFontSizeInPx);
            firstInstructionBox = new Rectangle(instructionTitleBox.Left, instructionTitleBox.Bottom, WidthInPx, IconBoxWidthHeight);
        }

        /// <inheritdoc />
        public void Draw(Graphics canvas)
        {
            canvas.FillRectangle(FillBrush, container);
            canvas.DrawRectangle(EdgePen, container);
            canvas.DrawString(LegendTitle, TitleFont, CaptionBrush, legendTitleBox);
            //canvas.DrawRectangle(Pens.BlueViolet, startIconBox);
            DrawPointIcon(StartBrush, startIconBox, canvas);
            DrawCaption(StartCaption, startIconBox, captionStringFormat, canvas);
            
            //canvas.DrawRectangle(Pens.BlueViolet, startCaptionBox);
            DrawPointIcon(GoalBrush, goalIconBox, canvas);
            DrawCaption(GoalCaption, goalIconBox, captionStringFormat, canvas);
            
            DrawPathIcon(PathPen, pathIconBox, canvas);
            DrawCaption(PathCaption, pathIconBox, captionStringFormat, canvas);

            canvas.DrawString(InstructionsTitle, TitleFont, CaptionBrush, instructionTitleBox);
            var instructionBox = firstInstructionBox;
            foreach (var instruction in Instructions)
            {
                //canvas.DrawRectangle(Pens.BlueViolet, instructionBox);
                canvas.DrawString(instruction, CaptionFont, CaptionBrush, instructionBox, captionStringFormat);
                instructionBox = new Rectangle(instructionBox.Left, instructionBox.Top + instructionBox.Height, instructionBox.Width, instructionBox.Height);
            }
        }

        private static void DrawPointIcon(Brush brush, Rectangle iconBox, Graphics canvas)
        {
            var centerPoint = new PointF(iconBox.Left + iconBox.Width * 0.5f, iconBox.Top + iconBox.Height * 0.5f);
            var radius = 0.25f * Math.Min(iconBox.Width, iconBox.Height);
            canvas.FillEllipse(brush, centerPoint.X - radius, centerPoint.Y - radius, 2*radius, 2*radius);
        }
        
        private static void DrawPathIcon(Pen pen, Rectangle iconBox, Graphics canvas)
        {
            var left = new PointF(iconBox.Left + iconBox.Width * 0.2f, iconBox.Top + iconBox.Height * 0.5f);
            var right = new PointF(iconBox.Right - iconBox.Width * 0.2f, iconBox.Top + iconBox.Height * 0.5f);
            canvas.DrawLine(pen, left.X, left.Y, right.X, right.Y);
        }

        private static void DrawCaption(string caption, Rectangle iconBox, StringFormat stringFormat, Graphics canvas)
        {
            var captionBox = new Rectangle(iconBox.Right, iconBox.Top, WidthInPx, IconBoxWidthHeight);
            canvas.DrawString(caption, CaptionFont, CaptionBrush, captionBox, stringFormat);
        }

    }
}