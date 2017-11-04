using System.Drawing;

namespace TriangulatedPolygonAStar.UI
{
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

        public void Draw(Graphics canvas)
        {
            var positionF = position.ToPointF();
            var diameter = 2 * radius;
            canvas.FillEllipse(brush, positionF.X - radius, positionF.Y - radius, diameter, diameter);
        }

        public IVector Position
        {
            get { return position; }
        }

        public double Radius
        {
            get { return radius; }
        }

        public void SetPosition(IVector positionInAbsoluteCoordinateSystem)
        {
            position = positionInAbsoluteCoordinateSystem;
        }
    }
}