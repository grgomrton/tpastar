using System;
using System.Collections.Generic;

namespace TriangulatedPolygonAStar
{
    public class FunnelStructure
    {
        private LinkedList<IVector> funnel;
        private LinkedListNode<IVector> apex;
        private LinkedList<IVector> path;
        
        private enum Side { Left, Right, Both, None }

        public FunnelStructure(IVector startPoint)
        {
            funnel = new LinkedList<IVector>();
            apex = funnel.AddFirst(startPoint);
            path = new LinkedList<IVector>();
            path.AddFirst(startPoint);
        }

        public FunnelStructure(FunnelStructure other)
        {
            funnel = new LinkedList<IVector>(other.funnel);
            apex = funnel.Find(other.apex.Value);
            path = new LinkedList<IVector>(other.path);
        }

        public LinkedList<IVector> Path
        {
            get { return path; }
        }

        public LinkedListNode<IVector> Apex
        {
            get { return apex; }
        }
        
        public void StepTo(IEdge edge)
        {
            Side commonSide = DetermineSideSharedByFunnelAndNewEdge(edge, funnel);
            IVector leftEnd = funnel.First.Value; // Left vertex in funnel
            IVector rightEnd = funnel.Last.Value; // Right vertex in funnel

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

        public void FinalizePath(IVector goal)
        {
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
            // else { } we have nothing to do, we were standing on the edge, this edge is not needed to be added
        }

        private static Side DetermineSideSharedByFunnelAndNewEdge(IEdge edge, LinkedList<IVector> funnel)
        {
            IVector leftEnd = funnel.First.Value; // Left vertex in funnel
            IVector rightEnd = funnel.Last.Value; // Right vertex in funnel
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
            
            bool popped = true;
            while (popped && (funnel.Count >= 3))
            {
                popped = false;
                IVector rightEnd = funnel.Last.Value;
                IVector secondFromRight = funnel.Last.Previous.Value;
                IVector thirdFromRight = funnel.Last.Previous.Previous.Value;
                IVector lastToSecondFromRight = secondFromRight.Minus(rightEnd);
                IVector lastToThirdFromRight = thirdFromRight.Minus(rightEnd);
                if (apex != funnel.Last.Previous)
                {
                    if (lastToSecondFromRight.IsInCounterClockWiseDirectionFrom(lastToThirdFromRight))
                    {
                        funnel.Remove(funnel.Last.Previous);
                        popped = true;
                    }
                }
                else
                {
                    if (lastToSecondFromRight.IsInClockWiseDirectionFrom(lastToThirdFromRight))
                    {
                        path.AddLast(thirdFromRight);
                        funnel.Remove(funnel.Last.Previous);
                        apex = funnel.Last.Previous;
                        popped = true;
                    }
                }
            }
        }

        private void AddToLeftSideOfFunnel(IVector point)
        {
            funnel.AddFirst(point);
            
            bool popped = true;
            while (popped && (funnel.Count >= 3))
            {
                popped = false;
                IVector leftEnd = funnel.First.Value;
                IVector secondFromLeft = funnel.First.Next.Value;
                IVector thirdFromLeft = funnel.First.Next.Next.Value;
                IVector firstToSecondFromLeft = secondFromLeft.Minus(leftEnd);
                IVector firstToThirdFromLeft = thirdFromLeft.Minus(leftEnd);
                if (apex != funnel.First.Next)
                {
                    if (firstToSecondFromLeft.IsInClockWiseDirectionFrom(firstToThirdFromLeft))
                    {
                        funnel.Remove(funnel.First.Next);
                        popped = true;
                    }
                }
                else
                {
                    if (firstToSecondFromLeft.IsInCounterClockWiseDirectionFrom(firstToThirdFromLeft))
                    {
                        path.AddLast(thirdFromLeft);
                        funnel.Remove(funnel.First.Next);
                        apex = funnel.First.Next;
                        popped = true;
                    }
                }
            }
        }
        
    }
}