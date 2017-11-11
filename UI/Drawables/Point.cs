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

using System.Drawing;

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
        public PointF BoundingBoxHigh { get { return position.ToPointF(); } }

        /// <inheritdoc />
        public PointF BoundingBoxLow { get { return position.ToPointF(); } }

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

        /// <inheritdoc />
        public void Draw(Graphics canvas)
        {
            var positionF = position.ToPointF();
            var diameter = 2 * radius;
            canvas.FillEllipse(brush, positionF.X - radius, positionF.Y - radius, diameter, diameter);
        }
        
    }
}