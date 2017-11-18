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
    /// The visual representation of a destination point.
    /// </summary>
    public class GoalMarker : ILocationMarker
    {
        private static readonly float Radius;
        private static readonly Brush FillBrush;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GoalMarker"/> class which
        /// draws a mark at the location of the goal point.
        /// </summary>
        /// <param name="position">The position of this point</param>
        public GoalMarker(IVector position)
        {
            SetLocation(position);
        }

        /// <inheritdoc />
        public IVector CurrentLocation { get; private set; }

        /// <inheritdoc />
        public PointF BoundingBoxHigh { get; private set; }

        /// <inheritdoc />
        public PointF BoundingBoxLow { get; private set; }

        /// <inheritdoc />
        public void SetLocation(IVector position)
        {
            this.CurrentLocation = position;
            BoundingBoxHigh = CurrentLocation.ToPointF();
            BoundingBoxLow = CurrentLocation.ToPointF();
        }

        /// <inheritdoc />
        public bool IsPositionUnderMarker(IVector position)
        {
            return CurrentLocation.DistanceFrom(position) < 2 * Radius;
        }

        /// <inheritdoc />
        public void Draw(Graphics canvas)
        {
            var positionF = CurrentLocation.ToPointF();
            var diameter = 2 * Radius;
            canvas.FillEllipse(FillBrush, positionF.X - Radius, positionF.Y - Radius, diameter, diameter);
        }

        static GoalMarker()
        {
            Radius = 0.08f;
            FillBrush = new SolidBrush(Color.Green);
        }
    }
}