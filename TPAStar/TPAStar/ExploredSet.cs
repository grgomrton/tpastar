using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonTools.Geometry;

namespace PathFinder.TPAStar
{
    public class ExploredSet
    {
        private Dictionary<Edge, double> exploredSet;

        public ExploredSet()
        {
            exploredSet = new Dictionary<Edge, double>();
        }

        public void Add(TPAPath p)
        {
            Edge e = p.CurrentEdge;
            // currentedge is null at the initial path with only the startpoint
            if (e != null)
            {
                if (exploredSet.ContainsKey(e))
                {
                    exploredSet[e] = Math.Min(p.GMax, exploredSet[e]);
                }
                else
                {
                    exploredSet.Add(e, p.GMax);
                }
            }
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

            if (exploredSet.ContainsKey(e))
            {
                ret = exploredSet[e];
            }

            return ret;
        }

        public double GetContainedValue(Edge e)
        {
            return exploredSet[e];
        }

        public bool Contains(Edge e)
        {
            return exploredSet.ContainsKey(e);
        }

        public void Clear()
        {
            exploredSet.Clear();
        }

    }
}
