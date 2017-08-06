using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonTools.Geometry;

namespace PathFinder.TPAStar
{
    class OpenSet
    {
        private List<TPAPath> openSet;

        public OpenSet()
        {
            openSet = new List<TPAPath>();
        }

        public void Add(TPAPath p)
        {
            openSet.Add(p);
            SortOpenSet();
        }

        public int Count
        {
            get { return openSet.Count; }
        }

        public TPAPath PopFirst()
        {
            TPAPath p = openSet[0];
            openSet.RemoveAt(0);
            return p;
        }

        public bool PathMightBeShorterThanWhatWeScheduledForExploring(TPAPath p)
        {
            bool ret = true;

            double minOfGMaxToEdge = GetMinOfGMaxToEdge(p.CurrentEdge);
            if ((minOfGMaxToEdge != 0) && (minOfGMaxToEdge < p.ShortestPossiblePathLength))
            {
                ret = false;
            }

            return ret;
        }

        private double GetMinOfGMaxToEdge(Edge e)
        {
            double ret = 0;

            for (int i = 0; i < openSet.Count; i++)
            {
                if (openSet[i].CurrentEdge != null) // TODO: if we wouldnt add the starttriangle to openset, we wouldnt need this check
                {
                    if (openSet[i].CurrentEdge.Equals(e) && (openSet[i].GoalReached == false))
                    {
                        if (ret == 0)
                        {
                            ret = openSet[i].LongestPossiblePathLength;
                        }
                        else
                        {
                            ret = Math.Min(ret, openSet[i].LongestPossiblePathLength);
                        }
                    }
                }
            }

            return ret;
        }

        private void SortOpenSet()
        {
            bool switched = true;
            TPAPath tmp;

            while (switched == true)
            {
                switched = false;
                for (int i = 0; i < openSet.Count - 1; i++)
                {
                    if (openSet[i].EstimatedMinimalOverallCost > openSet[i + 1].EstimatedMinimalOverallCost)
                    {
                        tmp = openSet[i];
                        openSet[i] = openSet[i + 1];
                        openSet[i + 1] = tmp;
                        switched = true;
                    }
                }
            }
        }

        public void Clear()
        {
            openSet.Clear();
        }
    }
}
