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
        private ExploredSet exploredSet;
        
        public TPAStarSolver()
        {
            openSet = new OpenSet();
            exploredSet = new ExploredSet();    
        }   
        
        public Curve FindPath(Vector3 startPoint, Triangle startTriangle, Vector3[] goals)
        {
            openSet.Clear();
            exploredSet.Clear();
            
            TPAPath initialPath = new TPAPath(startPoint, startTriangle);
            openSet.Add(initialPath);
            TriangleEvaluationResult startTriangleResult = new TriangleEvaluationResult(0.0, 0.0, 0.0, 0.0);
            InvokeTriangleExploredIfAnySubscriberExists(startTriangle, startTriangleResult);
            
            TPAPath optimalPath = initialPath;
            bool done = false;
            while ((openSet.Count > 0) && (!done))
            {
                TPAPath bestPath = openSet.PopFirst();
                exploredSet.Add(bestPath);

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
                    IEnumerable<Triangle> neighbourTriangles = bestPath.GetExplorableTriangles();
                    foreach (Triangle t in neighbourTriangles)
                    {
                        TPAPath newPath = bestPath.Clone();
                        TriangleEvaluationResult result = newPath.StepTo(t, goals);

                        if (exploredSet.IsExplorable(newPath))
                        {
                            if (openSet.IsExplorable(newPath))
                            {
                                openSet.Add(newPath);
                            }
                        }
                        
                        InvokeTriangleExploredIfAnySubscriberExists(t, result);
                    }
                }
            }
            return optimalPath.GetBuiltPath();
        }

        private void InvokeTriangleExploredIfAnySubscriberExists(Triangle triangle, TriangleEvaluationResult result)
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
