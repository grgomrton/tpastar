using System.Collections.Generic;

namespace TriangulatedPolygonAStar
{
    /// <summary>
    /// A path finder which is able to determine the euclidean shortest path between one start and multiple goal points
    /// in a triangulated polygon with holes.
    /// </summary>
    public class TPAStarPathFinder
    {
        private LinkedList<TPAPath> candidates;             // open set
        private Dictionary<IEdge, double> higherBounds;
        
        /// <summary>
        /// Initializes a new instance of a <see cref="TPAStarPathFinder"/> class which can be used to find
        /// shortest paths in a trianglulated polygon with holes.
        /// </summary>
        public TPAStarPathFinder()
        {
            candidates = new LinkedList<TPAPath>();
            higherBounds = new Dictionary<IEdge, double>();    
        }   
        
        /// <summary>
        /// Method signature for handling <see cref="TriangleExplored"/> events.
        /// </summary>
        /// <param name="triangle">The triangle which has been stepped into</param>
        /// <param name="result">Information about the path which was the result of stepping into this triangle</param>
        public delegate void TriangleExploredEventHandler(ITriangle triangle, TriangleEvaluationResult result);

        /// <summary>
        /// An event raised after a triangle has been expanded through the traversal of the triangle graph.
        /// The same triangle might be reached multiple times.
        /// </summary>
        public event TriangleExploredEventHandler TriangleExplored;
        
        /// <summary>
        /// Executes a path finding on the triangle graph from the specified start point to the goal points. 
        /// If no goal can be reached then the result is a one element list containing the start point.
        /// </summary>
        /// <param name="startPoint">The point to originate the pathfinding from</param>
        /// <param name="startTriangle">The triangle which contains the start point</param>
        /// <param name="goals">The possible goal points</param>
        /// <returns>The list of points that define the euclidean shortest path between the start and the closest goal point</returns>
        public LinkedList<IVector> FindPath(IVector startPoint, ITriangle startTriangle, IEnumerable<IVector> goals)
        {
            candidates.Clear();
            higherBounds.Clear();
            
            TPAPath initialPath = new TPAPath(startPoint, startTriangle);
            AddToCandidates(initialPath);
            FireTriangleExploredEvent(initialPath);
            
            TPAPath optimalPath = initialPath;
            bool done = false;
            while ((candidates.Count > 0) && !done)
            {
                TPAPath candidate = candidates.First.Value;
                candidates.RemoveFirst();
                
                if (candidate.Finalized)    
                {                            
                    optimalPath = candidate;
                    done = true;
                }
                else
                {
                    if (candidate.ReachedAnyOf(goals) && !candidate.FinalPathsHaveBeenBuilt)
                    {
                        foreach (TPAPath path in candidate.BuildFinalizedPaths(goals))
                        {
                            AddToCandidates(path);
                        }
                        if (candidate.IsAnyOtherGoalThatMightBeReached)
                        {
                            AddToCandidates(candidate);    
                        }
                    }
                    else
                    {
                        foreach (TPAPath path in candidate.ExploreNeighbourTriangles(goals))
                        {
                            if (IsGoodCandidate(path))
                            {
                                AddToCandidates(path);
                                UpdateHigherBoundToReachedEdge(path);
                            }
                            
                            FireTriangleExploredEvent(path);
                        }
                    }
                }
            }
            return optimalPath.Path;
        }

        private bool IsGoodCandidate(TPAPath path)
        {
            bool isGoodCandidate = true;
            if (higherBounds.ContainsKey(path.CurrentEdge))
            {
                if (higherBounds[path.CurrentEdge] < path.ShortestPathToEdgeLength)
                {
                    isGoodCandidate = false;
                }
            }
            return isGoodCandidate;
        }

        private void UpdateHigherBoundToReachedEdge(TPAPath path)
        {
            if (higherBounds.ContainsKey(path.CurrentEdge))
            {
                if (higherBounds[path.CurrentEdge] > path.LongestPathToEdgeLength)
                {
                    higherBounds[path.CurrentEdge] = path.LongestPathToEdgeLength;
                }
            }
            else
            {
                higherBounds.Add(path.CurrentEdge, path.LongestPathToEdgeLength);
            }
        }

        private void AddToCandidates(TPAPath path)
        {
            if ((candidates.First == null) || 
                (candidates.First.Value.MinimalTotalCost > path.MinimalTotalCost))
            {
                candidates.AddFirst(path);
            }
            else
            {
                LinkedListNode<TPAPath> targetNode = candidates.First;
                while ((targetNode.Next != null) && 
                       (targetNode.Next.Value.MinimalTotalCost < path.MinimalTotalCost))
                {
                    targetNode = targetNode.Next;
                }
                candidates.AddAfter(targetNode, path);
            }
        }

        private void FireTriangleExploredEvent(TPAPath resultingPath)
        {
            if (TriangleExplored != null)
            {
                TriangleEvaluationResult evaluationEventArgs = new TriangleEvaluationResult(resultingPath);
                TriangleExplored(resultingPath.CurrentTriangle, evaluationEventArgs);
            }
        }
        
    }
}
