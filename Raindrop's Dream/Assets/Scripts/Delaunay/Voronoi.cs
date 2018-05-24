/*
 * The author of this software is Steven Fortune.  Copyright (c) 1994 by AT&T
 * Bell Laboratories.
 * Permission to use, copy, modify, and distribute this software for any
 * purpose without fee is hereby granted, provided that this entire notice
 * is included in all copies of any software which is or includes a copy
 * or modification of this software and in all copies of the supporting
 * documentation for such software.
 * THIS SOFTWARE IS BEING PROVIDED "AS IS", WITHOUT ANY EXPRESS OR IMPLIED
 * WARRANTY.  IN PARTICULAR, NEITHER THE AUTHORS NOR AT&T MAKE ANY
 * REPRESENTATION OR WARRANTY OF ANY KIND CONCERNING THE MERCHANTABILITY
 * OF THIS SOFTWARE OR ITS FITNESS FOR ANY PARTICULAR PURPOSE.
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Delaunay
{
    public class Voronoi : IDisposable
    {
        private SiteList _sites;
        private Dictionary<Vector2, Site> _sitesIndexedByLocation;
        private List<Triangle> _triangles;
        private List<Edge> _edges;


        // TODO generalize this so it doesn't have to be a Rect;
        // then we can make the fractal voronois-within-voronois
        private Rect _plotBounds;
        public Rect plotBounds
        {
            get
            {
                return _plotBounds;
            }
        }

        public void Dispose()
        {
            int i, n;
            if (_sites != null)
            {
                _sites.Dispose();
                _sites = null;
            }
            if (_triangles != null)
            {
                n = _triangles.Count;
                for (i = 0; i < n; ++i)
                {
                    _triangles[i].Dispose();
                }
                _triangles.Clear();
                _triangles = null;
            }
            if (_edges != null)
            {
                n = _edges.Count;
                for (i = 0; i < n; ++i)
                {
                    _edges[i].Dispose();
                }
                _edges.Clear();
                _edges = null;
            }
            _plotBounds = new Rect();
            _sitesIndexedByLocation = null;
        }

        public Voronoi(List<Vector2> Vector2s, List<uint> colors, Rect plotBounds)
        {
            _sites = new SiteList();
            _sitesIndexedByLocation = new Dictionary<Vector2, Site>();
            AddSites(Vector2s, colors);
            _plotBounds = plotBounds;
            _triangles = new List<Triangle>();
            _edges = new List<Edge>();
            FortunesAlgorithm();
        }

        private void AddSites(List<Vector2> Vector2s, List<uint> colors)
        {
            uint length = (uint)Vector2s.Count;
            for (uint i = 0; i < length; ++i)
            {
                AddSite(Vector2s[(int)i], colors != null ? colors[(int)i] : 0, (int)i);
            }
        }

        private void AddSite(Vector2 p, uint color, int index)
        {
            //throw new NotImplementedException("This was modified, might not work");
            System.Random random = new System.Random();
            float weight = (float)random.NextDouble() * 100;
            Site site = Site.Create(p, index, weight, color);
            _sites.Push(site);
            _sitesIndexedByLocation[p] = site;
        }

        public List<Edge> Edges()
        {
            return _edges;
        }

        public List<Vector2> Region(Vector2 p)
        {
            Site site = _sitesIndexedByLocation[p];
            if (site == null)
            {
                return new List<Vector2>();
            }
            return site.Region(_plotBounds);
        }

        // TODO: bug: if you call this before you call region(), something goes wrong :(
        public List<Vector2> NeighborSitesForSite(Vector2 coord)
        {
            List<Vector2> Vector2s = new List<Vector2>();
            Site site = _sitesIndexedByLocation[coord];
            if (site == null)
            {
                return Vector2s;
            }
            List<Site> sites = site.NeighborSites();
            foreach (Site neighbor in sites)
            {
                Vector2s.Add(neighbor.Coord());
            }
            return Vector2s;
        }

        public List<Circle> Circles()
        {
            return _sites.Circles();
        }

        public List<LineSegment> VoronoiBoundaryForSite(Vector2 coord)
        {
            return visibleLineSegmentsClass.VisibleLineSegments(selectEdgesForSiteVector2Class.SelectEdgesForSiteVector2(coord, _edges));
        }

        public List<LineSegment> DelaunayLinesForSite(Vector2 coord)
        {
            return DelaunayLinesForEdgesClass.DelaunayLinesForEdges(selectEdgesForSiteVector2Class.SelectEdgesForSiteVector2(coord, _edges));
        }

        public List<LineSegment> VoronoiDiagram()
        {
            return visibleLineSegmentsClass.VisibleLineSegments(_edges);
        }

        public List<LineSegment> DelaunayTriangulation()
        {
            return DelaunayTriangulation(null);
        }
        public List<LineSegment> DelaunayTriangulation(BitmapData keepOutMask)
        {
            return DelaunayLinesForEdgesClass.DelaunayLinesForEdges(selectNonIntersectingEdgesClass.SelectNonIntersectingEdges(keepOutMask, _edges));
        }

        public List<LineSegment> Hull()
        {
            return DelaunayLinesForEdgesClass.DelaunayLinesForEdges(HullEdges());
        }

        private List<Edge> HullEdges()
        {
            return _edges.Filter(MyTestHullEdges);
        }

        bool MyTestHullEdges(Edge edge, int index, List<Edge> vector)
        {
            return (edge.IsPartOfConvexHull());
        }

        public List<Vector2> HullVector2sInOrder()
        {
            List<Edge> hullEdges = HullEdges();

            List<Vector2> Vector2s = new List<Vector2>();
            if (hullEdges.Count == 0)
            {
                return Vector2s;
            }

            EdgeReorderer reorderer = new EdgeReorderer(hullEdges, typeof(Site));
            hullEdges = reorderer.Edges;
            List<LR> orientations = reorderer.EdgeOrientations;
            reorderer.Dispose();

            LR orientation;

            int n = hullEdges.Count;
            for (int i = 0; i < n; ++i)
            {
                Edge edge = hullEdges[i];
                orientation = orientations[i];
                Vector2s.Add(edge.GetSite(orientation).Coord());
            }
            return Vector2s;
        }

        public List<LineSegment> SpanningTree(BitmapData keepOutMask)
        {
            return SpanningTree("minimum", keepOutMask);
        }

        public List<LineSegment> SpanningTree()
        {
            return SpanningTree("minimum", null);
        }

        public List<LineSegment> SpanningTree(string type)
        {
            return SpanningTree(type, null);
        }
        public List<LineSegment> SpanningTree(string type, BitmapData keepOutMask)
        {
            List<Edge> edges = selectNonIntersectingEdgesClass.SelectNonIntersectingEdges(keepOutMask, _edges);
            List<LineSegment> segments = DelaunayLinesForEdgesClass.DelaunayLinesForEdges(edges);
            return Kruskal.GetKruskal(segments, type);
        }

        public List<List<Vector2>> Regions()
        {
            return _sites.Regions(_plotBounds);
        }

        public List<uint> SiteColors()
        {
            return SiteColors(null);
        }
        public List<uint> SiteColors(BitmapData referenceImage)
        {
            return _sites.SiteColors(referenceImage);
        }

        /**
         * 
         * @param proximityMap a BitmapData whose regions are filled with the site index values; see PlaneVector2sCanvas::fillRegions()
         * @param x
         * @param y
         * @return coordinates of nearest Site to (x, y)
         * 
         */
        public Vector2 NearestSiteVector2(BitmapData proximityMap, float x, float y)
        {
            return _sites.NearestSiteVector2(proximityMap, x, y);
        }

        public List<Vector2> SiteCoords()
        {
            return _sites.SiteCoords();
        }

        private void FortunesAlgorithm()
        {
            Site newSite, bottomSite, topSite, tempSite;
            Vertex v, vertex;
            Vector2 newintstar = Vector2.zero;
            LR leftRight;
            Halfedge lbnd, rbnd, llbnd, rrbnd, bisector;
            Edge edge;

            Rect dataBounds = _sites.GetSitesBounds();

            int sqrt_nsites = (int)(Math.Sqrt(_sites.Length + 4));
            HalfedgePriorityQueue heap = new HalfedgePriorityQueue(dataBounds.y, dataBounds.height, sqrt_nsites);
            EdgeList edgeList = new EdgeList(dataBounds.x, dataBounds.width, sqrt_nsites);
            List<Halfedge> halfEdges = new List<Halfedge>();
            List<Vertex> vertices = new List<Vertex>();

            Site bottomMostSite = _sites.Next();
            newSite = _sites.Next();

            for (; ; )
            {
                if (heap.Empty() == false)
                {
                    newintstar = heap.Min();
                }

                if (newSite != null
                && (heap.Empty() || CompareByYThenX(newSite, newintstar) < 0))
                {
                    /* new site is smallest */
                    //trace("smallest: new site " + newSite);

                    // Step 8:
                    lbnd = edgeList.EdgeListLeftNeighbor(newSite.Coord());	// the Halfedge just to the left of newSite
                    //trace("lbnd: " + lbnd);
                    rbnd = lbnd.edgeListRightNeighbor;		// the Halfedge just to the right
                    //trace("rbnd: " + rbnd);
                    bottomSite = RightRegion(lbnd, bottomMostSite);		// this is the same as leftRegion(rbnd)
                    // this Site determines the region containing the new site
                    //trace("new Site is in region of existing site: " + bottomSite);

                    // Step 9:
                    edge = Edge.CreateBisectingEdge(bottomSite, newSite);
                    //trace("new edge: " + edge);
                    _edges.Add(edge);

                    bisector = Halfedge.Create(edge, LR.LEFT);
                    halfEdges.Add(bisector);
                    // inserting two Halfedges into edgeList static readonlyitutes Step 10:
                    // insert bisector to the right of lbnd:
                    edgeList.Insert(lbnd, bisector);

                    // first half of Step 11:
                    if ((vertex = Vertex.Intersect(lbnd, bisector)) != null)
                    {
                        vertices.Add(vertex);
                        heap.Remove(lbnd);
                        lbnd.vertex = vertex;
                        lbnd.ystar = vertex.Y + newSite.Dist(vertex);
                        heap.Insert(lbnd);
                    }

                    lbnd = bisector;
                    bisector = Halfedge.Create(edge, LR.RIGHT);
                    halfEdges.Add(bisector);
                    // second Halfedge for Step 10:
                    // insert bisector to the right of lbnd:
                    edgeList.Insert(lbnd, bisector);

                    // second half of Step 11:
                    if ((vertex = Vertex.Intersect(bisector, rbnd)) != null)
                    {
                        vertices.Add(vertex);
                        bisector.vertex = vertex;
                        bisector.ystar = vertex.Y + newSite.Dist(vertex);
                        heap.Insert(bisector);
                    }

                    newSite = _sites.Next();
                }
                else if (heap.Empty() == false)
                {
                    /* intersection is smallest */
                    lbnd = heap.ExtractMin();
                    llbnd = lbnd.edgeListLeftNeighbor;
                    rbnd = lbnd.edgeListRightNeighbor;
                    rrbnd = rbnd.edgeListRightNeighbor;
                    bottomSite = LeftRegion(lbnd, bottomMostSite);
                    topSite = RightRegion(rbnd, bottomMostSite);
                    // these three sites define a Delaunay triangle
                    // (not actually using these for anything...)
                    //_triangles.Add(new Triangle(bottomSite, topSite, rightRegion(lbnd)));

                    v = lbnd.vertex;
                    v.SetIndex();
                    lbnd.edge.SetVertex(lbnd.leftRight, v);
                    rbnd.edge.SetVertex(rbnd.leftRight, v);
                    edgeList.Remove(lbnd);
                    heap.Remove(rbnd);
                    edgeList.Remove(rbnd);
                    leftRight = LR.LEFT;
                    if (bottomSite.Y > topSite.Y)
                    {
                        tempSite = bottomSite; bottomSite = topSite; topSite = tempSite; leftRight = LR.RIGHT;
                    }
                    edge = Edge.CreateBisectingEdge(bottomSite, topSite);
                    _edges.Add(edge);
                    bisector = Halfedge.Create(edge, leftRight);
                    halfEdges.Add(bisector);
                    edgeList.Insert(llbnd, bisector);
                    edge.SetVertex(LR.Other(leftRight), v);
                    if ((vertex = Vertex.Intersect(llbnd, bisector)) != null)
                    {
                        vertices.Add(vertex);
                        heap.Remove(llbnd);
                        llbnd.vertex = vertex;
                        llbnd.ystar = vertex.Y + bottomSite.Dist(vertex);
                        heap.Insert(llbnd);
                    }
                    if ((vertex = Vertex.Intersect(bisector, rrbnd)) != null)
                    {
                        vertices.Add(vertex);
                        bisector.vertex = vertex;
                        bisector.ystar = vertex.Y + bottomSite.Dist(vertex);
                        heap.Insert(bisector);
                    }
                }
                else
                {
                    break;
                }
            }

            // heap should be empty now
            heap.Dispose();
            edgeList.Dispose();

            foreach (Halfedge halfEdge in halfEdges)
            {
                halfEdge.ReallyDispose();
            }
            halfEdges.Clear();

            // we need the vertices to clip the edges
            foreach (Edge edge2 in _edges)
            {
                edge2.ClipVertices(_plotBounds);
            }
            // but we don't actually ever use them again!
            foreach (Vertex vertex2 in vertices)
            {
                vertex2.Dispose();
            }
            vertices.Clear();
        }

        Site LeftRegion(Halfedge he, Site bottomMostSite)
        {
            Edge edge = he.edge;
            if (edge == null)
            {
                return bottomMostSite;
            }
            return edge.GetSite(he.leftRight);
        }

        Site RightRegion(Halfedge he, Site bottomMostSite)
        {
            Edge edge = he.edge;
            if (edge == null)
            {
                return bottomMostSite;
            }
            return edge.GetSite(LR.Other(he.leftRight));
        }

        internal static float CompareByYThenX(Site s1, Site s2)
        {
            if (s1.Y < s2.Y) return -1;
            if (s1.Y > s2.Y) return 1;
            if (s1.X < s2.X) return -1;
            if (s1.X > s2.X) return 1;
            return 0;
        }

        internal static float CompareByYThenX(Site s1, Vector2 s2)
        {
            if (s1.Y < s2.y) return -1;
            if (s1.Y > s2.y) return 1;
            if (s1.X < s2.x) return -1;
            if (s1.X > s2.x) return 1;
            return 0;
        }

    }
}