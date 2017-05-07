using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonTools.Geometry;

namespace PathFinder.Funnel
{
    public static class FunnelAlgorithm
    {
        // vector3 equals operátora is érték alapján egyeztet
        // Funnel - bal szélsőtől jobb szélsőig tartalmazza a vertexeket
        // Bal oldal vége - First
        // Jobb oldal vége - Last
        public static Curve FindPath(Vector3 start, Vector3 goal, Edge[] edges)
        {
            FunnelStructure funnel = new FunnelStructure(start);
            for (int i = 0; i < edges.Length; i++)
            {
                funnel.StepTo(edges[i]);
            }
            funnel.FinalizePath(goal);

            return funnel.Path;
        }
    }
}
