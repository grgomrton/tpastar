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
    /// The visual representation of a start point.
    /// </summary>
    public class StartPoint : Point
    {
        private static float StartRadius = 0.08f;
        private static Color Color = Color.Blue; 
        
        /// <summary>
        /// Initializes a new instance of the <see cref="StartPoint"/> class which
        /// draws a start point on the canvas.
        /// </summary>
        /// <param name="position">The position of this point in an absolute coordinate system</param>
        public StartPoint(IVector position)
        : base(StartRadius, Color, position)
        {
        }
    }
}