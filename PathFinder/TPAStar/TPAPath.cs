using System;
using System.Collections.Generic;
using System.Linq;
using PathFinder.Funnel;
using TriangulatedPolygonAStar.Geometry;

namespace PathFinder.TPAStar
{
    public class TPAPath : FunnelStructure
    {
        private ITriangle currentTriangle;
        private IEnumerable<ITriangle> explorableTriangles;
        private IEdge currentEdge;
        private double dgMin; // minimum length from apex to finaltriangle
        private double dgMax;
        private double h; // heuristic value
        private bool isGoalReached;
        
        internal TPAPath(IVector startPoint) : base(startPoint)
        {
            currentTriangle = null;
            explorableTriangles = null;
            dgMin = 0;
            dgMax = 0;
            h = 0;
            isGoalReached = false;
        }

        private TPAPath(TPAPath other) : base(other)
        {
            currentTriangle = other.currentTriangle;
            explorableTriangles = other.explorableTriangles;
            dgMin = other.dgMin;
            dgMax = other.dgMax;
            h = other.h;
            isGoalReached = other.isGoalReached;
        }

        internal TriangleEvaluationResult StepTo(ITriangle targetTriangle, IVector[] goalPoints)
        {
            if (currentTriangle == null)
            {
                currentTriangle = targetTriangle;
                explorableTriangles = targetTriangle.Neighbours;
            }
            else
            {
                currentEdge = currentTriangle.GetCommonEdge(targetTriangle);
                var explorableNeighbourList = new LinkedList<ITriangle>();
                foreach (var neighbour in targetTriangle.Neighbours)
                {
                    if (!neighbour.Equals(currentTriangle)) // TODO investigate this equality check warning
                    {
                        explorableNeighbourList.AddLast(neighbour);
                    }
                }
                explorableTriangles = explorableNeighbourList;
                currentTriangle = targetTriangle;
                
                base.StepTo(currentEdge);
                UpdateLowerBoundOfPathToEdge(currentEdge);
                UpdateHigherBoundOfPathToEdge(currentEdge);
                UpdateHeuristicValue(currentEdge, goalPoints);
            }
            
            return new TriangleEvaluationResult(h, EstimatedMinimalOverallCost, ShortestPossiblePathLength, LongestPossiblePathLength);
        }

        internal void UpdateLowerBoundOfPathToEdge(IEdge edge)
        {
            double minpathlength = 0;

            if ((apex.Next != null) && (apex.Previous != null)) // otherwise the apex lies on the edge, the path is already the minpath to the edge
            {
                IVector closestPoint = edge.ClosestPointOnEdgeFrom(apex.Value);
                IVector apexPoint = apex.Value;
                IVector left_1 = apex.Previous.Value;
                IVector right_1 = apex.Next.Value;

                IVector val = left_1.Minus(apexPoint); // vector from apex to the left part of the funnel
                IVector var = right_1.Minus(apexPoint); // vector from apex to the right part of the funnel
                IVector vacp = closestPoint.Minus(apexPoint); // vector from apex to the closest point

                // 
                if (val.ClockWise(vacp))
                {
                    if (var.CounterClockWise(vacp))
                    {
                        // easy way, closest point is visible from apex
                        //path.Add(closestPoint);
                        minpathlength += apexPoint.Distance(closestPoint);
                    }
                    else
                    {
                        // we have to march on the right side of the funnel, to see the edge
                        LinkedListNode<IVector> node = apex;

                        while ((var.ClockWise(vacp)) && (node.Next.Next != null)) // TODO: next.next béna..
                        {
                            minpathlength += node.Value.Distance(node.Next.Value);

                            node = node.Next;
                            //path.Add(node.Value); TODO: guinak

                            closestPoint = edge.ClosestPointOnEdgeFrom(node.Value);
                            apexPoint = node.Value;
                            left_1 = node.Previous.Value;
                            right_1 = node.Next.Value;

                            val = left_1.Minus(apexPoint); // vector from apex to the left part of the funnel
                            var = right_1.Minus(apexPoint); // vector from apex to the right part of the funnel
                            vacp = closestPoint.Minus(apexPoint); // vector from apex to the closest point
                        }

                        closestPoint = edge.ClosestPointOnEdgeFrom(node.Value);
                        minpathlength += node.Value.Distance(closestPoint);
                        //path.Add(closestPoint);
                    }
                }
                else
                {
                    // we have to march on the left side of the funnel, to see the edge
                    LinkedListNode<IVector> node = apex;

                    while ((val.CounterClockWise(vacp)) && (node.Previous.Previous != null))
                    {
                        minpathlength += node.Value.Distance(node.Previous.Value);

                        node = node.Previous;
                        //path.Add(apex.Value);

                        closestPoint = edge.ClosestPointOnEdgeFrom(node.Value);
                        apexPoint = node.Value;
                        left_1 = node.Previous.Value;
                        right_1 = node.Next.Value;

                        val = left_1.Minus(apexPoint); // vector from apex to the left part of the funnel
                        var = right_1.Minus(apexPoint); // vector from apex to the right part of the funnel
                        vacp = closestPoint.Minus(apexPoint); // vector from apex to the closest point
                    }

                    closestPoint = edge.ClosestPointOnEdgeFrom(node.Value);
                    minpathlength += node.Value.Distance(closestPoint);
                    //path.Add(closestPoint);
                }
            }
            dgMin = minpathlength;
        }

        protected void UpdateHigherBoundOfPathToEdge(IEdge edge)
        {
            LinkedListNode<IVector> node = apex;
            double maxLeft = 0, maxRight = 0;

            while (node.Previous != null)
            {
                maxLeft += node.Value.Distance(node.Previous.Value);
                node = node.Previous;
            }

            node = apex;
            while (node.Next != null)
            {
                maxRight += node.Value.Distance(node.Next.Value);
                node = node.Next;
            }

            dgMax = Math.Max(maxLeft, maxRight);
        }

        public void FinalizePath(IVector goalPoint)
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

        private void UpdateHeuristicValue(IEdge edge, IVector[] goalPoints)
        {
            h = FindDistanceFromClosestGoalPoint(edge, goalPoints);
        }

        private double FindDistanceFromClosestGoalPoint(IEdge edge, IEnumerable<IVector> goals)
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

        public IEdge CurrentEdge
        {
            get { return currentEdge; }
        }

        internal IEnumerable<ITriangle> ExplorableTriangles => explorableTriangles;

        public TPAPath Clone()
        {
            return new TPAPath(this);
        }

        public IEnumerable<IVector> GetReachedGoalPoints(IVector[] goalPoints)
        {
            return goalPoints.Where(point => currentTriangle.ContainsPoint(point));
        }
    }
}
