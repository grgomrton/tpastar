using System;
using System.Collections.Generic;
using System.Linq;

namespace TriangulatedPolygonAStar
{
    public class TPAPath
    {
        private ITriangle currentTriangle;
        private IEnumerable<ITriangle> explorableTriangles;
        private IEdge currentEdge;
        private FunnelStructure funnel;
        
        private double dgMin; // minimum length from apex to finaltriangle
        private double dgMax;
        private double h; // heuristic value
        private double gPart;
        private bool isGoalReached;
        
        internal TPAPath(IVector startPoint)
        {
            funnel = new FunnelStructure(startPoint);
            currentTriangle = null;
            explorableTriangles = null;
            gPart = 0;
            dgMin = 0;
            dgMax = 0;
            h = 0;
            isGoalReached = false;
        }

        private TPAPath(TPAPath other)
        {
            funnel = new FunnelStructure(other.funnel);
            currentTriangle = other.currentTriangle;
            explorableTriangles = other.explorableTriangles;
            gPart = other.gPart;
            dgMin = other.dgMin;
            dgMax = other.dgMax;
            h = other.h;
            isGoalReached = other.isGoalReached;
        }

        public TriangleEvaluationResult StepTo(ITriangle targetTriangle, IEnumerable<IVector> goalPoints)
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
                foreach (ITriangle neighbour in targetTriangle.Neighbours)
                {
                    if (!neighbour.Equals(currentTriangle))
                    {
                        explorableNeighbourList.AddLast(neighbour);
                    }
                }
                explorableTriangles = explorableNeighbourList;
                currentTriangle = targetTriangle;
                
                funnel.StepTo(currentEdge);
                UpdateLengthOfCurrentlyBuiltPath();
                UpdateLowerBoundOfPathToEdge(currentEdge);
                UpdateHigherBoundOfPathToEdge(currentEdge);
                UpdateHeuristicValue(currentEdge, goalPoints);
            }
            
            return new TriangleEvaluationResult(h, EstimatedMinimalOverallCost, ShortestPossiblePathLength, LongestPossiblePathLength);
        }

        private void UpdateLowerBoundOfPathToEdge(IEdge edge)
        {
            double minpathlength = 0;

            if ((funnel.Apex.Next != null) && (funnel.Apex.Previous != null)) // otherwise the apex lies on the edge, the path is already the minpath to the edge
            {
                IVector closestPoint = edge.ClosestPointOnEdgeFrom(funnel.Apex.Value);
                IVector apexPoint = funnel.Apex.Value;
                IVector left_1 = funnel.Apex.Previous.Value;
                IVector right_1 = funnel.Apex.Next.Value;

                IVector val = left_1.Minus(apexPoint); // vector from apex to the left part of the funnel
                IVector var = right_1.Minus(apexPoint); // vector from apex to the right part of the funnel
                IVector vacp = closestPoint.Minus(apexPoint); // vector from apex to the closest point

                // 
                if (val.IsInCounterClockWiseDirectionFrom(vacp))
                {
                    if (var.IsInClockWiseDirectionFrom(vacp))
                    {
                        // easy way, closest point is visible from apex
                        minpathlength += apexPoint.DistanceFrom(closestPoint);
                    }
                    else
                    {
                        // we have to march on the right side of the funnel, to see the edge
                        LinkedListNode<IVector> node = funnel.Apex;

                        while ((var.IsInCounterClockWiseDirectionFrom(vacp)) && (node.Next.Next != null)) // TODO: next.next béna..
                        {
                            minpathlength += node.Value.DistanceFrom(node.Next.Value);

                            node = node.Next;

                            closestPoint = edge.ClosestPointOnEdgeFrom(node.Value);
                            apexPoint = node.Value;
                            left_1 = node.Previous.Value;
                            right_1 = node.Next.Value;

                            val = left_1.Minus(apexPoint); // vector from apex to the left part of the funnel
                            var = right_1.Minus(apexPoint); // vector from apex to the right part of the funnel
                            vacp = closestPoint.Minus(apexPoint); // vector from apex to the closest point
                        }

                        closestPoint = edge.ClosestPointOnEdgeFrom(node.Value);
                        minpathlength += node.Value.DistanceFrom(closestPoint);
                    }
                }
                else
                {
                    // we have to march on the left side of the funnel, to see the edge
                    LinkedListNode<IVector> node = funnel.Apex;

                    while ((val.IsInClockWiseDirectionFrom(vacp)) && (node.Previous.Previous != null))
                    {
                        minpathlength += node.Value.DistanceFrom(node.Previous.Value);

                        node = node.Previous;

                        closestPoint = edge.ClosestPointOnEdgeFrom(node.Value);
                        apexPoint = node.Value;
                        left_1 = node.Previous.Value;
                        right_1 = node.Next.Value;

                        val = left_1.Minus(apexPoint); // vector from apex to the left part of the funnel
                        var = right_1.Minus(apexPoint); // vector from apex to the right part of the funnel
                        vacp = closestPoint.Minus(apexPoint); // vector from apex to the closest point
                    }

                    closestPoint = edge.ClosestPointOnEdgeFrom(node.Value);
                    minpathlength += node.Value.DistanceFrom(closestPoint);
                }
            }
            dgMin = minpathlength;
        }

        private void UpdateHigherBoundOfPathToEdge(IEdge edge)
        {
            LinkedListNode<IVector> node = funnel.Apex;
            double maxLeft = 0, maxRight = 0;

            while (node.Previous != null)
            {
                maxLeft += node.Value.DistanceFrom(node.Previous.Value);
                node = node.Previous;
            }

            node = funnel.Apex;
            while (node.Next != null)
            {
                maxRight += node.Value.DistanceFrom(node.Next.Value);
                node = node.Next;
            }

            dgMax = Math.Max(maxLeft, maxRight);
        }

        public void FinalizePath(IVector goalPoint)
        {
            funnel.FinalizePath(goalPoint);
            gPart = GetLengthOfBuiltPath();
            dgMin = 0;
            dgMax = 0;
            h = 0;
            isGoalReached = true;
        }

        public bool GoalReached
        {
            get { return isGoalReached; }
        }

        private void UpdateHeuristicValue(IEdge edge, IEnumerable<IVector> goalPoints)
        {
            h = FindDistanceFromClosestGoalPoint(edge, goalPoints);
        }

        private void UpdateLengthOfCurrentlyBuiltPath()
        {
            gPart = GetLengthOfBuiltPath();
        }
        
        private double GetLengthOfBuiltPath()
        {
            double length = 0;
            LinkedListNode<IVector> currentNode = funnel.Path.First;
            while (currentNode.Next != null)
            {
                length += currentNode.Value.DistanceFrom(currentNode.Next.Value);
                currentNode = currentNode.Next;
            }
            return length;
        }
        
        private double FindDistanceFromClosestGoalPoint(IEdge edge, IEnumerable<IVector> goals)
        {
            return goals.Min(point => edge.DistanceFromPoint(point));
        }

        public LinkedList<IVector> GetBuiltPath()
        {
            return funnel.Path;
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
            get { return gPart + dgMin; }
        }

        public double LongestPossiblePathLength
        {
            get { return gPart + dgMax; }
        }

        public IEdge CurrentEdge
        {
            get { return currentEdge; }
        }

        public IEnumerable<ITriangle> ExplorableTriangles
        {
            get { return explorableTriangles; }
        }

        public TPAPath Clone()
        {
            return new TPAPath(this);
        }

        public IEnumerable<IVector> GetReachedGoalPoints(IEnumerable<IVector> goalPoints)
        {
            return goalPoints.Where(point => currentTriangle.ContainsPoint(point));
        }
    }
}
