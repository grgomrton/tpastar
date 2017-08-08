using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonTools.Geometry;

namespace PathFinder.TPAStar
{
    public class TPAStarSolver
    {
        private OpenSet openSet;
        private Dictionary<Edge, double> higherBoundOfPathToEdges;
        
        public TPAStarSolver()
        {
            openSet = new OpenSet();
            higherBoundOfPathToEdges = new Dictionary<Edge, double>();    
        }   
        
        public Curve FindPath(Vector3 startPoint, Triangle startTriangle, Vector3[] goals)
        {
            openSet.Clear();
            higherBoundOfPathToEdges.Clear();
            
            TPAPath initialPath = new TPAPath(startPoint, startTriangle);
            openSet.Add(initialPath);
            TriangleEvaluationResult startTriangleResult = new TriangleEvaluationResult(0.0, 0.0, 0.0, 0.0);
            FireTriangleExploredEvent(startTriangle, startTriangleResult);
            
            TPAPath optimalPath = initialPath;
            bool done = false;
            while ((openSet.Count > 0) && (!done))
            {
                TPAPath bestPath = openSet.PopFirst();
                UpdateHigherBoundsOfPathToEdges(bestPath);
                
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
                        openSet.Add(newPath);
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
                            if (openSet.PathMightBeShorterThanWhatWeScheduledForExploring(newPath))
                            {
                                openSet.Add(newPath);
                            }
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
            if (path.CurrentEdge != null) 
            {
                if (higherBoundOfPathToEdges.ContainsKey(path.CurrentEdge))
                {
                    if (higherBoundOfPathToEdges[path.CurrentEdge] < path.ShortestPossiblePathLength)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        
        private void UpdateHigherBoundsOfPathToEdges(TPAPath path)
        {
            if (path.CurrentEdge != null) // currentedge is null at the initial path with only the startpoint
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
        }
        
        private void FireTriangleExploredEvent(Triangle triangle, TriangleEvaluationResult result)
        {
            if (TriangleExplored != null)
            {
                TriangleExplored(triangle, result);
            }
        }
        
        public delegate void TriangleExploredEventHandler(Triangle t, TriangleEvaluationResult result);

        public event TriangleExploredEventHandler TriangleExplored;
    }
}
