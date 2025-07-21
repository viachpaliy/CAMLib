using System;

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
                    case 0: return MinPt.X;
                    case 1: return MaxPt.X;
                    case 2: return MinPt.Y;
                    case 3: return MaxPt.Y;
                    case 4: return MinPt.Z;
                    case 5: return MaxPt.Z;
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
            if (p.X > MaxPt.X) return false;
            if (p.X < MinPt.X) return false;
            if (p.Y > MaxPt.Y) return false;
            if (p.Y < MinPt.Y) return false;
            if (p.Z > MaxPt.Z) return false;
            if (p.Z < MinPt.Z) return false;
            return true;
        }

        /// <summary>
        /// Return true if this overlaps Bbox b
        /// </summary>
        public bool Overlaps(Bbox b)
        {
            if ((this.MaxPt.X < b.MinPt.X) || (this.MinPt.X > b.MaxPt.X))
                return false;
            if ((this.MaxPt.Y < b.MinPt.Y) || (this.MinPt.Y > b.MaxPt.Y))
                return false;
            if ((this.MaxPt.Z < b.MinPt.Z) || (this.MinPt.Z > b.MaxPt.Z))
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
                if (p.X > MaxPt.X) MaxPt.X = p.X;
                if (p.X < MinPt.X) MinPt.X = p.X;

                if (p.Y > MaxPt.Y) MaxPt.Y = p.Y;
                if (p.Y < MinPt.Y) MinPt.Y = p.Y;

                if (p.Z > MaxPt.Z) MaxPt.Z = p.Z;
                if (p.Z < MinPt.Z) MinPt.Z = p.Z;
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