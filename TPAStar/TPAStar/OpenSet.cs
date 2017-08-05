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

        public bool IsExplorable(TPAPath p)
        {
            bool ret = true;

            double minOfGMaxToEdge = GetMinOfGMaxToEdge(p.CurrentEdge);
            if ((minOfGMaxToEdge != 0) && (minOfGMaxToEdge < p.GMin))
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
                if (openSet[i].CurrentEdge != null)
                {
                    if (openSet[i].CurrentEdge.Equals(e) && (openSet[i].GoalReached == false))
                    {
                        if (ret == 0)
                        {
                            ret = openSet[i].GMax;
                        }
                        else
                        {
                            ret = Math.Min(ret, openSet[i].GMax);
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
                    if (openSet[i].FMin > openSet[i + 1].FMin)
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
