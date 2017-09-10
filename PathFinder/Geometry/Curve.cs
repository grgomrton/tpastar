using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace TriangulatedPolygonAStar.Geometry
{
    /// <summary>
    /// Represents a curve by a list of <see cref="Vector3"/> objects.
    /// </summary>
    public class Curve : IEnumerable<IVector> // TODO this class is unnecessary, can be replaced with a linked list
    {
        private LinkedList<IVector> points;
        private double length;

        /// <summary>
        /// Initializes a new instance of the <see cref="Curve"/> class that is empty.
        /// </summary>
        public Curve()
        {
            points = new LinkedList<IVector>();
            length = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Curve"/> class that contains elements copied from the specified <see cref="Curve"/>.
        /// </summary>
        /// <param name="p">The <see cref="Curve"/> whose elements are copied to the new <see cref="Curve"/>.</param>
        public Curve(Curve p)
        {
            points = new LinkedList<IVector>(p.points);
            length = p.length;
        }

        /// <summary>
        /// Adds the specified point to the end of the <see cref="Curve"/>.
        /// </summary>
        /// <param name="point">The point to add at the end of the <see cref="Curve"/>.</param>
        public void Add(IVector point)
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
        public IEnumerator<IVector> GetEnumerator()
        {
            return points.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        /// <summary>
        /// Converts the curve to a list of <see cref="Vector3"/>.
        /// </summary>
        /// <returns>List of vectors.</returns>
        public List<IVector> ToList()
        {
            return new List<IVector>(points);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (IVector v in points)
            {
                sb.AppendLine(String.Format("({0: 00.00;-00.00};{1: 00.00;-00.00})", v.X, v.Y));
            }
            return sb.ToString();
        }

    }
}
