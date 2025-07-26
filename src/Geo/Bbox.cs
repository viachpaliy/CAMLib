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

        // Constructor from two Points
        // Arguments: p1, p2
        public Bbox(Point p1, Point p2)
        {
            MinPt = new Point(Math.Min(p1.x, p2.x), Math.Min(p1.y, p2.y), Math.Min(p1.z, p2.z));
            MaxPt = new Point(Math.Max(p1.x, p2.x), Math.Max(p1.y, p2.y), Math.Max(p1.z, p2.z));
            initialized = true;
        }   

        // Copy constructor
        public Bbox(Bbox bbox)
        {
            Debug.Assert(bbox != null, "Bbox copy constructor called with null argument");
            MinPt = new Point(bbox.MinPt);
            MaxPt = new Point(bbox.MaxPt);
            initialized = bbox.initialized;
        }

        /// <summary>
        /// Gets the width of the bounding box along the X-axis.
        /// Returns 0 if the bounding box is empty.
        /// </summary>
        public double Width => IsEmpty ? 0 : MaxPt.x - MinPt.x;

        /// <summary>
        /// Gets the height of the bounding box along the Y-axis.
        /// Returns 0 if the bounding box is empty.
        /// </summary>
        public double Height => IsEmpty ? 0 : MaxPt.y - MinPt.y;

        /// <summary>
        /// Gets the depth of the bounding box along the Z-axis.
        /// Returns 0 if the bounding box is empty.
        /// </summary>
        public double Depth => IsEmpty ? 0 : MaxPt.z - MinPt.z;

        /// <summary>
        /// Gets the center point of the bounding box.
        /// Returns a default Point3D if the bounding box is empty.
        /// </summary>
        public Point Center => IsEmpty ? new Point() : new Point(
            (MinPt.x + MaxPt.x) / 2.0,
            (MinPt.y + MaxPt.y) / 2.0,
            (MinPt.z + MaxPt.z) / 2.0
        );

        /// <summary>
        /// Gets the size of the bounding box as a Vector3D.
        /// Returns a zero vector if the bounding box is empty.
        /// </summary>
        public Point Size => IsEmpty ? new Point() : new Point(Width, Height, Depth);



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
        /// Determines if this bounding box intersects with another bounding box.
        /// </summary>
        /// <param name="other">The other bounding box to check for intersection.</param>
        /// <returns>True if the bounding boxes intersect, false otherwise.</returns>
        public bool Intersects(Bbox other)
        {
            // If either bounding box is empty, they cannot intersect.
            if (this.IsEmpty || other.IsEmpty)
            {
                return false;
            }

            // Check for overlap on all three axes.
            // They intersect if and only if they overlap on X AND Y AND Z.
            bool overlapsX = this.MaxPt.x >= other.MinPt.x && other.MaxPt.x >= this.MinPt.x;
            bool overlapsY = this.MaxPt.y >= other.MinPt.y && other.MaxPt.y >= this.MinPt.y;
            bool overlapsZ = this.MaxPt.z >= other.MinPt.z && other.MaxPt.z >= this.MinPt.z;

            return overlapsX && overlapsY && overlapsZ;
        }


        /// <summary>
        /// Reset the Bbox (sets initialized=false)
        /// </summary>
        public void Clear()
        {
            initialized = false;
        }

        /// <summary>
        /// Return true if the Bbox is empty (no points added)
        /// </summary>
        public bool IsEmpty => !initialized;    


        /// <summary>
        /// Add another Bbox to the Bbox. Enlarges the Bbox so another Bbox is contained within it.
        /// <summary>
        public void AddBbox(Bbox b)
        {
            if (!initialized)
            {
                MaxPt = new Point(b.MaxPt);
                MinPt = new Point(b.MinPt);
                initialized = true;
            }
            else
            {
                if (b.MaxPt.x > MaxPt.x) MaxPt.x = b.MaxPt.x;
                if (b.MinPt.x < MinPt.x) MinPt.x = b.MinPt.x;

                if (b.MaxPt.y > MaxPt.y) MaxPt.y = b.MaxPt.y;
                if (b.MinPt.y < MinPt.y) MinPt.y = b.MinPt.y;

                if (b.MaxPt.z > MaxPt.z) MaxPt.z = b.MaxPt.z;
                if (b.MinPt.z < MinPt.z) MinPt.z = b.MinPt.z;
            }
        } 
     
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

         /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Bbox other = (Bbox)obj;
            return MinPt.x == other.MinPt.x && MinPt.y == other.MinPt.y && MinPt.z == other.MinPt.z &&
                   MaxPt.x == other.MaxPt.x && MaxPt.y == other.MaxPt.y && MaxPt.z == other.MaxPt.z;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            // Simple hash code combination for the properties
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + MinPt.x.GetHashCode();
                hash = hash * 23 + MinPt.y.GetHashCode();
                hash = hash * 23 + MinPt.z.GetHashCode();
                hash = hash * 23 + MaxPt.x.GetHashCode();
                hash = hash * 23 + MaxPt.y.GetHashCode();
                hash = hash * 23 + MaxPt.z.GetHashCode();
                return hash;
            }
        }
        public override string ToString()
        {
            return $"Bbox [MinPt.x={MinPt.x}, MinPt.y={MinPt.y}, MinPt.z={MinPt.z}, MaxPt.x={MaxPt.x}, MaxPt.y={MaxPt.y}, MaxPt.z={MaxPt.z}]\n";
        }
    }
}