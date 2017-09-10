using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonTools.Geometry;
using CommonTools.Miscellaneous;

namespace PathFinder.Funnel
{
    public enum Side { Left, Right, Both, None };

    public class FunnelStructure
    {
        protected LinkedList<Vector3> funnel; // from the apex view, the left side is the first element, the last one is on the right side
        protected LinkedListNode<Vector3> apex;
        protected Curve path;

        public FunnelStructure(Vector3 startPoint) 
        {
            funnel = new LinkedList<Vector3>();
            apex = funnel.AddFirst(startPoint);
            path = new Curve();
            path.Add(startPoint);
        }

        public FunnelStructure(FunnelStructure other)
        {
            funnel = new LinkedList<Vector3>(other.funnel);
            apex = funnel.Find(other.apex.Value);
            path = new Curve(other.path);
        }

        public void StepTo(IEdge edge)
        {
            Side modifiedSide = AddNewVertexToFunnel(edge);

            if ((modifiedSide == Side.Left) || (modifiedSide == Side.Right))
            {
                RefreshFunnel(modifiedSide); // regular edge overstep
            }
            else if (modifiedSide == Side.None)
            {
                if (funnel.Count == 1)
                {
                    InitFunnel(edge); // this is the first edge
                }
                else
                {
                    throw new ArgumentException("Illegal new edge: funnel end and new edge doesn't have common vertex.", "edge");
                }                
            }            
            else if (modifiedSide == Side.Both)
            {
                throw new ArgumentException("Illegal new edge (step backwards on current final edge in funnel?)", "edge");
            }
        }

        public void FinalizePath(Vector3 goal)
        {
            funnel.AddLast(goal);
            RefreshFunnel(Side.Right);
            LinkedListNode<Vector3> node = apex;
            while (node.Value != goal)
            {
                node = node.Next;
                path.Add(node.Value);
            }
        }

        protected void InitFunnel(IEdge firstEdge)
        {
            Vector3 startPoint = apex.Value;

            Vector3 toV1 = firstEdge.A - startPoint;
            Vector3 toV2 = firstEdge.B - startPoint;

            if (OrientationUtil.ClockWise(toV1, toV2))
            {
                funnel.AddFirst(firstEdge.A);
                funnel.AddLast(firstEdge.B);
            }
            else if (OrientationUtil.CounterClockWise(toV1, toV2))
            {
                funnel.AddFirst(firstEdge.B);
                funnel.AddLast(firstEdge.A);
            }
            // else { } we have nothing to do, we were standing on the edge, this edge is not needed to be added
        }


        // returns the modified side
        // todo: addedge
        protected Side AddNewVertexToFunnel(IEdge edge)
        {
            Side ret = Side.None;
            Vector3 left = funnel.First.Value; // Left vertex in funnel
            Vector3 right = funnel.Last.Value; // Right vertex in funnel

            // a funnel mindkét vége azonos pont
            if (((left == edge.A) && (right == edge.B))
                || ((left == edge.B) && (right == edge.A))
                )
            {
                // kétszer léptünk ugyanarra az élre,
                // ez elvileg nem fordulhat elő
                ret = Side.Both;
            }
            // a funnel balszélső vertexe közös az éllel
            else if (left == edge.A)
            {
                // jobb oldalra fűzünk
                ret = Side.Right;
                funnel.AddLast(edge.B);
            }
            else if (left == edge.B)
            {
                ret = Side.Right;
                funnel.AddLast(edge.A);
            }
            // a funnel jobbszélső vertexe közös az éllel
            else if (right == edge.A)
            {
                // bal oldalra fűzünk
                ret = Side.Left;
                funnel.AddFirst(edge.B);
            }
            else if (right == edge.B)
            {
                ret = Side.Left;
                funnel.AddFirst(edge.A);
            }
            else
            {
                ret = Side.None;
            }

            return ret;
        }

        protected void RefreshFunnel(Side modifiedSide)
        {
            // ha jobbról indulunk - ez van papírra rajzolva
            if (modifiedSide == Side.Right)
            {
                bool popped = true;

                while (popped && (funnel.Count >= 3))
                {
                    popped = false;
                    LinkedListNode<Vector3> last = funnel.Last;
                    Vector3 right = last.Value;
                    Vector3 right_1 = last.Previous.Value;
                    Vector3 right_2 = last.Previous.Previous.Value;
                    Vector3 v1 = right_1 - right;
                    Vector3 v2 = right_2 - right;
                    if (apex != last.Previous)
                    {
                        if (OrientationUtil.ClockWise(v1, v2))
                        {
                            funnel.Remove(last.Previous);
                            popped = true;
                        }
                    }
                    else
                    {
                        if (OrientationUtil.CounterClockWise(v1, v2))
                        {
                            funnel.Remove(last.Previous);
                            path.Add(last.Previous.Value);
                            apex = last.Previous;
                            popped = true;
                        }
                    }
                }
            }
            else if (modifiedSide == Side.Left)
            {
                bool popped = true;
                while (popped && (funnel.Count >= 3))
                {
                    popped = false;
                    LinkedListNode<Vector3> first = funnel.First;
                    Vector3 left = first.Value;
                    Vector3 left_1 = first.Next.Value;
                    Vector3 left_2 = first.Next.Next.Value;
                    Vector3 v1 = left_1 - left;
                    Vector3 v2 = left_2 - left;
                    if (apex != first.Next)
                    {
                        if (OrientationUtil.CounterClockWise(v1, v2))
                        {
                            funnel.Remove(first.Next);
                            popped = true;
                        }
                    }
                    else
                    {
                        if (OrientationUtil.ClockWise(v1, v2))
                        {
                            funnel.Remove(first.Next);
                            path.Add(first.Next.Value);
                            apex = first.Next;
                            popped = true;
                        }
                    }
                }
            }
        }

        public Curve Path
        {
            get { return path; }
        }

        // TODO: only for test now, but should be changed for use in algorithm
        internal IEnumerable<Vector3> Funnel => funnel;
    }
}
