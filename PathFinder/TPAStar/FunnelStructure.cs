using System;
using System.Collections.Generic;

namespace TriangulatedPolygonAStar
{
    public class FunnelStructure
    {
        private LinkedList<IVector> funnel; // from the apex view, the left side is the first element, the last one is on the right side
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
                throw new ArgumentException("Illegal new edge: edge is identical with the edge defined by the funnel endpoints",
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
                    throw new ArgumentException("Illegal new edge: funnel end and new edge do not have common vertex", nameof(edge));
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

        private void InitFunnel(IEdge firstEdge)
        {
            IVector startPoint = apex.Value;

            IVector toV1 = firstEdge.A.Minus(startPoint);
            IVector toV2 = firstEdge.B.Minus(startPoint);

            if (toV1.IsInCounterClockWiseDirectionFrom(toV2))
            {
                funnel.AddFirst(firstEdge.A);
                funnel.AddLast(firstEdge.B);
            }
            else if (toV1.IsInClockWiseDirectionFrom(toV2))
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
                LinkedListNode<IVector> last = funnel.Last;
                IVector right = last.Value;
                IVector right_1 = last.Previous.Value;
                IVector right_2 = last.Previous.Previous.Value;
                IVector v1 = right_1.Minus(right);
                IVector v2 = right_2.Minus(right);
                if (apex != last.Previous)
                {
                    if (v1.IsInCounterClockWiseDirectionFrom(v2))
                    {
                        funnel.Remove(last.Previous);
                        popped = true;
                    }
                }
                else
                {
                    if (v1.IsInClockWiseDirectionFrom(v2))
                    {
                        funnel.Remove(last.Previous);
                        path.AddLast(last.Previous.Value);
                        apex = last.Previous;
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
                LinkedListNode<IVector> first = funnel.First;
                IVector left = first.Value;
                IVector left_1 = first.Next.Value;
                IVector left_2 = first.Next.Next.Value;
                IVector v1 = left_1.Minus(left);
                IVector v2 = left_2.Minus(left);
                if (apex != first.Next)
                {
                    if (v1.IsInClockWiseDirectionFrom(v2))
                    {
                        funnel.Remove(first.Next);
                        popped = true;
                    }
                }
                else
                {
                    if (v1.IsInCounterClockWiseDirectionFrom(v2))
                    {
                        funnel.Remove(first.Next);
                        path.AddLast(first.Next.Value);
                        apex = first.Next;
                        popped = true;
                    }
                }
            }
        }
        
    }
}