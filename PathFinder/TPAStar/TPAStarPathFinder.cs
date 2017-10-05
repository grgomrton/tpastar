using System.Collections.Generic;

namespace TriangulatedPolygonAStar
{
    public class TPAStarPathFinder
    {
        private LinkedList<TPAPath> candidates;             // open set
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
        
        public delegate void TriangleExploredEventHandler(ITriangle t, TriangleEvaluationResult result);

        public event TriangleExploredEventHandler TriangleExplored;
    }
}
