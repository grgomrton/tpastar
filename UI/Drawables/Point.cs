using System.Drawing;

namespace TriangulatedPolygonAStar.UI
{
    public abstract class Point : IPoint
    {
        private float radius;
        private Brush brush;
        private IVector position;

        protected Point(float radius, Color color, IVector position)
        {
            this.radius = radius;
            brush = new SolidBrush(color);
            this.position = position;
        }
        
        public void Draw(Graphics canvas)
        {
            PointF positionFloat = position.ToPointF();
            float x = positionFloat.X - radius;
            float y = positionFloat.Y - radius;
            float diameter = 2 * radius;

            canvas.FillEllipse(brush, x, y, diameter, diameter);
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

        public bool ContainsPoint(IVector pointInAbsoluteCoordinateSystem)
        {
            return pointInAbsoluteCoordinateSystem.DistanceFrom(position) < 2 * radius; // TODO why 2 times?
        }
    }
}