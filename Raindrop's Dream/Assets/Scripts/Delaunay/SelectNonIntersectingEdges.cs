using System.Collections.Generic;
using UnityEngine;

namespace Delaunay
{
    public class selectNonIntersectingEdgesClass
    {
        static BitmapData _keepOutMask;
        static Vector2 zeroVector2 = new Vector2();

        internal static List<Edge> SelectNonIntersectingEdges(BitmapData keepOutMask, List<Edge> edgesToTest)
        {
            if (keepOutMask == null)
            {
                return edgesToTest;
            }
            _keepOutMask = keepOutMask;
            return edgesToTest.Filter(MyTest);
        }

        static bool MyTest(Edge edge, int index, List<Edge> vector)
        {
            BitmapData delaunayLineBmp = edge.MakeDelaunayLineBmp();
            bool notIntersecting = !(_keepOutMask.hitTest(zeroVector2, 1, delaunayLineBmp, zeroVector2, 1));
            delaunayLineBmp.dispose();
            return notIntersecting;
        }
    }
}