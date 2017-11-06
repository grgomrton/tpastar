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
    public class GoalPoint : Point
    {
        private static float GoalRadius = 0.8f;
        private static Color Color = Color.Green; 
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GoalPoint"/> class which
        /// draws a goal point to the canvas.
        /// </summary>
        /// <param name="position">The position of this point in absolute coordinate system</param>
        public GoalPoint(IVector position) 
            : base(GoalRadius, Color, position)
        {
        }

    }
}