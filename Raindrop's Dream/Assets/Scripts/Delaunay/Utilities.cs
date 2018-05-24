using System;
using UnityEngine;

namespace Delaunay
{
    class Utilities
    {
        public static float Distance(Vector2 one, Vector2 two)
        {
            float x = (two.x - one.x) * (two.x - one.x);
            float y = (two.y - one.y) * (two.y - one.y);
            return (float)Math.Sqrt(x + y);
        }
    }
}