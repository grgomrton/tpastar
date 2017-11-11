using System;
using System.Collections.Generic;

namespace TriangulatedPolygonAStar
{
    /// <summary>
    /// The funnel structure from the Funnel algorithm that can be used to determine the 
    /// euclidean shortest path between a start and a goal point along a set of adjacent triangles.
    /// </summary>
    public class FunnelStructure
    {
        private LinkedList<IVector> funnel;
        private LinkedListNode<IVector> apex;
        private LinkedList<IVector> path;
        
        private enum Side { Left, Right, Both, None }

        /// <summary>
        /// Initializes a new instance of a <see cref="FunnelStructure"/> class 
        /// by the first point in the path.
        /// </summary>
        /// <param name="startPoint">The start point of the path</param>
        public FunnelStructure(IVector startPoint)
        {
            CheckForNullArgument(startPoint, nameof(startPoint));
            
            funnel = new LinkedList<IVector>();
            apex = funnel.AddFirst(startPoint);
            path = new LinkedList<IVector>();
            path.AddFirst(startPoint);
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="FunnelStructure"/> class by 
        /// another funnel instance. The instantiation will result in a 
        /// deep copy of the specified one.
        /// </summary>
        /// <param name="other">The other funnel to copy the values from</param>
        public FunnelStructure(FunnelStructure other)
        {
            CheckForNullArgument(other, nameof(other));
            
            funnel = new LinkedList<IVector>(other.funnel);
            apex = funnel.Find(other.apex.Value);
            path = new LinkedList<IVector>(other.path);
        }

        /// <summary>
        /// The path which is built by stepping over the specified edges.
        /// Once a goal point has added to the funnel, the path contains the complete path.
        /// </summary>
        public LinkedList<IVector> Path
        {
            get { return path; }
        }
        
        /// <summary>
        /// The apex of the funnel. 
        /// </summary>
        public LinkedListNode<IVector> Apex
        {
            get { return apex; }
        }
        
        /// <summary>
        /// Extends the funnel by adding a new edge at the end.
        /// </summary>
        /// <param name="edge">The edge that was stepped over</param>
        public void StepOver(IEdge edge)
        {
            CheckForNullArgument(edge, nameof(edge));

            Side commonSide = DetermineSideSharedBy(edge, funnel);
            IVector leftEnd = funnel.First.Value;
            IVector rightEnd = funnel.Last.Value;

            if (commonSide == Side.Both)
            {
                throw new ArgumentException("Illegal new edge: edge endpoints are identical with funnel endpoints",
                    nameof(edge));
            }
            else if (commonSide == Side.Left)
            {
                IVector pointToAdd = leftEnd.Equals(edge.A) ? edge.B : edge.A;
                AddToRightSideOfFunnel(pointToAdd); // regular edge overstep
            }
            else if (commonSide == Side.Right)
            {
                IVector pointToAdd = rightEnd.Equals(edge.A) ? edge.B : edge.A;
                AddToLeftSideOfFunnel(pointToAdd); // regular edge overstep
            }
            else if (commonSide == Side.None)
            {
                if (funnel.Count == 1)
                {
                    InitFunnel(edge); // this is the first edge
                }
                else
                {
                    throw new ArgumentException("Illegal new edge: funnel end and new edge do not share any vertex", nameof(edge));
                }
            }
        }

        /// <summary>
        /// Cleans up the funnel and adds the required points to the path to define a
        /// complete path.
        /// </summary>
        /// <param name="goal">The last point of the path</param>
        public void FinalizePath(IVector goal)
        {
            CheckForNullArgument(goal, nameof(goal));
            
            AddToRightSideOfFunnel(goal);
            LinkedListNode<IVector> node = apex;
            while (!node.Value.Equals(goal))
            {
                node = node.Next;
                path.AddLast(node.Value);
            }
        }

        // from the apex view, the left side is the first element, the last one is on the right side
        private void InitFunnel(IEdge firstEdge)
        {
            if (!firstEdge.PointLiesOnEdge(apex.Value))
            {
                IVector apexToEdgeA = firstEdge.A.Minus(apex.Value);
                IVector apexToEdgeB = firstEdge.B.Minus(apex.Value);

                if (apexToEdgeA.IsInCounterClockWiseDirectionFrom(apexToEdgeB))
                {
                    funnel.AddFirst(firstEdge.A);
                    funnel.AddLast(firstEdge.B);
                }
                else if (apexToEdgeA.IsInClockWiseDirectionFrom(apexToEdgeB))
                {
                    funnel.AddFirst(firstEdge.B);
                    funnel.AddLast(firstEdge.A);
                }
            }
        }

        private static Side DetermineSideSharedBy(IEdge edge, LinkedList<IVector> funnel)
        {
            IVector leftEnd = funnel.First.Value;
            IVector rightEnd = funnel.Last.Value;
            bool leftEndEqualsA = leftEnd.Equals(edge.A);
            bool leftEndEqualsB = leftEnd.Equals(edge.B);
            bool rightEndEqualsA = rightEnd.Equals(edge.A);
            bool rightEndEqualsB = rightEnd.Equals(edge.B);
            
            if ((leftEndEqualsA && rightEndEqualsB) || (leftEndEqualsB && rightEndEqualsA))
            {
                return Side.Both;
            }
            else if (leftEndEqualsA || leftEndEqualsB)
            {
                return Side.Left;
            }
            else if (rightEndEqualsA || rightEndEqualsB)
            {
                return Side.Right;
            }
            else
            {
                return Side.None;
            }
        }

        private void AddToRightSideOfFunnel(IVector point)
        {
            funnel.AddLast(point);
            IVector rightEndPoint = point;
            
            bool popped = true;
            while (popped && (funnel.Last.Previous != apex))
            {
                popped = false;
                LinkedListNode<IVector> secondItemFromRight = funnel.Last.Previous;
                LinkedListNode<IVector> thirdItemFromRight = funnel.Last.Previous.Previous;
                IVector backOne = secondItemFromRight.Value.Minus(rightEndPoint);
                IVector backTwo = thirdItemFromRight.Value.Minus(rightEndPoint);
                if (backTwo.IsInClockWiseDirectionFrom(backOne))
                {
                    funnel.Remove(secondItemFromRight);
                    popped = true;
                }
            }
            popped = true;
            while ((apex != funnel.First) && popped)
            {
                popped = false;
                IVector apexToRightEnd = funnel.Last.Value.Minus(apex.Value);
                IVector apexToOneLeft = apex.Previous.Value.Minus(apex.Value);
                if (apexToRightEnd.IsInCounterClockWiseDirectionFrom(apexToOneLeft))
                {
                    path.AddLast(apex.Previous.Value);
                    funnel.Remove(apex);
                    apex = funnel.Last.Previous;
                    popped = true;
                }                
            }
        }

        private void AddToLeftSideOfFunnel(IVector point)
        {
            funnel.AddFirst(point);
            IVector leftEndPoint = point;
            
            bool popped = true;
            while (popped && (funnel.First.Next != apex))
            {
                popped = false;
                LinkedListNode<IVector> secondItemFromLeft = funnel.First.Next;
                LinkedListNode<IVector> thirdItemFromLeft = funnel.First.Next.Next;
                IVector backOne = secondItemFromLeft.Value.Minus(leftEndPoint);
                IVector backTwo = thirdItemFromLeft.Value.Minus(leftEndPoint);
                if (backTwo.IsInCounterClockWiseDirectionFrom(backOne))
                {
                    funnel.Remove(secondItemFromLeft);
                    popped = true;
                }
            }
            popped = true;
            while ((apex != funnel.Last) && popped)
            {
                popped = false;
                IVector apexToLeftEnd = funnel.First.Value.Minus(apex.Value);
                IVector apexToOneRight = apex.Next.Value.Minus(apex.Value);
                if (apexToLeftEnd.IsInClockWiseDirectionFrom(apexToOneRight))
                {
                    path.AddLast(apex.Next.Value);
                    funnel.Remove(apex);
                    apex = funnel.First.Next;
                    popped = true;
                }                
            }
        }
        
        private static void CheckForNullArgument(object value, string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }
        
    }
}