using System;
using System.Collections.Generic;
using System.Linq;

namespace TriangulatedPolygonAStar
{
    /// <summary>
    /// Represents the state of a traversal through the triangle graph along a specific set of adjacent triangles.
    /// </summary>
    public class TPAPath
    {
        private ITriangle currentTriangle;
        private IEdge currentEdge;
        
        private FunnelStructure funnel;
        private double alreadyBuiltPathLength;                 // gPart
        private double lengthOfShortestPathFromApexToEdge;     // dgMin
        private double lengthOfLongestPathFromApexToEdge;      // dgMax
        private double distanceFromClosestGoalPoint;           // h

        private bool finalPathsAcquired;

        /// <summary>
        /// Initializes a new instance of the <see cref="TPAPath"/> class which can be used 
        /// to explore the triangle graph.
        /// </summary>
        /// <param name="startPoint">The point where the traversion originates from</param>
        /// <param name="startTriangle">The triangle which contains the start point</param>
        public TPAPath(IVector startPoint, ITriangle startTriangle)
        {
            CheckForNullArgument(startPoint, nameof(startPoint));
            CheckForNullArgument(startTriangle, nameof(startTriangle));
            
            funnel = new FunnelStructure(startPoint);
            currentTriangle = startTriangle;
            alreadyBuiltPathLength = 0;
            lengthOfShortestPathFromApexToEdge = 0;
            lengthOfLongestPathFromApexToEdge = 0;
            distanceFromClosestGoalPoint = 0; 
            finalPathsAcquired = false;
        }

        private TPAPath(TPAPath other)
        {
            CheckForNullArgument(other, nameof(other));
            
            funnel = new FunnelStructure(other.funnel);
            currentTriangle = other.currentTriangle;
            alreadyBuiltPathLength = other.alreadyBuiltPathLength;
            lengthOfShortestPathFromApexToEdge = other.lengthOfShortestPathFromApexToEdge;
            lengthOfLongestPathFromApexToEdge = other.lengthOfLongestPathFromApexToEdge;
            distanceFromClosestGoalPoint = other.distanceFromClosestGoalPoint;
            finalPathsAcquired = other.finalPathsAcquired;
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
        /// Until we step to the first neighbour triangle from the start triangle, this value is null.
        /// </summary>
        public IEdge CurrentEdge
        {
            get { return currentEdge; }
        }
        
        /// <summary>
        /// The length of the possibly shortest path from the start point to the current edge along the set of triangles
        /// that has been stepped over.
        /// </summary>
        public double ShortestPathToEdgeLength
        {
            get { return alreadyBuiltPathLength + lengthOfShortestPathFromApexToEdge; }
        }

        /// <summary>
        /// The length of the longest possible path from the start point to the current edge along the set of triangles
        /// that has been stepped over.
        /// </summary>
        public double LongestPathToEdgeLength
        {
            get { return alreadyBuiltPathLength + lengthOfLongestPathFromApexToEdge; }
        }
        
        /// <summary>
        /// The length of the possibly shortest path from the start point to the closest goal point along the set of 
        /// triangles that has been stepped over.
        /// Before the final paths have been at a specific state of an exploration, the goals that fall in the current 
        /// triangle are taken into account by calculating the minimal total cost. 
        /// Once the final paths have been built only goals that fall outside the current triangle are included in 
        /// this value.
        /// </summary>
        public double MinimalTotalCost
        {
            get { return ShortestPathToEdgeLength + distanceFromClosestGoalPoint; }
        }

        /// <summary>
        /// Indicates whether the final paths to the goals contained by the current triangle have been acquired.
        /// </summary>
        public bool FinalPathsAcquired
        {
            get { return finalPathsAcquired; }
            set { finalPathsAcquired = value; }
        }

        /// <summary>
        /// Returns the set of paths which can be built by proceeding the graph exploration with the approachable 
        /// neighbours of the current triangle. A path built to the neighbour triangle has a <see cref="FinalPathsAcquired"/>
        /// value of false.
        /// </summary>
        /// <param name="neighbour"></param>
        /// <param name="goals"></param>
        /// <returns>The set of the resulting paths, among each one of them is standing on one of the approachable neighbour triangles</returns>
        public TPAPath StepToNeighbour(ITriangle neighbour, IEnumerable<IVector> goals)
        {
            CheckForNullArgument(neighbour, nameof(neighbour));
            if (!CurrentTriangle.Neighbours.Contains(neighbour))
            {
                throw new ArgumentException("The triangle is not adjacent with the one this path is standing on", nameof(neighbour));
            }
            
            TPAPath newPath = this.Clone();
            newPath.StepTo(neighbour, goals);
            
            return newPath;
        }

        /// <summary>
        /// Returns the finalized paths to the goal points which fall inside the triangle this path currently stands on.
        /// </summary>
        /// <param name="goal"></param>
        /// <returns>The set of finalized paths to the reached goal points</returns>
        public LinkedList<IVector> CompletePathTo(IVector goal)
        {
            CheckForNullArgument(goal, nameof(goal));
            if (!currentTriangle.ContainsPoint(goal))
            {
                throw new ArgumentException("This path has not reached the specified goal point", nameof(goal));
            }

            FunnelStructure funnelForPathToGoal = new FunnelStructure(this.funnel);
            funnelForPathToGoal.FinalizePath(goal);
            
            return funnelForPathToGoal.Path;
        }
        
        /// <summary>
        /// Calculates the minimal length between the current edge and the closest goal point depending on, whether
        /// the paths reached by stepping into this triangle have been built, indicated by the 
        /// <see cref="FinalPathsAcquired"/> property. If they have not yet built, those goals are included, 
        /// otherwise they are excluded from the minimum finding. 
        /// </summary>
        /// <param name="goals"></param>
        public void UpdateEstimationToClosestGoalPoint(IEnumerable<IVector> goals)
        {
            bool shouldIncludeGoalsInCurrentTriangle = !FinalPathsAcquired ? true : false;
            if (currentEdge != null)
            {
                distanceFromClosestGoalPoint = 
                    MinimalDistanceBetween(currentEdge, goals, shouldIncludeGoalsInCurrentTriangle, currentTriangle);
            }
            else
            {
                distanceFromClosestGoalPoint = 
                    MinimalDistanceBetween(funnel.Apex.Value, goals, shouldIncludeGoalsInCurrentTriangle, currentTriangle);
            }
        }
        
        private void StepTo(ITriangle targetTriangle, IEnumerable<IVector> goals)
        {
            currentEdge = currentTriangle.GetCommonEdgeWith(targetTriangle);
            currentTriangle = targetTriangle;

            funnel.StepOver(currentEdge);
            alreadyBuiltPathLength = LengthOfBuiltPathInFunnel(funnel.Path);
            lengthOfShortestPathFromApexToEdge = LengthOfShortestPathFromApexToEdge(currentEdge, funnel.Apex);
            lengthOfLongestPathFromApexToEdge = LengthOfLongestPathFromApexToEdge(funnel.Apex);
            FinalPathsAcquired = false;
            UpdateEstimationToClosestGoalPoint(goals);
        }
        
        private TPAPath Clone()
        {
            return new TPAPath(this);
        }
        
        private static double LengthOfShortestPathFromApexToEdge(IEdge edge, LinkedListNode<IVector> apex)
        {
            double length = 0.0;

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
                        length = apexPoint.DistanceFrom(closestPointOfEdgeToApex);
                    }
                    else
                    {
                        length = WalkOnRightSideOfFunnelUntilClosestPointBecomesVisible(apex, edge);
                    }
                }
                else
                {
                    length = WalkOnLeftSideOfFunnelUntilClosestPointBecomesVisible(apex, edge);
                }
            }
            return length;
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

        private static double MinimalDistanceBetween(IEdge edge, IEnumerable<IVector> targetPoints, bool shouldIncludePointsInTriangle, ITriangle triangle)
        {
            double minDistance = Double.PositiveInfinity;
            foreach (var targetPoint in targetPoints)
            {
                bool pointFallsInTriangle = triangle.ContainsPoint(targetPoint);
                bool shouldEvaluate = !pointFallsInTriangle || (pointFallsInTriangle && shouldIncludePointsInTriangle);
                if (shouldEvaluate)
                {
                    double distance = edge.DistanceFrom(targetPoint);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                    }                    
                }
            }
            return minDistance;
        }

        private static double MinimalDistanceBetween(IVector point, IEnumerable<IVector> targetPoints, bool shouldIncludePointsInTriangle, ITriangle currentTriangle)
        {
            double minDistance = Double.PositiveInfinity;
            foreach (var targetPoint in targetPoints)
            {
                bool pointFallsInTriangle = currentTriangle.ContainsPoint(targetPoint);
                bool shouldEvaluate = !pointFallsInTriangle || (pointFallsInTriangle && shouldIncludePointsInTriangle);
                if (shouldEvaluate)
                {
                    double distance = point.DistanceFrom(targetPoint);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                    }                    
                }
            }
            return minDistance;
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