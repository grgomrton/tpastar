using System.Collections.Generic;

namespace TriangulatedPolygonAStar
{
    public class TPAStarPathFinder
    {
        private LinkedList<TPAPath> candidates; // open set
        private Dictionary<IEdge, double> higherBounds;
        
        public TPAStarPathFinder()
        {
            candidates = new LinkedList<TPAPath>();
            higherBounds = new Dictionary<IEdge, double>();    
        }   
        
        public LinkedList<IVector> FindPath(IVector startPoint, ITriangle startTriangle, IEnumerable<IVector> goals)
        {
            candidates.Clear();
            higherBounds.Clear();
            
            TPAPath initialPath = new TPAPath(startPoint, startTriangle);
            candidates.AddFirst(initialPath);
            FireTriangleExploredEvent(initialPath);
            
            TPAPath optimalPath = initialPath;
            bool done = false;
            while ((candidates.Count > 0) && !done)
            {
                TPAPath candidate = candidates.First.Value;
                candidates.RemoveFirst();
                
                // if the lowest-cost path of the openset is a finalized path then this is an optimal path, 
                // because even the lower bound of every other candidate is higher than the total cost of this one
                if (candidate.IsFinalized)    
                {                            
                    optimalPath = candidate;
                    done = true;
                }
                else
                {
                    if (candidate.ReachedAnyOf(goals) && !candidate.FinalPathsHaveBeenBuilt)
                    {
                        IEnumerable<TPAPath> finalizedPaths = candidate.BuildFinalizedPaths(goals);
                        foreach (TPAPath path in finalizedPaths)
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
                        IEnumerable<TPAPath> newCandidates = candidate.ExploreNeighbourTriangles(goals);
                        foreach (TPAPath newCandidate in newCandidates)
                        {
                            if (IsGoodCandidate(newCandidate))
                            {
                                AddToCandidates(newCandidate);
                                UpdateHigherBoundToReachedEdge(newCandidate);
                            }
                            
                            FireTriangleExploredEvent(newCandidate);
                        }
                    }
                }
            }
            return optimalPath.Path;
        }

        // By maintaining higher-bounds to edges we prevent the algorithm to enter into
        // a loop around a polygon hole, and also the evaluation of known to be bad candidates
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

        // open set is a list of paths ordered by their minimal overall cost
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
        
        public delegate void TriangleExploredEventHandler(ITriangle t, TriangleEvaluationResult result);

        public event TriangleExploredEventHandler TriangleExplored;
    }
}
