using System;

namespace Ocl
{
    /// <summary>
    /// A finite line segment in 3D space specified by its end points (p1, p2)
    /// </summary>
    public class Line
    {
        /// <summary>
        /// Start point
        /// </summary>
        public Point p1;

        /// <summary>
        /// End point
        /// </summary>
        public Point p2;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Line()
        {
            p1 = new Point();
            p2 = new Point();
        }

        /// <summary>
        /// Create a line from p1 to p2
        /// </summary>
        public Line(Point p1, Point p2)
        {
            this.p1 = new Point(p1);
            this.p2 = new Point(p2);
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        public Line(Line l)
        {
            this.p1 = new Point(l.p1);
            this.p2 = new Point(l.p2);
        }

        /// <summary>
        /// Return the length of the line-segment in the xy-plane
        /// </summary>
        public double Length2d()
        {
            return (p2 - p1).XyNorm();
        }

        /// <summary>
        /// Return a Point on the Line at parameter value t [0,1]
        /// </summary>
        public Point GetPoint(double t)
        {
            return (p2 - p1) * t + p1;
        }

        /// <summary>
        /// Return the point on the Line which is closest to Point p.
        /// </summary>
        public Point Near(Point p)
        {
            // Returns the near point from a line on the extended line
            Point v = p2 - p1;
            v.Normalize();
            double dp = (p - p1).Dot(v);
            return p1 + (v * dp);
        }

        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString()
        {
            return $"({p1}, {p2})";
        }
    }
}