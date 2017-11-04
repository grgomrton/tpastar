﻿using System.Drawing;

namespace TriangulatedPolygonAStar.UI
{
    /// <summary>
    /// The visual representation of an <see cref="IVector"/>.
    /// </summary>
    public abstract class Point : IDrawable
    {
        private float radius;
        private Brush brush;
        private IVector position;

        protected Point(float radius, Color color, IVector position)
        {
            this.radius = radius;
            this.position = position;
            brush = new SolidBrush(color);
        }

        /// <inheritdoc />
        public void Draw(Graphics canvas)
        {
            var positionF = position.ToPointF();
            var diameter = 2 * radius;
            canvas.FillEllipse(brush, positionF.X - radius, positionF.Y - radius, diameter, diameter);
        }

        /// <summary>
        /// The position of this point in an absolute coordinate system.
        /// </summary>
        public IVector Position
        {
            get { return position; }
        }

        /// <summary>
        /// The radius which is used for displaying this point. The size need
        /// to be specified in the same coordinate system as the position itself.
        /// </summary>
        public double Radius
        {
            get { return radius; }
        }

        /// <summary>
        /// Updates the position which is displayed by this point.
        /// </summary>
        /// <param name="positionInAbsoluteCoordinateSystem"></param>
        public void SetPosition(IVector positionInAbsoluteCoordinateSystem)
        {
            position = positionInAbsoluteCoordinateSystem;
        }

    }
}