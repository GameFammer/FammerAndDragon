using System;
using System.Collections.Generic;
using UnityEngine;

namespace Delaunay
{
    public class Polygon
    {
        private List<Vector2> _vertices;

        public Polygon(List<Vector2> vertices)
        {
            _vertices = vertices;
        }

        public float Area()
        {
            return Math.Abs(SignedDoubleArea() * 0.5f);
        }

        public Winding GetWinding()
        {
            float signedDoubleAreaVar = SignedDoubleArea();
            if (signedDoubleAreaVar < 0)
            {
                return Winding.CLOCKWISE;
            }
            if (signedDoubleAreaVar > 0)
            {
                return Winding.COUNTERCLOCKWISE;
            }
            return Winding.NONE;
        }

        private float SignedDoubleArea()
        {
            uint index, nextIndex;
            uint n = (uint)_vertices.Count;
            Vector2 point, next;
            float signedDoubleArea = 0;
            for (index = 0; index < n; ++index)
            {
                nextIndex = (index + 1) % n;
                point = _vertices[(int)index];
                next = _vertices[(int)nextIndex];
                signedDoubleArea += point.x * next.y - next.x * point.y;
            }
            return signedDoubleArea;
        }
    }
}