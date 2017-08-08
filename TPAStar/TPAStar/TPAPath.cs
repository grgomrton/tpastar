using System;
using System.Collections.Generic;
using System.Linq;
using CommonTools.Geometry;
using CommonTools.Miscellaneous;
using PathFinder.Funnel;

namespace PathFinder.TPAStar
{
    public class TPAPath : FunnelStructure
    {
        private Triangle currentTriangle;
        private Triangle previousTriangle; // TODO: this might be removed by not adding backward steps in solver
        private IEnumerable<Triangle> explorableTriangles;
        private Edge currentEdge;
        private double dgMin; // minimum length from apex to finaltriangle
        private double dgMax;
        private double h; // heuristic value
        private bool isGoalReached;
        
        internal TPAPath(Vector3 startPoint, Triangle startTriangle) : base(startPoint)
        {
            this.currentTriangle = startTriangle;
            this.previousTriangle = null;
            explorableTriangles = startTriangle.Neighbours;
            dgMin = 0;
            dgMax = 0;
            h = 0;
            isGoalReached = false;
        }

        private TPAPath(TPAPath other) : base(other)
        {
            currentTriangle = other.currentTriangle;
            previousTriangle = other.previousTriangle;
            explorableTriangles = other.explorableTriangles;
            dgMin = other.dgMin;
            dgMax = other.dgMax;
            h = other.h;
            isGoalReached = other.isGoalReached;
        }

        internal TriangleEvaluationResult StepTo(Triangle t, Vector3[] goalPoints)
        {
            if (previousTriangle != null) // funnel contains only the start point, we need to initialize the funnel first
            {
                Edge currentEdge = currentTriangle.GetCommonEdge(t);
                
                base.StepTo(currentEdge);
                
                UpdateLowerBoundOfPathToEdge(currentEdge);
                UpdateHigherBoundOfPathToEdge(currentEdge);
                UpdateHeuristicValue(currentEdge, goalPoints);

                this.currentEdge = currentEdge;
            }
            
            explorableTriangles = t.Neighbours.Where(triangle => triangle != previousTriangle);
            previousTriangle = currentTriangle;
            currentTriangle = t;
            
            return new TriangleEvaluationResult(h, EstimatedMinimalOverallCost, ShortestPossiblePathLength, LongestPossiblePathLength);
        }

        internal void UpdateLowerBoundOfPathToEdge(Edge edge)
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

        protected void UpdateHigherBoundOfPathToEdge(Edge edge)
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

        private void UpdateHeuristicValue(Edge edge, Vector3[] goalPoints)
        {
            h = FindDistanceFromClosestGoalPoint(edge, goalPoints);
        }

        private double FindDistanceFromClosestGoalPoint(Edge edge, IEnumerable<Vector3> goals)
        {
            return goals.Min(point => edge.DistanceFromPoint(point));
        }

        public Curve GetBuiltPath()
        {
            return path;
        }

        /// <summary>
        /// The length of the possibly shortest path to the closest goal point along this path.
        /// It is the sum of the length of the shortest possible path to the end edge of this path, 
        /// and the shortest path between the edge and the closest goal point.
        /// </summary>
        public double EstimatedMinimalOverallCost
        {
            get { return ShortestPossiblePathLength + h; }
        }

        public double ShortestPossiblePathLength
        {
            get { return path.Length + dgMin; }
        }

        public double LongestPossiblePathLength
        {
            get { return path.Length + dgMax; }
        }

        public Edge CurrentEdge
        {
            get { return currentEdge; }
        }

        internal IEnumerable<Triangle> ExplorableTriangles => explorableTriangles;

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
