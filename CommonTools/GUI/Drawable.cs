using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CommonTools.GUI
{

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
}
