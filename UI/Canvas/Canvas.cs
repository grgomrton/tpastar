using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using TriangulatedPolygonAStar.BasicGeometry;

namespace TriangulatedPolygonAStar.UI
{
    /// <summary>
    /// A canvas user control that can be used for displaying multiple <see cref="IDrawable"/> instances.
    /// </summary>
    public partial class Canvas : UserControl
    {
        private List<IDrawable> drawables;
        
        private double displayedObjectWidth; 
        private double displayedObjectHeight;
        private float magnify;

        /// <summary>
        /// Initializes a new instance of the <see cref="Canvas"/> class which can be used to display
        /// drawables.
        /// </summary>
        public Canvas()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            drawables = new List<IDrawable>();

            displayedObjectWidth = 1.0;
            displayedObjectHeight = 1.0;
            UpdateMagnify();
            
            InitializeComponent();
        }

        /// <summary>
        /// Updates the magnify setting that the displayed object fills the canvas.
        /// </summary>
        private void UpdateMagnify() {
            double mx = this.Width / displayedObjectWidth;
            double my = this.Height / displayedObjectHeight;
            magnify = Convert.ToSingle(Math.Min(mx, my));
        }

        /// <summary>
        /// Adds a drawable object to the list of objects to draw. 
        /// Every drawable should use identical coordinate system for drawing.
        /// </summary>
        /// <param name="drawable">The drawable to add</param>
        public void AddDrawable(IDrawable drawable)
        {
            drawables.Add(drawable);
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
        }
        
        /// <summary>
        /// Removes every drawable objects.
        /// </summary>
        public void ClearDrawables()
        {
            drawables.Clear();
        }
        
        /// <summary>
        /// Gets or sets the width of the displayed object.
        /// Also sets the zoom property of this canvas accordingly.
        /// </summary>
        public double DisplayedObjectWidth
        {
            get { return displayedObjectWidth; }
            set { 
                displayedObjectWidth = value; 
                UpdateMagnify(); 
            }
        }

        /// <summary>
        /// Gets or sets the height of the displayed object.
        /// Also sets the zoom property of this canvas accordingly.
        /// </summary>
        public double DisplayedObjectHeight
        {
            get { return displayedObjectHeight; }
            set
            {
                displayedObjectHeight = value;
                UpdateMagnify();
            }
        }
        
        /// <summary>
        /// Gets the absolute position of the specified coordinates on this canvas.
        /// </summary>
        /// <param name="x">The x coordinate on the canvas</param>
        /// <param name="y">The y coordinate on the canvas</param>
        /// <returns>The point that represent the specified canvas point in absolute coordinate system</returns>
        public IVector GetAbsolutePosition(int x, int y)
        {
            return new Vector(x / this.magnify, y / this.magnify);
        }

        protected override void OnPaint(PaintEventArgs eventDetails)
        {            
            eventDetails.Graphics.SmoothingMode = SmoothingMode.AntiAlias;            
            Graphics canvas = eventDetails.Graphics;
            Matrix mscale = new Matrix();
            mscale.Scale(magnify, magnify, MatrixOrder.Append);
            canvas.Transform = mscale;

            foreach (var drawable in drawables)
            {
                try
                {
                    drawable.Draw(canvas);
                }
                catch (Exception e)
                {
                    MessageBox.Show("An item failed to draw.\n" + e);
                }
                canvas.Transform = mscale;
            }
        }

        private void CanvasOnClientSizeChanged(object sender, EventArgs e)
        {
            UpdateMagnify();
        }

    }
}