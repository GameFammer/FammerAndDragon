using System.Collections.Generic;
using UnityEngine;

namespace Delaunay
{
    public class selectEdgesForSiteVector2Class
    {
        static Vector2 _coord;
        internal static List<Edge> SelectEdgesForSiteVector2(Vector2 coord, List<Edge> edgesToTest)
        {
            _coord = coord;
            return edgesToTest.Filter(MyTest);
        }

        static bool MyTest(Edge edge, int index, List<Edge> vector)
        {
            return ((edge.LeftSite != null && edge.LeftSite.Coord() == _coord)
            || (edge.RightSite != null && edge.RightSite.Coord() == _coord));
        }
    }
}