using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonTools.Geometry;

namespace PathFinder.TPAStar
{
    public class TPAStarSolver
    {
        private LinkedList<TPAPath> openSet;
        private Dictionary<Edge, double> higherBoundOfPathToEdges;
        
        public TPAStarSolver()
        {
            openSet = new LinkedList<TPAPath>();
            higherBoundOfPathToEdges = new Dictionary<Edge, double>();    
        }   
        
        public Curve FindPath(Vector3 startPoint, ITriangle startTriangle, Vector3[] goals)
        {
            openSet.Clear();
            higherBoundOfPathToEdges.Clear();
            
            TPAPath initialPath = new TPAPath(startPoint);
            TriangleEvaluationResult startTriangleResult = initialPath.StepTo(startTriangle, goals);
            FireTriangleExploredEvent(startTriangle, startTriangleResult);
            openSet.AddFirst(initialPath);
            
            TPAPath optimalPath = initialPath;
            bool done = false;
            while ((openSet.Count > 0) && (!done))
            {
                TPAPath bestPath = openSet.First.Value;
                openSet.RemoveFirst();
                
                // two-level goaltest - second level
                if (bestPath.GoalReached) // if the first path of the openset is a finalized path to one of the goalVectorts
                {                          // then this is an optimal path, beacuse even the lower bound of every other path is bigger than the cost of this full path
                    optimalPath = bestPath;
                    done = true;
                }
                else
                {
                    // first level goaltest - if in the triangle on the end of this path contains goalpoints, we add the finalized paths to the openset
                    IEnumerable<Vector3> reachedGoalPoints = bestPath.GetReachedGoalPoints(goals);
                    foreach (Vector3 goalPoint in reachedGoalPoints)
                    {
                        TPAPath newPath = bestPath.Clone();
                        newPath.FinalizePath(goalPoint);
                        AddToOpenSetOrderedByEstimatedOverallCost(newPath);
                    }
                    
                    // adding new paths
                    var neighbourTriangles = bestPath.ExplorableTriangles;
                    foreach (Triangle t in neighbourTriangles)
                    {
                        TPAPath newPath = bestPath.Clone();
                        TriangleEvaluationResult result = newPath.StepTo(t, goals);
                        FireTriangleExploredEvent(t, result);
                        
                        if (PathMightBeShorterThanWhatWeAlreadyFound(newPath))
                        {
                            AddToOpenSetOrderedByEstimatedOverallCost(newPath);
                            UpdateHigherBoundsOfPathToEdges(newPath);
                        }
                    }
                }
            }
            return optimalPath.GetBuiltPath();
        }

        /// <summary>
        /// We exclude paths whose even the shortest possible path to the their edge is longer than
        /// the one, we already found during exploration. We also prevent the algorithm to enter 
        /// a loop around a hole in a polygon, but do not exclude possible shorter paths we found
        /// only later during triangle traversion.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool PathMightBeShorterThanWhatWeAlreadyFound(TPAPath path)
        {
            if (higherBoundOfPathToEdges.ContainsKey(path.CurrentEdge))
            {
                if (higherBoundOfPathToEdges[path.CurrentEdge] < path.ShortestPossiblePathLength)
                {
                    return false;
                }
            }
            return true;
        }
        
        private void UpdateHigherBoundsOfPathToEdges(TPAPath path)
        {
            if (higherBoundOfPathToEdges.ContainsKey(path.CurrentEdge))
            {
                if (higherBoundOfPathToEdges[path.CurrentEdge] > path.LongestPossiblePathLength)
                {
                    higherBoundOfPathToEdges[path.CurrentEdge] = path.LongestPossiblePathLength;
                }
            }
            else
            {
                higherBoundOfPathToEdges.Add(path.CurrentEdge, path.LongestPossiblePathLength);
            }
        }

        private void AddToOpenSetOrderedByEstimatedOverallCost(TPAPath path)
        {
            if ((openSet.First == null) || 
                (openSet.First.Value.EstimatedMinimalOverallCost > path.EstimatedMinimalOverallCost))
            {
                openSet.AddFirst(path);
            }
            else
            {
                var targetNode = openSet.First;
                while ((targetNode.Next != null) && 
                       (targetNode.Next.Value.EstimatedMinimalOverallCost < path.EstimatedMinimalOverallCost))
                {
                    targetNode = targetNode.Next;
                }
                openSet.AddAfter(targetNode, path);
            }
        }

        private void FireTriangleExploredEvent(ITriangle triangle, TriangleEvaluationResult result)
        {
            if (TriangleExplored != null)
            {
                TriangleExplored(triangle, result);
            }
        }
        
        public delegate void TriangleExploredEventHandler(ITriangle t, TriangleEvaluationResult result);

        public event TriangleExploredEventHandler TriangleExplored;
    }
}
