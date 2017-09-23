using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TriangulatedPolygonAStar.BasicGeometry;

namespace TriangulatedPolygonAStar.UI
{
    public class PolyLine : IDrawable
    {
        private IEnumerable<IVector> points;

        private static Dictionary<string, Color> colors = new Dictionary<string, Color>()
        {
            {"edge", Color.Green},
            {"data", Color.Black}
        };

        private static Dictionary<string, float> widths = new Dictionary<string, float>()
        {
            {"edge", 0.04f},
            {"data", 0.12f},
            {"fontSize", 0.12f}
        };
        
        public PolyLine(IEnumerable<IVector> points)
        {
            this.points = points;
        }
        
        public void Draw(Graphics canvas)
        {
            if (points.Any())
            {
                List<PointF> nodes = new List<PointF>();
                foreach (Vector v in points)
                {
                    nodes.Add(v.ToPointF());
                }
                canvas.DrawLines(new Pen(colors["edge"], widths["edge"]), nodes.ToArray());
                float fontSize = widths["fontSize"];
                var position = points.Last().Minus(new Vector(2 * fontSize, 3 * fontSize));
                var positionFloat = new PointF(Convert.ToSingle(position.X), Convert.ToSingle(position.Y));
                canvas.DrawString(points.Length().ToString("#.##"), new Font("Arial", fontSize, FontStyle.Bold), new SolidBrush(colors["data"]), positionFloat);
            }
        }

        public void SetPoints(IEnumerable<IVector> points)
        {
            this.points = points;
        }
    }
}