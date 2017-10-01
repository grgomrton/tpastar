using System;
using System.Collections.Generic;

namespace TriangulatedPolygonAStar
{
    public class TPAPath
    {
        private ITriangle currentTriangle;
        private IEnumerable<ITriangle> explorableTriangles;
        private IEdge currentEdge;
        private FunnelStructure funnel;
        
        private double alreadyBuiltPathLength;             // gPart
        private double lengthOfShortestPathFromApexToEdge; // dgMin
        private double lengthOfLongestPathFromApexToEdge;  // dgMax
        private double distanceOfClosestGoalPointToEdge;   // h
        private bool isGoalReached;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TPAPath"/> class which represents
        /// the result of stepping over the specified set of adjacent triangles.
        /// </summary>
        /// <param name="startPoint"></param>
        public TPAPath(IVector startPoint)
        {
            funnel = new FunnelStructure(startPoint);
            currentTriangle = null;
            explorableTriangles = null;
            alreadyBuiltPathLength = 0;
            lengthOfShortestPathFromApexToEdge = 0;
            lengthOfLongestPathFromApexToEdge = 0;
            distanceOfClosestGoalPointToEdge = 0;
            isGoalReached = false;
        }

        private TPAPath(TPAPath other)
        {
            funnel = new FunnelStructure(other.funnel);
            currentTriangle = other.currentTriangle;
            explorableTriangles = other.explorableTriangles;
            alreadyBuiltPathLength = other.alreadyBuiltPathLength;
            lengthOfShortestPathFromApexToEdge = other.lengthOfShortestPathFromApexToEdge;
            lengthOfLongestPathFromApexToEdge = other.lengthOfLongestPathFromApexToEdge;
            distanceOfClosestGoalPointToEdge = other.distanceOfClosestGoalPointToEdge;
            isGoalReached = other.isGoalReached;
        }

        /// <summary>
        /// The length of the possibly shortest path from the start to the closest goal point along the set of triangles
        /// stepped over during building this path.
        /// </summary>
        public double EstimatedMinimalCost
        {
            get { return ShortestPathToEdgeLength + distanceOfClosestGoalPointToEdge; }
        }

        /// <summary>
        /// The length of the possibly shortest path from the start to the current edge along the set of triangles
        /// stepped over during building this path.
        /// </summary>
        public double ShortestPathToEdgeLength
        {
            get { return alreadyBuiltPathLength + lengthOfShortestPathFromApexToEdge; }
        }

        /// <summary>
        /// The length of the longest possible path from the start to the current edge along the set of triangles
        /// stepped over during building this path.
        /// </summary>
        public double LongestPathToEdgeLength
        {
            get { return alreadyBuiltPathLength + lengthOfLongestPathFromApexToEdge; }
        }

        /// <summary>
        /// The edge we are currently standing on.
        /// </summary>
        public IEdge CurrentEdge
        {
            get { return currentEdge; }
        }

        /// <summary>
        /// The triangles which can be further explored from the triangle we are standing in.
        /// </summary>
        public IEnumerable<ITriangle> ExplorableTriangles
        {
            get { return explorableTriangles; }
        }
        
        /// <summary>
        /// Indicates, whether a last point has been added to this path, and therefore is finalized.
        /// </summary>
        public bool GoalReached
        {
            get { return isGoalReached; }
        }

        public TPAPath Clone()
        {
            return new TPAPath(this);
        }
        
        public void StepTo(ITriangle targetTriangle, IEnumerable<IVector> goalPoints)
        {
            if (GoalReached)
            {
                throw new InvalidOperationException(
                    "The path has already reached a final point, therefore no further exploration is possible");
            }
            
            if (currentTriangle == null) // this is the first triangle we step into
            {
                currentTriangle = targetTriangle;
                explorableTriangles = targetTriangle.Neighbours;
            }
            else // we are exploring a new triangle
            {
                currentEdge = currentTriangle.GetCommonEdgeWith(targetTriangle);
                LinkedList<ITriangle> explorableNeighbourList = new LinkedList<ITriangle>();
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
                alreadyBuiltPathLength = CalculateLengthOfAlreadyBuiltPath(funnel.Path);
                lengthOfShortestPathFromApexToEdge = CalculateLengthOfShortestPathFromApexToEdge(currentEdge, funnel.Apex);
                lengthOfLongestPathFromApexToEdge = CalculateLengthOfLongestPathFromApexToEdge(funnel.Apex);
                distanceOfClosestGoalPointToEdge = CalculateDistanceFromClosestGoalPoint(currentEdge, goalPoints);
            }
        }

        public IEnumerable<IVector> SelectReachedGoals(IEnumerable<IVector> goalPoints)
        {
            List<IVector> reachedGoals = new List<IVector>();
            foreach (var goalPoint in goalPoints)
            {
                if (currentTriangle.ContainsPoint(goalPoint))
                {
                    reachedGoals.Add(goalPoint);
                }
            }
            return reachedGoals;
        }
        
        public void FinalizePath(IVector goalPoint)
        {          
            funnel.FinalizePath(goalPoint);
            alreadyBuiltPathLength = CalculateLengthOfAlreadyBuiltPath(funnel.Path);
            lengthOfShortestPathFromApexToEdge = 0;
            lengthOfLongestPathFromApexToEdge = 0;
            distanceOfClosestGoalPointToEdge = 0;
            isGoalReached = true;
        }
        
        public LinkedList<IVector> GetBuiltPath()
        {
            return funnel.Path;
        }
        
        private static double CalculateLengthOfShortestPathFromApexToEdge(IEdge edge, LinkedListNode<IVector> apex)
        {
            double minpathlength = 0;

            if ((apex.Next != null) && (apex.Previous != null)) // otherwise the apex lies on the edge, the path is already the minpath to the edge
            {
                IVector closestPointOfEdgeToApex = edge.ClosestPointTo(apex.Value);
                IVector apexPoint = apex.Value;
                IVector apexToLeftOne = apex.Previous.Value.Minus(apex.Value);
                IVector apexToRightOne = apex.Next.Value.Minus(apex.Value);
                IVector apexToClosestPointOnEdge = closestPointOfEdgeToApex.Minus(apexPoint);

                if (apexToLeftOne.IsInCounterClockWiseDirectionFrom(apexToClosestPointOnEdge))
                {
                    if (apexToRightOne.IsInClockWiseDirectionFrom(apexToClosestPointOnEdge))
                    {
                        // easy way, closest point is visible from apex
                        minpathlength = apexPoint.DistanceFrom(closestPointOfEdgeToApex);
                    }
                    else
                    {
                        minpathlength = WalkOnRightSideOfFunnelUntilClosestPointBecomesVisible(apex, edge);
                    }
                }
                else
                {
                    minpathlength = WalkOnLeftSideOfFunnelUntilClosestPointBecomesVisible(apex, edge);
                }
            }
            return minpathlength;
        }

        private static double WalkOnRightSideOfFunnelUntilClosestPointBecomesVisible(LinkedListNode<IVector> startNode, IEdge edge)
        {
            double pathLength = 0;
            LinkedListNode<IVector> currentNode = startNode;
            
            IVector closestPointOfEdgeToCurrentNode = edge.ClosestPointTo(currentNode.Value);
            IVector currentNodeToRightOne = currentNode.Next.Value.Minus(currentNode.Value);
            IVector currentNodeToClosestPointOnEdge = closestPointOfEdgeToCurrentNode.Minus(currentNode.Value);
            
            while (currentNodeToRightOne.IsInCounterClockWiseDirectionFrom(currentNodeToClosestPointOnEdge) && (currentNode.Next.Next != null))
            {
                pathLength += currentNode.Value.DistanceFrom(currentNode.Next.Value);
                currentNode = currentNode.Next;
                
                closestPointOfEdgeToCurrentNode = edge.ClosestPointTo(currentNode.Value);
                currentNodeToRightOne = currentNode.Next.Value.Minus(currentNode.Value);
                currentNodeToClosestPointOnEdge = closestPointOfEdgeToCurrentNode.Minus(currentNode.Value);
            }

            pathLength += currentNode.Value.DistanceFrom(edge.ClosestPointTo(currentNode.Value));

            return pathLength;
        }
        
        private static double WalkOnLeftSideOfFunnelUntilClosestPointBecomesVisible(LinkedListNode<IVector> startNode, IEdge edge)
        {
            double pathLength = 0;
            LinkedListNode<IVector> currentNode = startNode;
            
            IVector closestPointOfEdgeToCurrentNode = edge.ClosestPointTo(currentNode.Value);
            IVector currentNodeToRightOne = currentNode.Previous.Value.Minus(currentNode.Value);
            IVector currentNodeToClosestPointOnEdge = closestPointOfEdgeToCurrentNode.Minus(currentNode.Value);
            
            while (currentNodeToRightOne.IsInCounterClockWiseDirectionFrom(currentNodeToClosestPointOnEdge) && (currentNode.Previous.Previous != null))
            {
                pathLength += currentNode.Value.DistanceFrom(currentNode.Previous.Value);
                currentNode = currentNode.Previous;
                
                closestPointOfEdgeToCurrentNode = edge.ClosestPointTo(currentNode.Value);
                currentNodeToRightOne = currentNode.Previous.Value.Minus(currentNode.Value);
                currentNodeToClosestPointOnEdge = closestPointOfEdgeToCurrentNode.Minus(currentNode.Value);
            }

            pathLength += currentNode.Value.DistanceFrom(edge.ClosestPointTo(currentNode.Value));

            return pathLength;
        }
        
        private static double CalculateLengthOfLongestPathFromApexToEdge(LinkedListNode<IVector> apex)
        {
            double leftPathLength = 0;
            double rightPathLength = 0;

            LinkedListNode<IVector> currentNode = apex;
            while (currentNode.Previous != null)
            {
                leftPathLength += currentNode.Value.DistanceFrom(currentNode.Previous.Value);
                currentNode = currentNode.Previous;
            }

            currentNode = apex;
            while (currentNode.Next != null)
            {
                rightPathLength += currentNode.Value.DistanceFrom(currentNode.Next.Value);
                currentNode = currentNode.Next;
            }

            return Math.Max(leftPathLength, rightPathLength);
        }

        private static double CalculateLengthOfAlreadyBuiltPath(LinkedList<IVector> path)
        {
            double length = 0;
            LinkedListNode<IVector> currentNode = path.First;
            while (currentNode.Next != null)
            {
                length += currentNode.Value.DistanceFrom(currentNode.Next.Value);
                currentNode = currentNode.Next;
            }
            return length;
        }
        
        private static double CalculateDistanceFromClosestGoalPoint(IEdge edge, IEnumerable<IVector> goals)
        {
            double minDistance = -1;
            foreach (var goal in goals)
            {
                double distance = edge.DistanceFromPoint(goal);
                if (minDistance < 0 || distance < minDistance)
                {
                    minDistance = distance;
                }
            }
            return minDistance;
        }

    }
}
