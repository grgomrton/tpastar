using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonTools.Geometry;

namespace PathFinder.TPAStar
{
    public static class TPAStarAlgorithm
    {
        public static Curve FindPath(Vector3 start, Triangle startTriangle, Vector3[] goals)
        {
            OpenSet openSet = new OpenSet();
            ExploredSet exploredSet = new ExploredSet();

            TPAPath initialPath = new TPAPath(start, startTriangle);
            openSet.Add(initialPath);
            TPAPath optimalPath = initialPath;
            bool done = false;
            while ((openSet.Count > 0) && (!done))
            {
                TPAPath bestPath = openSet.PopFirst();
                exploredSet.Add(bestPath);

                bestPath.UpdateDisplayData(); // GUI only

                // two-level goaltest - second level
                if (bestPath.GoalReached) // if the first path of the openset is a finalized path to one of the goalVectorts
                {                          // then this is an optimal path, beacuse even the lower bound of every other path is bigger than the cost of this full path
                    optimalPath = bestPath;
                    done = true;
                }
                else
                {
                    // first level goaltest - if in the triangle on the end of this path contains goalpoints, we add the finalized paths to the openset
                    LinkedList<Vector3> reachedGoalPoints = bestPath.GetReachedGoalPoints(goals);
                    foreach (Vector3 goalPoint in reachedGoalPoints)
                    {
                        TPAPath newPath = bestPath.Clone();
                        newPath.FinalizePath(goalPoint);
                        openSet.Add(newPath);
                    }

                    // adding new paths
                    LinkedList<Triangle> neighbourTriangles = bestPath.GetExplorableTriangles();
                    foreach (Triangle t in neighbourTriangles)
                    {
                        TPAPath newPath = bestPath.Clone();
                        newPath.StepTo(t, goals);

                        if (exploredSet.IsExplorable(newPath))
                        {
                            if (openSet.IsExplorable(newPath))
                            {
                                openSet.Add(newPath);
                            }
                        }
                    }
                }
            }
            return optimalPath.GetBuiltPath();
        }
    }
}
