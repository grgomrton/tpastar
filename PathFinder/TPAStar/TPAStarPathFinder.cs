using System;
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
            // TODO add argument check or define result for unexpected inputs
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
                TPAPath bestPath = openSet.First.Value;
                openSet.RemoveFirst();
                
                // if the first path of the openset is a finalized path to one of the goal points
                // then this is an optimal path, beacuse even the lower bound of every other path is higher than the cost of this fully built path
                if (bestPath.GoalReached)    
                {                            
                    optimalPath = bestPath;
                    done = true;
                }
                else
                {
                    // if the triangle on the end of this path contains goalpoints, 
                    // we add finalized paths from the current path to the contained goals
                    IEnumerable<IVector> reachedGoalPoints = bestPath.SelectReachedGoals(goals);
                    foreach (IVector goalPoint in reachedGoalPoints)
                    {
                        TPAPath newPath = bestPath.Clone();
                        newPath.FinalizePath(goalPoint);
                        InsertToOpenSet(newPath);
                    }
                    
                    // further explore the triangle map through the current path
                    IEnumerable<ITriangle> neighbourTriangles = bestPath.ExplorableTriangles;
                    foreach (ITriangle neighbour in neighbourTriangles)
                    {
                        TPAPath newPath = bestPath.Clone();
                        newPath.StepTo(neighbour, goals);
                        if (IsPathGoodCandidate(newPath))
                        {
                            InsertToOpenSet(newPath);
                            UpdateHigherBoundsOfPathToEdges(newPath);
                        }
                        
                        FireTriangleExploredEvent(neighbour, newPath);
                    }
                }
            }
            return optimalPath.GetBuiltPath();
        }

        private bool IsPathGoodCandidate(TPAPath path)
        {
            if (higherBounds.ContainsKey(path.CurrentEdge))
            {
                if (higherBounds[path.CurrentEdge] < path.ShortestPathToEdgeLength)
                {
                    return false;
                }
            }
            return true;
        }

        // by maintaining higher-bounds to edges we prevent the algorithm to enter into
        // a loop around a polygon hole, and prevent the re-evaluation of paths already 
        // known to be bad candidates
        private void UpdateHigherBoundsOfPathToEdges(TPAPath path)
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
                (openSet.First.Value.EstimatedMinimalCost > path.EstimatedMinimalCost))
            {
                openSet.AddFirst(path);
            }
            else
            {
                LinkedListNode<TPAPath> targetNode = openSet.First;
                while ((targetNode.Next != null) && 
                       (targetNode.Next.Value.EstimatedMinimalCost < path.EstimatedMinimalCost))
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
