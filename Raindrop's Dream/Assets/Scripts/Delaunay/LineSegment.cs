using UnityEngine;

namespace Delaunay
{
    public class LineSegment
    {
        public Room p0;
        public Room p1;

        public LineSegment(Vector2 p0, Vector2 p1)
        {
            this.p0 = new Room();
            this.p1 = new Room();
            this.p0.position = p0;
            this.p1.position = p1;
        }

        public static float CompareLengthsMax(LineSegment segment0, LineSegment segment1)
        {
            float length0 = Utilities.Distance(segment0.p0.position, segment0.p1.position);
            float length1 = Utilities.Distance(segment1.p0.position, segment1.p1.position);
            if (length0 < length1)
            {
                return 1;
            }
            if (length0 > length1)
            {
                return -1;
            }
            return 0;
        }

        public static float CompareLengths(LineSegment edge0, LineSegment edge1)
        {
            return -CompareLengthsMax(edge0, edge1);
        }
    }
}