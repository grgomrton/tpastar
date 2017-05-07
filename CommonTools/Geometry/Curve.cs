using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;

namespace CommonTools.Geometry
{
    /// <summary>
    /// Represents a curve by a list of <see cref="Vector3"/> objects.
    /// </summary>
    public class Curve : IEnumerable
    {
        LinkedList<Vector3> points;
        double length;

        /// <summary>
        /// Initializes a new instance of the <see cref="Curve"/> class that is empty.
        /// </summary>
        public Curve()
        {
            points = new LinkedList<Vector3>();
            length = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Curve"/> class that contains elements copied from the specified <see cref="Curve"/>.
        /// </summary>
        /// <param name="p">The <see cref="Curve"/> whose elements are copied to the new <see cref="Curve"/>.</param>
        public Curve(Curve p)
        {
            points = new LinkedList<Vector3>(p.points);
            length = p.length;
        }

        /// <summary>
        /// Adds the specified point to the end of the <see cref="Curve"/>.
        /// </summary>
        /// <param name="point">The point to add at the end of the <see cref="Curve"/>.</param>
        public void Add(Vector3 point)
        {
            if (points.Count > 0)
            {
                length += point.Distance(points.Last.Value);
            }
            points.AddLast(point);
        }

        /// <summary>
        /// Gets the length of the <see cref="Curve"/> represented by the list of <see cref="Vector3"/>. 
        /// The length is the sum of the distances between the consecutive <see cref="Vector3"/>s.
        /// </summary>
        public double Length
        {
            get { return length; }
        }

        /// <summary>
        /// Gets the number of vectors stored in the <see cref="Curve"/>.
        /// </summary>
        public int Count
        {
            get { return points.Count; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return points.GetEnumerator();
        }

        /// <summary>
        /// Converts the curve to a list of <see cref="Vector3"/>.
        /// </summary>
        /// <returns>List of vectors.</returns>
        public List<Vector3> ToList()
        {
            return new List<Vector3>(points);
        }

        /// <summary>
        /// Draws a polyline representing this curve.
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="colors">Required: edge</param>
        /// <param name="widths">Required: edge</param>
        public void Draw(Graphics canvas, Dictionary<string, Color> colors, Dictionary<string, float> widths)
        {
            if (points.Count >= 2)
            {
                List<PointF> nodes = new List<PointF>();
                foreach (Vector3 v in points)
                {
                    nodes.Add(v.ToPointF());
                }

                canvas.DrawLines(new Pen(colors["edge"], widths["edge"]), nodes.ToArray());
            }
        }

        /// <summary>
        /// Draws the length of the curve at the endpoint.
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="colors">Required: data</param>
        /// <param name="widths">Required: fontSize</param>
        public void DrawMeta(Graphics canvas, Dictionary<string, Color> colors, Dictionary<string, float> widths)
        {
            if (points.Count > 0)
            {
                float fontSize = widths["fontSize"];
                canvas.DrawString(length.ToString("#.##"), new Font("Arial", fontSize, FontStyle.Bold), new SolidBrush(colors["data"]), (points.Last.Value - new Vector3(2 * fontSize, 3 * fontSize, 0)).ToPointF());
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        override public String ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Vector3 v in points)
            {
                sb.AppendLine(String.Format("({0: 00.00;-00.00};{1: 00.00;-00.00})", v.X, v.Y));
            }
            return sb.ToString();
        }
    }
}
