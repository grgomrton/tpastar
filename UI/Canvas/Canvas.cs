using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Linq;
using TriangulatedPolygonAStar.BasicGeometry;

namespace TriangulatedPolygonAStar.UI
{
    /// <summary>
    /// A canvas user control that can be used for displaying multiple <see cref="IDrawable"/> instances.
    /// </summary>
    public partial class Canvas : UserControl
    {
        private static float TopBottomPaddingInPercent = 10;
        private static float LeftRightPaddingInPercent = 10;
        
        private List<IDrawable> drawables;
        private float translationXBeforeScaling;
        private float translationYBeforeScaling;
        private float magnification;

        /// <summary>
        /// Initializes a new instance of the <see cref="Canvas"/> class which can be used to display
        /// drawables.
        /// </summary>
        public Canvas()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            drawables = new List<IDrawable>();
            InitializeComponent();
        }

        /// <summary>
        /// Adds a drawable object to the list of objects to draw. 
        /// Every drawable should use identical coordinate system for drawing.
        /// </summary>
        /// <param name="drawable">The drawable to add</param>
        public void AddDrawable(IDrawable drawable)
        {
            drawables.Add(drawable);
            AutoScale();
            Invalidate();
        }

        /// <summary>
        /// Removes the specified drawable from the list.
        /// </summary>
        /// <param name="drawable">The drawable to remove</param>
        public void RemoveDrawable(IDrawable drawable)
        {
            if (drawables.Contains(drawable))
            {
                drawables.Remove(drawable);
            }
            AutoScale();
            Invalidate();
        }
        
        /// <summary>
        /// Removes every drawable objects.
        /// </summary>
        public void ClearDrawables()
        {
            drawables.Clear();
            AutoScale();
            Invalidate();
        }
        
        /// <summary>
        /// Gets the absolute position of the specified coordinates on this canvas.
        /// </summary>
        /// <param name="x">The x coordinate on the canvas</param>
        /// <param name="y">The y coordinate on the canvas</param>
        /// <returns>The point that represent the specified canvas point in absolute coordinate system</returns>
        public IVector GetAbsolutePosition(int x, int y)
        {
            return new Vector(x / magnification - translationXBeforeScaling, y / magnification - translationYBeforeScaling);
        }

        protected override void OnPaint(PaintEventArgs eventDetails)
        {            
            eventDetails.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Graphics canvas = eventDetails.Graphics;

            Matrix mscale = new Matrix();
            mscale.Translate(translationXBeforeScaling, translationYBeforeScaling, MatrixOrder.Prepend);
            mscale.Scale(magnification, magnification, MatrixOrder.Append);
            canvas.Transform = mscale;
            
            foreach (var drawable in drawables)
            {
                try
                {
                    drawable.Draw(canvas);
                }
                catch (Exception e)
                {
                    MessageBox.Show("An item failed to draw: \n" + e);
                }
            }
        }

        private void AutoScale() {
            var centroidX = 0.0f;
            var centroidY = 0.0f;
            var boundingBoxWidth = 1.0f;
            var boundingBoxHeight = 1.0f;
            if (drawables.Count > 0)
            {
                var minX = drawables.Select(drawable => drawable.BoundingBoxLow.X).Min();
                var minY = drawables.Select(drawable => drawable.BoundingBoxLow.Y).Min();
                var maxX = drawables.Select(drawable => drawable.BoundingBoxHigh.X).Max();
                var maxY = drawables.Select(drawable => drawable.BoundingBoxHigh.Y).Max();

                boundingBoxWidth = maxX - minX;
                boundingBoxHeight = maxY - minY;
                centroidX = minX + boundingBoxWidth / 2.0f;
                centroidY = minY + boundingBoxHeight / 2.0f;
            }

            boundingBoxWidth *= 1 + LeftRightPaddingInPercent / 100.0f;
            boundingBoxHeight *= 1 + TopBottomPaddingInPercent / 100.0f;
            var scaleX = this.Width / boundingBoxWidth;
            var scaleY = this.Height / boundingBoxHeight;
            
            magnification = Math.Min(scaleX, scaleY);
            translationXBeforeScaling = this.Width / magnification / 2.0f - centroidX;
            translationYBeforeScaling = this.Height / magnification / 2.0f - centroidY;
        }
        
        private void CanvasOnClientSizeChanged(object sender, EventArgs e)
        {
            AutoScale();
            Invalidate();
        }

    }
}