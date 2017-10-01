using System.Collections.Generic;

namespace TriangulatedPolygonAStar
{
    public class TPAStarPathFinder
    {
        private LinkedList<TPAPath> openSet;
        private Dictionary<IEdge, double> higherBounds;
        
        public TPAStarPathFinder()
        {
            openSet = new LinkedList<TPAPath>();
            higherBounds = new Dictionary<IEdge, double>();    
        }   
        
        public LinkedList<IVector> FindPath(IVector startPoint, ITriangle startTriangle, IEnumerable<IVector> goals)
        {
            openSet.Clear();
            higherBounds.Clear();
            
            TPAPath initialPath = new TPAPath(startPoint);
            initialPath.StepTo(startTriangle, goals);
            openSet.AddFirst(initialPath);
            FireTriangleExploredEvent(startTriangle, initialPath);
            
            TPAPath optimalPath = initialPath;
            bool done = false;
            while ((openSet.Count > 0) && !done)
            {
                TPAPath candidate = openSet.First.Value;
                openSet.RemoveFirst();
                
                // if the lowest-cost path of the openset is a finalized path then this is an optimal path, 
                // because even the lower bound of every other candidate is higher than the total cost of this one
                if (candidate.Finalized)    
                {                            
                    optimalPath = candidate;
                    done = true;
                }
                else
                {
                    if (candidate.ReachedAnyGoal(goals) && !candidate.FinalPathsHaveBeenBuilt)
                    {
                        IEnumerable<TPAPath> finalizedPaths = candidate.BuildFinalizedPaths(goals);
                        foreach (TPAPath path in finalizedPaths)
                        {
                            InsertToOpenSet(path);
                        }
                        if (candidate.IsAnyOtherGoalThatMightBeReached)
                        {
                            InsertToOpenSet(candidate);    
                        }
                    }
                    else
                    {
                        IEnumerable<ITriangle> neighbourTriangles = candidate.ExplorableTriangles;
                        foreach (ITriangle neighbour in neighbourTriangles)
                        {
                            TPAPath newCandidate = candidate.Clone();
                            newCandidate.StepTo(neighbour, goals);
                            if (IsGoodCandidate(newCandidate))
                            {
                                InsertToOpenSet(newCandidate);
                                UpdateHigherBoundToReachedEdge(newCandidate);
                            }
                        
                            FireTriangleExploredEvent(neighbour, newCandidate);
                        }
                    }
                }
            }
            return optimalPath.GetBuiltPath();
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

        // By maintaining higher-bounds to edges we prevent the algorithm to enter into
        // a loop around a polygon hole, and also the evaluation of known to be bad candidates
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
        private void InsertToOpenSet(TPAPath path)
        {
            if ((openSet.First == null) || 
                (openSet.First.Value.MinimalTotalCost > path.MinimalTotalCost))
            {
                openSet.AddFirst(path);
            }
            else
            {
                LinkedListNode<TPAPath> targetNode = openSet.First;
                while ((targetNode.Next != null) && 
                       (targetNode.Next.Value.MinimalTotalCost < path.MinimalTotalCost))
                {
                    targetNode = targetNode.Next;
                }
                openSet.AddAfter(targetNode, path);
            }
        }

        private void FireTriangleExploredEvent(ITriangle triangle, TPAPath resultingPath)
        {
            if (TriangleExplored != null)
            {
                TriangleEvaluationResult evaluationEventArgs = new TriangleEvaluationResult(resultingPath);
                TriangleExplored(triangle, evaluationEventArgs);
            }
        }
        
        public delegate void TriangleExploredEventHandler(ITriangle t, TriangleEvaluationResult result);

        public event TriangleExploredEventHandler TriangleExplored;
    }
}
