using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using TriangulatedPolygonAStar;
using TriangulatedPolygonAStar.BasicGeometry;

namespace TPAStarGUI
{
    /// <summary>
    /// A canvas user control that can be used for displaying multiple <see cref="DrawMethod"/>s.
    /// </summary>
    public partial class Canvas : UserControl
    {
        List<DrawMethod> drawMethods;
        List<Dictionary<string, Color>> drawMethodColors;
        List<Dictionary<string, float>> drawMethodWidths;

        private double displayedObjectWidth;
        private double displayedObjectHeight;
        private float magnify;

        /// <summary>
        /// A delegate function that can be used to implement drawing functions.
        /// An instance of a <see cref="DrawMethod"/> can be added to a 
        /// <see cref="Canvas"/>.One object can implement multiple draw methods, 
        /// displaying different type of data. Every draw method using the same 
        /// canvas should also use identical coordinate systems.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="colors">The colors used for drawing. See the used draw method for details.</param>
        /// <param name="widths">The widths used for drawing. See the used draw method for details.</param>
        public delegate void DrawMethod(Graphics canvas, Dictionary<string, Color> colors, Dictionary<string, float> widths);
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Canvas"/> class.
        /// </summary>
        public Canvas()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            drawMethods = new List<DrawMethod>();
            drawMethodColors = new List<Dictionary<string, Color>>();
            drawMethodWidths = new List<Dictionary<string, float>>();

            displayedObjectWidth = 1.0;
            displayedObjectHeight = 1.0;
            updateMagnify();
            
            InitializeComponent();
        }

        /// <summary>
        /// Updates the magnify setting that the displayed object fills the canvas.
        /// </summary>
        private void updateMagnify() {
            double mx = this.Width / displayedObjectWidth;
            double my = this.Height / displayedObjectHeight;
            magnify = Convert.ToSingle(Math.Min(mx, my));
        }

        /// <summary>
        /// Adds a draw method delegate to the list of called draw methods. 
        /// Every draw method used on the same canvas should also use identical 
        /// coordinate system for drawing.
        /// </summary>
        /// <param name="m">The draw method delegate.</param>
        /// <param name="l">The hashtable that contains the used colors.</param>
        /// <param name="w">The hashtable that contains the used widths.</param>
        public void AddDrawMethod(DrawMethod m, Dictionary<string, Color> l, Dictionary<string, float> w)
        {
            drawMethods.Add(m);
            drawMethodColors.Add(l);
            drawMethodWidths.Add(w);
        }

        /// <summary>
        /// Removes the specified draw method from the list.
        /// </summary>
        /// <param name="m">The draw method.</param>
        public void RemoveDrawMethod(DrawMethod m)
        {
            if (drawMethods.Contains(m))
            {
                int i = drawMethods.IndexOf(m);
                drawMethods.RemoveAt(i);
                drawMethodColors.RemoveAt(i);
                drawMethodWidths.RemoveAt(i);
            }
        }

        /// <summary>
        /// Clears the draw method list.
        /// </summary>
        public void ClearDrawMethods()
        {
            drawMethods.Clear();
            drawMethodColors.Clear();
            drawMethodWidths.Clear();
        }

        /// <summary>
        /// Raises the <see cref="E:Paint"/> event.
        /// </summary>
        /// <param name="pe">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        protected override void OnPaint(PaintEventArgs pe)
        {            
            pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;            
            try
            {
                Graphics canvas = pe.Graphics;

                Matrix mscale = new Matrix();
                mscale.Scale(magnify, magnify, MatrixOrder.Append);
                canvas.Transform = mscale;

                for (int i = 0; i < drawMethods.Count; i++)
                {
                    try
                    {
                        drawMethods[i](canvas, drawMethodColors[i], drawMethodWidths[i]);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.Write(e.StackTrace);
                    }
                    canvas.Transform = mscale;
                }
             }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e.StackTrace);
            }
        }

        /// <summary>
        /// Gets or sets the width of the displayed object. [m]
        /// Also sets the zoom property of this canvas accordingly.
        /// </summary>
        /// <value>
        /// The width of the displayed object.
        /// </value>
        public double DisplayedObjectWidth
        {
            get { return displayedObjectWidth; }
            set { 
                displayedObjectWidth = value; 
                updateMagnify(); 
            }
        }

        /// <summary>
        /// Gets or sets the height of the displayed object. [m]
        /// Also sets the zoom property of this canvas accordingly.
        /// </summary>
        /// <value>
        /// The height of the displayed object.
        /// </value>
        public double DisplayedObjectHeight
        {
            get { return displayedObjectHeight; }
            set
            {
                displayedObjectHeight = value;
                updateMagnify();
            }
        }

        /// <summary>
        /// Handles the ClientSizeChanged event of the Canvas control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Canvas_ClientSizeChanged(object sender, EventArgs e)
        {
            updateMagnify();
        }

        /// <summary>
        /// Gets the absolute position of the specified coordinates on this canvas.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns></returns>
        public IVector GetAbsolutePosition(int x, int y)
        {
            return new Vector(x / this.magnify, y / this.magnify);
        }

    }
}
