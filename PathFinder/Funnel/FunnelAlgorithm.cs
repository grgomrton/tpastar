﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TriangulatedPolygonAStar.Geometry;

namespace PathFinder.Funnel
{
    public static class FunnelAlgorithm
    {
        // vector3 equals operátora is érték alapján egyeztet
        // Funnel - bal szélsőtől jobb szélsőig tartalmazza a vertexeket
        // Bal oldal vége - First
        // Jobb oldal vége - Last
        public static Curve FindPath(IVector start, IVector goal, IEdge[] edges)
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
