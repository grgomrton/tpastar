using System;
using System.Collections.Generic;

namespace TriangulatedPolygonAStar
{
    public class TPAPath
    {
        private ITriangle currentTriangle;
        private IEdge currentEdge;
        private LinkedList<ITriangle> explorableTriangles;
        
        private FunnelStructure funnel;
        private double alreadyBuiltPathLength;                 // gPart
        private double lengthOfShortestPathFromApexToEdge;     // dgMin
        private double lengthOfLongestPathFromApexToEdge;      // dgMax
        private double distanceFromClosestGoalPoint;           // h

        private bool finalPathsHaveBeenBuilt;
        private bool isFinalized;

        private static double InitialValueForMinimumFinding = -1.0;

        /// <summary>
        /// Initializes a new instance of the <see cref="TPAPath"/> class which represents
        /// the result of stepping over the specified set of adjacent triangles.
        /// </summary>
        /// <param name="startPoint"></param>
        public TPAPath(IVector startPoint, ITriangle startTriangle)
        {
            funnel = new FunnelStructure(startPoint);
            currentTriangle = startTriangle;
            explorableTriangles = new LinkedList<ITriangle>(startTriangle.Neighbours);
            alreadyBuiltPathLength = 0;
            lengthOfShortestPathFromApexToEdge = 0;
            lengthOfLongestPathFromApexToEdge = 0;
            distanceFromClosestGoalPoint = 0; 
            finalPathsHaveBeenBuilt = false;
            isFinalized = false;
        }

        private TPAPath(TPAPath other)
        {
            funnel = new FunnelStructure(other.funnel);
            currentTriangle = other.currentTriangle;
            explorableTriangles = other.explorableTriangles;
            alreadyBuiltPathLength = other.alreadyBuiltPathLength;
            lengthOfShortestPathFromApexToEdge = other.lengthOfShortestPathFromApexToEdge;
            lengthOfLongestPathFromApexToEdge = other.lengthOfLongestPathFromApexToEdge;
            distanceFromClosestGoalPoint = other.distanceFromClosestGoalPoint;
            finalPathsHaveBeenBuilt = other.finalPathsHaveBeenBuilt;
            isFinalized = other.isFinalized;
        }

        /// <summary>
        /// The triangle we are currently standing on.
        /// </summary>
        public ITriangle CurrentTriangle
        {
            get { return currentTriangle; }
        }

        /// <summary>
        /// The edge where we entered the current triangle.
        /// </summary>
        public IEdge CurrentEdge
        {
            get { return currentEdge; }
        }
        
        /// <summary>
        /// The length of the possibly shortest path from the start to the current edge along the set of triangles
        /// stepped over while building this path.
        /// </summary>
        public double ShortestPathToEdgeLength
        {
            get { return alreadyBuiltPathLength + lengthOfShortestPathFromApexToEdge; }
        }

        /// <summary>
        /// The length of the longest possible path from the start to the current edge along the set of triangles
        /// stepped over while building this path.
        /// </summary>
        public double LongestPathToEdgeLength
        {
            get { return alreadyBuiltPathLength + lengthOfLongestPathFromApexToEdge; }
        }
        
        /// <summary>
        /// The length of the possibly shortest path from the start to the closest goal point along the set of triangles
        /// stepped over until this point.
        /// </summary>
        public double MinimalTotalCost
        {
            get { return ShortestPathToEdgeLength + distanceFromClosestGoalPoint; }
        }

        public bool FinalPathsHaveBeenBuilt
        {
            get { return finalPathsHaveBeenBuilt; }
        }
        
        /// <summary>
        /// Indicates, whether a goal point has been added to the end of this path and therefore is finalized.
        /// </summary>
        public bool Finalized
        {
            get { return isFinalized; }
        }

        /// <summary>
        /// Returns the path that has been built during this exploration.
        /// </summary>
        public LinkedList<IVector> Path
        {
            get { return funnel.Path; }
        }

        /// <summary>
        /// Indicates, whether the current triangle that this path is standing on contains any of the specified points.
        /// </summary>
        /// <param name="points">To points to check whether any of them is reached</param>
        /// <returns>true, if any of the points fall inside the current triangle, false otherwise</returns>
        public bool ReachedAnyOf(IEnumerable<IVector> points)
        {
            foreach (IVector point in points)
            {
                if (currentTriangle.ContainsPoint(point))
                {
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Indicates, whether after building final paths to the reached goal points
        /// there is any other goal pont that might be reached by further exploration.
        /// </summary>
        public bool IsAnyOtherGoalThatMightBeReached
        {
            get { return distanceFromClosestGoalPoint > InitialValueForMinimumFinding; }
        }

        public IEnumerable<TPAPath> ExploreNeighbourTriangles(IEnumerable<IVector> goals)
        {
            List<TPAPath> pathsToNeighbours = new List<TPAPath>();
            foreach (ITriangle neighbour in explorableTriangles)
            {
                TPAPath newPath = this.Clone();
                newPath.StepTo(neighbour, goals);
                pathsToNeighbours.Add(newPath);
            }
            return pathsToNeighbours;
        }

        public IEnumerable<TPAPath> BuildFinalizedPaths(IEnumerable<IVector> goals)
        {
            List<TPAPath> finalPaths = new List<TPAPath>();
            foreach (var goal in goals)
            {
                if (currentTriangle.ContainsPoint(goal))
                {
                    TPAPath newPath = this.Clone();
                    newPath.FinalizePath(goal);
                    finalPaths.Add(newPath);
                }
            }
            bool shouldIncludeGoalsInTriangle = false;
            if (currentEdge != null)
            {
                distanceFromClosestGoalPoint = 
                    MinimalDistanceBetween(currentEdge, goals, shouldIncludeGoalsInTriangle, currentTriangle);
            }
            else
            {
                distanceFromClosestGoalPoint = 
                    MinimalDistanceBetween(funnel.Apex.Value, goals, shouldIncludeGoalsInTriangle, currentTriangle);
            }

            finalPathsHaveBeenBuilt = true;
            return finalPaths;
        }
        
        private void StepTo(ITriangle targetTriangle, IEnumerable<IVector> goalPoints)
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

            funnel.StepOver(currentEdge);
            alreadyBuiltPathLength = LengthOfBuiltPathInFunnel(funnel.Path);
            lengthOfShortestPathFromApexToEdge = LengthOfShortestPathFromApexToEdge(currentEdge, funnel.Apex);
            lengthOfLongestPathFromApexToEdge = LengthOfLongestPathFromApexToEdge(funnel.Apex);
            
            finalPathsHaveBeenBuilt = false;
            bool shouldIncludeGoalsInTriangle = true;
            distanceFromClosestGoalPoint =
                MinimalDistanceBetween(currentEdge, goalPoints, shouldIncludeGoalsInTriangle, currentTriangle);
        }
        
        private void FinalizePath(IVector goalPoint)
        {
            funnel.FinalizePath(goalPoint);
            alreadyBuiltPathLength = LengthOfBuiltPathInFunnel(funnel.Path);
            lengthOfShortestPathFromApexToEdge = 0;
            lengthOfLongestPathFromApexToEdge = 0;
            distanceFromClosestGoalPoint = 0;
            isFinalized = true;
        }
        
        private TPAPath Clone()
        {
            return new TPAPath(this);
        }
        
        private static double LengthOfShortestPathFromApexToEdge(IEdge edge, LinkedListNode<IVector> apex)
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

        private static double WalkOnRightSideOfFunnelUntilClosestPointBecomesVisible(
            LinkedListNode<IVector> startNode, IEdge edge)
        {
            double pathLength = 0;
            LinkedListNode<IVector> currentNode = startNode;

            IVector closestPointOnEdge = edge.ClosestPointTo(currentNode.Value);
            IVector currentToOneRight = currentNode.Next.Value.Minus(currentNode.Value);
            IVector currentToClosestPoint = closestPointOnEdge.Minus(currentNode.Value);

            while (currentToOneRight.IsInCounterClockWiseDirectionFrom(currentToClosestPoint) &&
                   (currentNode.Next.Next != null))
            {
                pathLength += currentNode.Value.DistanceFrom(currentNode.Next.Value);
                currentNode = currentNode.Next;

                closestPointOnEdge = edge.ClosestPointTo(currentNode.Value);
                currentToOneRight = currentNode.Next.Value.Minus(currentNode.Value);
                currentToClosestPoint = closestPointOnEdge.Minus(currentNode.Value);
            }

            pathLength += currentNode.Value.DistanceFrom(edge.ClosestPointTo(currentNode.Value));

            return pathLength;
        }

        private static double WalkOnLeftSideOfFunnelUntilClosestPointBecomesVisible(
            LinkedListNode<IVector> startNode, IEdge edge)
        {
            double pathLength = 0;
            LinkedListNode<IVector> currentNode = startNode;

            IVector closestPointOnEdge = edge.ClosestPointTo(currentNode.Value);
            IVector currentToOneLeft = currentNode.Previous.Value.Minus(currentNode.Value);
            IVector currentToClosestPoint = closestPointOnEdge.Minus(currentNode.Value);

            while (currentToOneLeft.IsInClockWiseDirectionFrom(currentToClosestPoint) &&
                   (currentNode.Previous.Previous != null))
            {
                pathLength += currentNode.Value.DistanceFrom(currentNode.Previous.Value);
                currentNode = currentNode.Previous;

                closestPointOnEdge = edge.ClosestPointTo(currentNode.Value);
                currentToOneLeft = currentNode.Previous.Value.Minus(currentNode.Value);
                currentToClosestPoint = closestPointOnEdge.Minus(currentNode.Value);
            }

            pathLength += currentNode.Value.DistanceFrom(edge.ClosestPointTo(currentNode.Value));

            return pathLength;
        }

        private static double LengthOfLongestPathFromApexToEdge(LinkedListNode<IVector> apex)
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

        private static double LengthOfBuiltPathInFunnel(LinkedList<IVector> path)
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

        private static double MinimalDistanceBetween(
            IEdge edge, IEnumerable<IVector> targetPoints, 
            bool shouldIncludePointsInTriangle, ITriangle triangle)
        {
            double minDistance = InitialValueForMinimumFinding;
            foreach (var targetPoint in targetPoints)
            {
                bool pointFallsInTriangle = triangle.ContainsPoint(targetPoint);
                bool shouldEvaluate = !pointFallsInTriangle || (pointFallsInTriangle && shouldIncludePointsInTriangle);
                if (shouldEvaluate)
                {
                    double distance = edge.DistanceFrom(targetPoint);
                    if (minDistance < 0 || distance < minDistance)
                    {
                        minDistance = distance;
                    }                    
                }
            }
            return minDistance;
        }

        private static double MinimalDistanceBetween(
            IVector point, IEnumerable<IVector> targetPoints, 
            bool shouldIncludePointsInTriangle, ITriangle currentTriangle)
        {
            double minDistance = InitialValueForMinimumFinding;
            foreach (var targetPoint in targetPoints)
            {
                bool pointFallsInTriangle = currentTriangle.ContainsPoint(targetPoint);
                bool shouldEvaluate = !pointFallsInTriangle || (pointFallsInTriangle && shouldIncludePointsInTriangle);
                if (shouldEvaluate)
                {
                    double distance = point.DistanceFrom(targetPoint);
                    if (minDistance < 0 || distance < minDistance)
                    {
                        minDistance = distance;
                    }                    
                }
            }
            return minDistance;
        }

    }
}