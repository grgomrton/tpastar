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
    /// An object which can be drawn on a <see cref="Canvas "/>.
    /// </summary>
    public interface IDrawable
    {
        /// <summary>
        /// Draws the visual representation of the object to the specified canvas.
        /// Every drawable need to use the same coordinate-system.
        /// </summary>
        /// <param name="canvas">The canvas to draw onto</param>
        void Draw(Graphics canvas);
        
        /// <summary>
        /// The highest coordinates of the rectangle in which the entire object fits.
        /// </summary>
        PointF BoundingBoxHigh { get; }
        
        /// <summary>
        /// The lowest coordinates of the rectangle in which the entire object fits.
        /// </summary>
        PointF BoundingBoxLow { get; }
    }
}