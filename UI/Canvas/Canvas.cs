/**
 * Copyright 2017 Márton Gergó
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
        private List<IOverlay> overlays;
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
            overlays = new List<IOverlay>();
            InitializeComponent();
        }

        /// <summary>
        /// Adds a drawable object to the list of objects to draw if it is not already in the list.
        /// Every drawable should use identical coordinate system for drawing.
        /// </summary>
        /// <param name="drawable">The drawable to add</param>
        public void AddDrawable(IDrawable drawable)
        {
            if (!drawables.Contains(drawable))
            {
                drawables.Add(drawable);
                Invalidate();
            }
        }

        /// <summary>
        /// Indicates, whether the specified drawable is currently drawn.
        /// </summary>
        /// <param name="drawable">To drawable to check</param>
        public bool Draws(IDrawable drawable)
        {
            return drawables.Contains(drawable);
        }
        
        /// <summary>
        /// Indicates, whether the specified overlay is currently drawn.
        /// </summary>
        /// <param name="overlay">The overlay layer to check.</param>
        public bool Draws(IOverlay overlay)
        {
            return overlays.Contains(overlay);
        }

        /// <summary>
        /// Removes the specified drawable from the drawn items if it was drawn.
        /// </summary>
        /// <param name="drawable">The drawable to remove</param>
        public void RemoveDrawable(IDrawable drawable)
        {
            if (drawables.Contains(drawable))
            {
                drawables.Remove(drawable);
                Invalidate();
            }
        }
        
        /// <summary>
        /// Removes every drawable objects.
        /// </summary>
        public void ClearDrawables()
        {
            drawables.Clear();
            Invalidate();
        }

        /// <summary>
        /// Adds an overlay layer to the canvas if it is not already in the list.
        /// </summary>
        /// <param name="overlay">An overlay which will not be scaled or translated during update</param>
        public void AddOverlay(IOverlay overlay)
        {
            if (!overlays.Contains(overlay))
            {
                overlays.Add(overlay);   
                Invalidate();
            }
        }

        /// <summary>
        /// Removes the specified drawable from the list if it is contained.
        /// </summary>
        /// <param name="drawable">The overlay to remove</param>
        public void RemoveOverlay(IOverlay overlay)
        {
            if (overlays.Contains(overlay))
            {
                overlays.Remove(overlay);   
                Invalidate();
            }
        }

        /// <summary>
        /// Aligns the translation and magnification settings in a way
        /// that every drawable fit the canvas.
        /// </summary>
        public void ScaleToFit()
        {
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
            canvas.Transform = new Matrix();
            foreach (var overlay in overlays)
            {
                try
                {
                    overlay.Draw(canvas);
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