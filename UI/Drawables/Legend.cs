using System;
using System.Collections.Generic;
using System.Drawing;

namespace TriangulatedPolygonAStar.UI
{
    /// <summary>
    /// Draws signs and instructions.
    /// </summary>
    public class Legend : IOverlay
    {
        private static readonly int WidthInPx;
        private static readonly int HeightInPx;
        private static readonly int PaddingLeftRightInPx;
        private static readonly int PaddingTopBottomInPx;
        private static readonly int IconBoxWidthHeight;
        private static readonly Pen EdgePen;
        private static readonly Brush FillBrush;
        private static readonly Brush CaptionBrush;
        private static readonly Font TitleFont;
        private static readonly Font CaptionFont;
        private static readonly int TitleFontSizeInPx;
        private static readonly string LegendTitle;
        private static readonly string InstructionsTitle;
        private static readonly string StartCaption;
        private static readonly string GoalCaption;
        private static readonly string PathCaption;
        private static readonly IEnumerable<string> Instructions;
        private static readonly Brush GoalBrush;
        private static readonly Brush StartBrush;
        private static readonly Pen PathPen;
        private static readonly int InstructionsAlignmentFromLeftInPx;
        private readonly Rectangle container;
        private readonly Rectangle legendTitleBox;
        private readonly Rectangle startIconBox;
        private readonly Rectangle goalIconBox;
        private readonly Rectangle pathIconBox;
        private readonly Rectangle instructionTitleBox;
        private readonly Rectangle firstInstructionBox;
        private readonly StringFormat captionStringFormat;

        /// <summary>
        /// Instantiates a new instance of <see cref="Legend"/> class which
        /// displays the meaning of signs and instructions. 
        /// </summary>
        /// <param name="topCoordinateOnCanvas">The vertical coordinate of the legend on the canvas</param>
        /// <param name="leftCoordinateOnCanvas">The horizontal coordinate of the legend on the canvas</param>
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
        
        static Legend()
        {
            WidthInPx = 300;
            HeightInPx = 130;
            PaddingLeftRightInPx = 16;
            PaddingTopBottomInPx = 10;
            IconBoxWidthHeight = 20;
            var edgeWidthInPx = 1;
            EdgePen = new Pen(Color.DarkGray, edgeWidthInPx);
        
            FillBrush = new SolidBrush(Color.White);
            CaptionBrush = new SolidBrush(Color.Black);
            TitleFontSizeInPx = 14;
            TitleFont = new Font(FontFamily.GenericSansSerif, TitleFontSizeInPx, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Pixel);
            var captionFontSizeInPx = 11;
            CaptionFont = new Font(FontFamily.GenericSansSerif, captionFontSizeInPx, FontStyle.Italic, GraphicsUnit.Pixel);
        
            LegendTitle = "Legend";
            InstructionsTitle = "Interactions";
            StartCaption = "Start";
            GoalCaption = "Goal";
            PathCaption = "Path";
            Instructions = new[] {"Left-click: Add","Right-click: Remove","Left-down + Move: Relocate", "Space: Show / Hide metadata"};

            GoalBrush = Brushes.Green;
            StartBrush = Brushes.Blue;
            PathPen = Pens.Green;
            InstructionsAlignmentFromLeftInPx = 100;
        }
    }
}