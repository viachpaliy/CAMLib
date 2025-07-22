using System;
using System.Diagnostics;

namespace Ocl
{
    /// <summary>
    /// Axis-aligned bounding-box
    /// </summary>
    public class Bbox
    {
        /// The maximum point
        public Point MaxPt { get; set; }

        /// The minimum point
        public Point MinPt { get; set; }

        /// False until one Point or one Triangle has been added
        private bool initialized;

        // Default constructor
        public Bbox()
        {
            MinPt = new Point(0, 0, 0);
            MaxPt = new Point(0, 0, 0);
            initialized = false;
        }

        // Explicit constructor
        // Arguments: minx, maxx, miny, maxy, minz, maxz
        public Bbox(double b1, double b2, double b3, double b4, double b5, double b6)
        {
            MinPt = new Point(b1, b3, b5);
            MaxPt = new Point(b2, b4, b6);
            initialized = true;
        }

        // Index into maxpt and minpt returning a value [minx maxx miny maxy minz maxz]
        public double this[int idx]
        {
            get
            {
                switch (idx)
                {
                    case 0: return MinPt.x;
                    case 1: return MaxPt.x;
                    case 2: return MinPt.y;
                    case 3: return MaxPt.y;
                    case 4: return MinPt.z;
                    case 5: return MaxPt.z;
                    default: throw new ArgumentOutOfRangeException(nameof(idx), "Index must be in 0..5");
                }
            }
        }

        /// <summary>
        /// Return true if Point p is inside this Bbox
        /// </summary>
        public bool IsInside(Point p)
        {
            if (!initialized) throw new InvalidOperationException("Bbox not initialized");
            if (p.x > MaxPt.x) return false;
            if (p.x < MinPt.x) return false;
            if (p.y > MaxPt.y) return false;
            if (p.y < MinPt.y) return false;
            if (p.z > MaxPt.z) return false;
            if (p.z < MinPt.z) return false;
            return true;
        }

        /// <summary>
        /// Return true if this overlaps Bbox b
        /// </summary>
        public bool Overlaps(Bbox b)
        {
            if ((this.MaxPt.x < b.MinPt.x) || (this.MinPt.x > b.MaxPt.x))
                return false;
            if ((this.MaxPt.y < b.MinPt.y) || (this.MinPt.y > b.MaxPt.y))
                return false;
            if ((this.MaxPt.z < b.MinPt.z) || (this.MinPt.z > b.MaxPt.z))
                return false;
            return true;
        }

        /// <summary>
        /// Reset the Bbox (sets initialized=false)
        /// </summary>
        public void Clear()
        {
            initialized = false;
        }

        /// <summary>
        /// Add a Point to the Bbox. Enlarges the Bbox so that p is contained within it.
        /// </summary>
        public void AddPoint(Point p)
        {
            if (!initialized)
            {
                MaxPt = new Point(p);
                MinPt = new Point(p);
                initialized = true;
            }
            else
            {
                if (p.x > MaxPt.x) MaxPt.x = p.x;
                if (p.x < MinPt.x) MinPt.x = p.x;

                if (p.y > MaxPt.y) MaxPt.y = p.y;
                if (p.y < MinPt.y) MinPt.y = p.y;

                if (p.z > MaxPt.z) MaxPt.z = p.z;
                if (p.z < MinPt.z) MinPt.z = p.z;
            }
        }

        /// <summary>
        /// Add each vertex of a Triangle to the Bbox.
        /// Enlarges the Bbox so that the Triangle is contained within it.
        /// Calls AddPoint() for each vertex of the Triangle.
        /// </summary>
        public void AddTriangle(Triangle t)
        {
            AddPoint(t.p[0]);
            AddPoint(t.p[1]);
            AddPoint(t.p[2]);
        }

        public override string ToString()
        {
            return $"Bbox\n min= {MinPt}\n max= {MaxPt}\n";
        }
    }
}