using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonTools.Geometry;
using CommonTools.Miscellaneous;
using PathFinder.Funnel;

namespace PathFinder.TPAStar
{
    public class TPAPath : FunnelStructure
    {
        private Triangle currentTriangle;
        private Triangle previousTriangle;
        private double dgMin; // minimum length from apex to finaltriangle
        private double dgMax;
        private double h; // heuristic value
        private bool isGoalReached;
        
        public TPAPath(Vector3 startPoint, Triangle startTriangle) : base(startPoint)
        {
            this.currentTriangle = startTriangle;
            this.previousTriangle = null;
            dgMin = 0;
            dgMax = 0;
            h = 0;
            isGoalReached = false;
        }

        public TPAPath(TPAPath other) : base(other)
        {
            currentTriangle = other.currentTriangle;
            previousTriangle = other.previousTriangle;
            dgMin = other.dgMin;
            dgMax = other.dgMax;
            h = other.h;
            isGoalReached = other.isGoalReached;
        }

        internal TriangleEvaluationResult StepTo(Triangle t, Vector3[] goalPoints)
        {
            Edge currentEdge = currentTriangle.GetCommonEdge(t);

            if (previousTriangle == null) // funnel contains only the start point, we need to initialize the funnel first
            {
                InitFunnel(currentEdge, currentTriangle);
            }
            else
            {
                base.StepTo(currentEdge);
            }

            UpdateMinPathToEdge(currentEdge);
            UpdateMaxPathToEdge(currentEdge);
            UpdateHeuristicValue(currentEdge, goalPoints);
            
            previousTriangle = currentTriangle;
            currentTriangle = t;
            
            return new TriangleEvaluationResult(h, FMin, GMin, GMax);
        }

        protected void UpdateMinPathToEdge(Edge edge)
        {
            double minpathlength = 0;

            if ((apex.Next != null) && (apex.Previous != null)) // otherwise the apex lies on the edge, the path is already the minpath to the edge
            {
                Vector3 closestPoint = edge.ClosesPointFromPoint(apex.Value);
                Vector3 apexPoint = apex.Value;
                Vector3 left_1 = apex.Previous.Value;
                Vector3 right_1 = apex.Next.Value;

                Vector3 val = left_1 - apexPoint; // vector from apex to the left part of the funnel
                Vector3 var = right_1 - apexPoint; // vector from apex to the right part of the funnel
                Vector3 vacp = closestPoint - apexPoint; // vector from apex to the closest point

                // 
                if (OrientationUtil.ClockWise(val, vacp))
                {
                    if (OrientationUtil.CounterClockWise(var, vacp))
                    {
                        // easy way, closest point is visible from apex
                        //path.Add(closestPoint);
                        minpathlength += Vector3.Distance(apexPoint, closestPoint);
                    }
                    else
                    {
                        // we have to march on the right side of the funnel, to see the edge
                        LinkedListNode<Vector3> node = apex;

                        while ((OrientationUtil.ClockWise(var, vacp)) && (node.Next.Next != null)) // TODO: next.next béna..
                        {
                            minpathlength += Vector3.Distance(node.Value, node.Next.Value);

                            node = node.Next;
                            //path.Add(node.Value); TODO: guinak

                            closestPoint = edge.ClosesPointFromPoint(node.Value);
                            apexPoint = node.Value;
                            left_1 = node.Previous.Value;
                            right_1 = node.Next.Value;

                            val = left_1 - apexPoint; // vector from apex to the left part of the funnel
                            var = right_1 - apexPoint; // vector from apex to the right part of the funnel
                            vacp = closestPoint - apexPoint; // vector from apex to the closest point
                        }

                        closestPoint = edge.ClosesPointFromPoint(node.Value);
                        minpathlength += Vector3.Distance(node.Value, closestPoint);
                        //path.Add(closestPoint);
                    }
                }
                else
                {
                    // we have to march on the left side of the funnel, to see the edge
                    LinkedListNode<Vector3> node = apex;

                    while ((OrientationUtil.CounterClockWise(val, vacp)) && (node.Previous.Previous != null))
                    {
                        minpathlength += Vector3.Distance(node.Value, node.Previous.Value);

                        node = node.Previous;
                        //path.Add(apex.Value);

                        closestPoint = edge.ClosesPointFromPoint(node.Value);
                        apexPoint = node.Value;
                        left_1 = node.Previous.Value;
                        right_1 = node.Next.Value;

                        val = left_1 - apexPoint; // vector from apex to the left part of the funnel
                        var = right_1 - apexPoint; // vector from apex to the right part of the funnel
                        vacp = closestPoint - apexPoint; // vector from apex to the closest point
                    }

                    closestPoint = edge.ClosesPointFromPoint(node.Value);
                    minpathlength += Vector3.Distance(node.Value, closestPoint);
                    //path.Add(closestPoint);
                }
            }
            dgMin = minpathlength;
        }

        protected void UpdateMaxPathToEdge(Edge edge)
        {
            LinkedListNode<Vector3> node = apex;
            double maxLeft = 0, maxRight = 0;

            while (node.Previous != null)
            {
                maxLeft += Vector3.Distance(node.Value, node.Previous.Value);
                node = node.Previous;
            }

            node = apex;
            while (node.Next != null)
            {
                maxRight += Vector3.Distance(node.Value, node.Next.Value);
                node = node.Next;
            }

            dgMax = Math.Max(maxLeft, maxRight);
        }

        protected void InitFunnel(Edge edge, Triangle startTriangle)
        {
            // a funnel-t a háromszög kp-ja és a csúcspontok által definiált körbejárási irány alapján inicializáljuk,
            // mivel csak a start és az él alapján nem lehet, amennyiben a start pont az élre esik
            Vector3 toV1 = new Vector3(edge.V1 - startTriangle.Centroid);
            Vector3 toV2 = new Vector3(edge.V2 - startTriangle.Centroid);

            // Ha egy vonalban vannak? - elvileg nálunk most nem fordulhat elő...
            // távolság alapján lehetne, a közelebbi vektor lenne, valszeg úgy értelmes..
            if (OrientationUtil.ClockWise(toV1, toV2)) // V1 is on the left side of the funnel TODO: wtf?
            {
                // funnel left-right = edge.v1, start, edge.v2
                funnel.AddFirst(edge.V1);
                funnel.AddLast(edge.V2);
            }
            else
            {
                // funnel left-right = edge.v2, start, edge.v1
                funnel.AddFirst(edge.V2);
                funnel.AddLast(edge.V1);
            }
        }

        public new void FinalizePath(Vector3 goalPoint)
        {
            base.FinalizePath(goalPoint);
            dgMin = 0;
            dgMax = 0;
            h = 0;
            isGoalReached = true;
        }

        public bool GoalReached
        {
            get { return isGoalReached; }
        }

        protected void UpdateHeuristicValue(Edge edge, Vector3[] goalPoints)
        {
            double minH = 0;

            for (int i = 0; i < goalPoints.Length; i++)
            {
                double distance = edge.DistanceFromPoint(goalPoints[i]);
                if ((minH == 0) || (distance < minH))
                {
                    minH = distance;
                }
            }

            h = minH;
        }

        public Curve GetBuiltPath()
        {
            return path;
        }

        /// <summary>
        /// we are optimistic, we count with this value
        /// </summary>
        /// <returns></returns>
        public double FMin
        {
            get { return GMin + h; }
        }

        public double GMin
        {
            get { return path.Length + dgMin; }
        }

        public double GMax
        {
            get { return path.Length + dgMax; }
        }

        public Edge CurrentEdge
        {
            get 
            {
                Edge ret = null;
                if (previousTriangle != null)
                {
                    ret = previousTriangle.GetCommonEdge(currentTriangle);
                }
                return ret;
            }
        }

        public IEnumerable<Triangle> GetExplorableTriangles()
        {
            return currentTriangle.Neighbours.Where(triangle => triangle != previousTriangle);
        }

        public TPAPath Clone()
        {
            return new TPAPath(this);
        }

        public IEnumerable<Vector3> GetReachedGoalPoints(Vector3[] goalPoints)
        {
            return goalPoints.Where(point => currentTriangle.ContainsPoint(point));
        }
    }
}
