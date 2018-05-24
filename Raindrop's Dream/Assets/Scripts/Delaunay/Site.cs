using System;
using System.Collections.Generic;
using UnityEngine;

namespace Delaunay
{
    public class Site : ICoord
    {
        private static List<Site> _pool = new List<Site>();
        public static Site Create(Vector2 p, int index, float weight, uint color)
        {
            if (_pool.Count > 0)
            {
                return _pool.Pop().Init(p, index, weight, color);
            }
            else
            {
                return new Site(typeof(PrivateConstructorEnforcer), p, index, weight, color);
            }
        }

        internal static void SortSites(List<Site> sites)
        {
            sites.SortFunc(Site.Compare);
        }

        /**
         * sort sites on y, then x, coord
         * also change each site's _siteIndex to match its new position in the list
         * so the _siteIndex can be used to identify the site for nearest-neighbor queries
         * 
         * haha "also" - means more than one responsibility...
         * 
         */
        private static float Compare(Site s1, Site s2)
        {
            int returnValue = (int)Voronoi.CompareByYThenX(s1, s2);

            // swap _siteIndex values if necessary to match new ordering:
            int tempIndex;
            if (returnValue == -1)
            {
                if (s1._siteIndex > s2._siteIndex)
                {
                    tempIndex = (int)s1._siteIndex;
                    s1._siteIndex = s2._siteIndex;
                    s2._siteIndex = (uint)tempIndex;
                }
            }
            else if (returnValue == 1)
            {
                if (s2._siteIndex > s1._siteIndex)
                {
                    tempIndex = (int)s2._siteIndex;
                    s2._siteIndex = s1._siteIndex;
                    s1._siteIndex = (uint)tempIndex;
                }

            }

            return returnValue;
        }


        private static readonly float EPSILON = .005f;
        private static bool CloseEnough(Vector2 p0, Vector2 p1)
        {
            return Utilities.Distance(p0, p1) < EPSILON;
        }

        private Vector2 _coord;

        public Vector2 Coord()
        {
            //get {
            return _coord;
            //}
        }

        internal uint color;
        internal float weight;

        private uint _siteIndex;

        // the edges that define this Site's Voronoi region:
        private List<Edge> _edges;

        internal List<Edge> Edges
        {
            get
            {
                return _edges;
            }
        }
        // which end of each edge hooks up with the previous edge in _edges:
        private List<LR> _edgeOrientations;
        // ordered list of Vector2s that define the region clipped to bounds:
        private List<Vector2> _region;

        public Site(Type pce, Vector2 p, int index, float weight, uint color)
        {
            if (pce != typeof(PrivateConstructorEnforcer))
            {
                throw new Exception("Site static readonlyructor is private");
            }
            Init(p, index, weight, color);
        }

        private Site Init(Vector2 p, int index, float weight, uint color)
        {
            _coord = p;
            _siteIndex = (uint)index;
            this.weight = weight;
            this.color = color;
            _edges = new List<Edge>();
            _region = null;
            return this;
        }

        public override string ToString()
        {
            return "Site " + _siteIndex + ": " + Coord().ToString();
        }

        private void Move(Vector2 p)
        {
            Clear();
            _coord = p;
        }

        public void Dispose()
        {
            _coord = Vector2.zero;
            Clear();
            _pool.Add(this);
        }

        private void Clear()
        {
            if (_edges != null)
            {
                _edges.Clear();
                _edges = null;
            }
            if (_edgeOrientations != null)
            {
                _edgeOrientations.Clear();
                _edgeOrientations = null;
            }
            if (_region != null)
            {
                _region.Clear();
                _region = null;
            }
        }

        internal void AddEdge(Edge edge)
        {
            _edges.Add(edge);
        }

        internal Edge NearestEdge()
        {
            _edges.SortFunc(Edge.CompareSitesDistances);
            return _edges[0];
        }

        internal List<Site> NeighborSites()
        {
            if (_edges == null || _edges.Count == 0)
            {
                return new List<Site>();
            }
            if (_edgeOrientations == null)
            {
                ReorderEdges();
            }
            List<Site> list = new List<Site>();
            foreach (Edge edge in _edges)
            {
                list.Add(NeighborSite(edge));
            }
            return list;
        }

        private Site NeighborSite(Edge edge)
        {
            if (this == edge.LeftSite)
            {
                return edge.RightSite;
            }
            if (this == edge.RightSite)
            {
                return edge.LeftSite;
            }
            return null;
        }

        internal List<Vector2> Region(Rect clippingBounds)
        {
            if (_edges == null || _edges.Count == 0)
            {
                return new List<Vector2>();
            }
            if (_edgeOrientations == null)
            {
                ReorderEdges();
                _region = ClipToBounds(clippingBounds);
                if ((new Polygon(_region)).GetWinding() == Winding.CLOCKWISE)
                {
                    _region.Reverse();
                }
            }
            return _region;
        }

        private void ReorderEdges()
        {
            //trace("_edges:", _edges);
            EdgeReorderer reorderer = new EdgeReorderer(_edges, typeof(Vertex));
            _edges = reorderer.Edges;
            //trace("reordered:", _edges);
            _edgeOrientations = reorderer.EdgeOrientations;
            reorderer.Dispose();
        }

        private List<Vector2> ClipToBounds(Rect bounds)
        {
            List<Vector2> Vector2s = new List<Vector2>();
            int n = _edges.Count;
            int i = 0;
            Edge edge;
            while (i < n && ((_edges[i] as Edge).Visible == false))
            {
                ++i;
            }

            if (i == n)
            {
                // no edges visible
                return new List<Vector2>();
            }
            edge = _edges[i];
            LR orientation = _edgeOrientations[i];
            Vector2s.Add(edge.ClippedEnds[orientation]);
            Vector2s.Add(edge.ClippedEnds[LR.Other(orientation)]);

            for (int j = i + 1; j < n; ++j)
            {
                edge = _edges[j];
                if (edge.Visible == false)
                {
                    continue;
                }
                Connect(Vector2s, j, bounds);
            }
            // close up the polygon by adding another corner Vector2 of the bounds if needed:
            Connect(Vector2s, i, bounds, true);

            return Vector2s;
        }

        private void Connect(List<Vector2> Vector2s, int j, Rect bounds)
        {
            Connect(Vector2s, j, bounds, false);
        }

        private void Connect(List<Vector2> Vector2s, int j, Rect bounds, bool closingUp)
        {
            Vector2 rightVector2 = Vector2s[Vector2s.Count - 1];
            Edge newEdge = _edges[j] as Edge;
            LR newOrientation = _edgeOrientations[j];
            // the Vector2 that  must be connected to rightVector2:
            Vector2 newVector2 = newEdge.ClippedEnds[newOrientation];
            if (!CloseEnough(rightVector2, newVector2))
            {
                // The Vector2s do not coincide, so they must have been clipped at the bounds;
                // see if they are on the same border of the bounds:
                if (rightVector2.x != newVector2.x
                && rightVector2.y != newVector2.y)
                {
                    // They are on different borders of the bounds;
                    // insert one or two corners of bounds as needed to hook them up:
                    // (NOTE this will not be corRect if the region should take up more than
                    // half of the bounds Rect, for then we will have gone the wrong way
                    // around the bounds and included the smaller part rather than the larger)
                    int rightCheck = BoundsCheck.Check(rightVector2, bounds);
                    int newCheck = BoundsCheck.Check(newVector2, bounds);
                    float px, py;
                    //throw new NotImplementedException("Modified, might not work");
                    if (rightCheck == BoundsCheck.RIGHT)
                    {
                        px = bounds.right;
                        if (newCheck == BoundsCheck.BOTTOM)
                        {
                            py = bounds.bottom;
                            Vector2s.Add(new Vector2(px, py));
                        }
                        else if (newCheck == BoundsCheck.TOP)
                        {
                            py = bounds.top;
                            Vector2s.Add(new Vector2(px, py));
                        }
                        else if (newCheck == BoundsCheck.LEFT)
                        {
                            if (rightVector2.y - bounds.y + newVector2.y - bounds.y < bounds.height)
                            {
                                py = bounds.top;
                            }
                            else
                            {
                                py = bounds.bottom;
                            }
                            Vector2s.Add(new Vector2(px, py));
                            Vector2s.Add(new Vector2(bounds.left, py));
                        }
                    }
                    else if (rightCheck == BoundsCheck.LEFT)
                    {
                        px = bounds.left;
                        if (newCheck == BoundsCheck.BOTTOM)
                        {
                            py = bounds.bottom;
                            Vector2s.Add(new Vector2(px, py));
                        }
                        else if (newCheck == BoundsCheck.TOP)
                        {
                            py = bounds.top;
                            Vector2s.Add(new Vector2(px, py));
                        }
                        else if (newCheck == BoundsCheck.RIGHT)
                        {
                            if (rightVector2.y - bounds.y + newVector2.y - bounds.y < bounds.height)
                            {
                                py = bounds.top;
                            }
                            else
                            {
                                py = bounds.bottom;
                            }
                            Vector2s.Add(new Vector2(px, py));
                            Vector2s.Add(new Vector2(bounds.right, py));
                        }
                    }
                    else if (rightCheck == BoundsCheck.TOP)
                    {
                        py = bounds.top;
                        if (newCheck == BoundsCheck.RIGHT)
                        {
                            px = bounds.right;
                            Vector2s.Add(new Vector2(px, py));
                        }
                        else if (newCheck == BoundsCheck.LEFT)
                        {
                            px = bounds.left;
                            Vector2s.Add(new Vector2(px, py));
                        }
                        else if (newCheck == BoundsCheck.BOTTOM)
                        {
                            if (rightVector2.x - bounds.x + newVector2.x - bounds.x < bounds.width)
                            {
                                px = bounds.left;
                            }
                            else
                            {
                                px = bounds.right;
                            }
                            Vector2s.Add(new Vector2(px, py));
                            Vector2s.Add(new Vector2(px, bounds.bottom));
                        }
                    }
                    else if (rightCheck == BoundsCheck.BOTTOM)
                    {
                        py = bounds.bottom;
                        if (newCheck == BoundsCheck.RIGHT)
                        {
                            px = bounds.right;
                            Vector2s.Add(new Vector2(px, py));
                        }
                        else if (newCheck == BoundsCheck.LEFT)
                        {
                            px = bounds.left;
                            Vector2s.Add(new Vector2(px, py));
                        }
                        else if (newCheck == BoundsCheck.TOP)
                        {
                            if (rightVector2.x - bounds.x + newVector2.x - bounds.x < bounds.width)
                            {
                                px = bounds.left;
                            }
                            else
                            {
                                px = bounds.right;
                            }
                            Vector2s.Add(new Vector2(px, py));
                            Vector2s.Add(new Vector2(px, bounds.top));
                        }
                    }
                }
                if (closingUp)
                {
                    // newEdge's ends have already been added
                    return;
                }
                Vector2s.Add(newVector2);
            }
            Vector2 newRightVector2 = newEdge.ClippedEnds[LR.Other(newOrientation)];
            if (!CloseEnough(Vector2s[0], newRightVector2))
            {
                Vector2s.Add(newRightVector2);
            }
        }


        internal float X
        {
            get
            {
                return _coord.x;
            }
        }

        internal float Y
        {
            get
            {
                return _coord.y;
            }
        }

        internal float Dist(ICoord p)
        {
            return Utilities.Distance(p.Coord(), this._coord);
        }
    }

    class BoundsCheck
    {
        public static readonly int TOP = 1;
        public static readonly int BOTTOM = 2;
        public static readonly int LEFT = 4;
        public static readonly int RIGHT = 8;

        /**
         * 
         * @param Vector2
         * @param bounds
         * @return an int with the appropriate bits set if the Vector2 lies on the corresponding bounds lines
         * 
         */
        public static int Check(Vector2 point, Rect bounds)
        {
            int value = 0;
            if (point.x == bounds.left)
            {
                value |= LEFT;
            }
            if (point.x == bounds.right)
            {
                value |= RIGHT;
            }
            if (point.y == bounds.top)
            {
                value |= TOP;
            }
            if (point.y == bounds.bottom)
            {
                value |= BOTTOM;
            }
            return value;
        }

        public BoundsCheck()
        {
            throw new Exception("BoundsCheck constructor unused");
        }
    }
}