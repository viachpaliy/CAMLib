using System;

namespace Ocl
{
    /// <summary>
    /// Represents a finite arc segment in 3D space specified by its end points (p1, p2).
    /// </summary>
    public class Arc
    {
        // 2D length of the segment in the xy-plane
        private double length; // 2d length
        // Radius of the arc
        private double radius;

        /// <summary>
        /// Start point.
        /// </summary>
        public Point p1;
        /// <summary>
        /// End point.
        /// </summary>
        public Point p2;
        /// <summary>
        /// Centre point.
        /// </summary>
        public Point c;
        /// <summary>
        /// Direction true for anti-clockwise.
        /// </summary>
        public bool dir;

        public Arc() 
        {
            p1 = new Point();
            p2 = new Point();
            c = new Point();
            dir = true; // default to counter-clockwise
        }

        /// <summary>
        /// Create an arc from point p1 to point p2 with center c and direction dir.
        /// Direction is true for anti-clockwise arcs.
        /// </summary>
        public Arc(Point p1in, Point p2in, Point cin, bool dirin)
        {
            p1 = p1in;
            p2 = p2in;
            c = cin;
            dir = dirin;
            SetProperties();
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public Arc(Arc a)
        {
            p1 = a.p1;
            p2 = a.p2;
            c = a.c;
            dir = a.dir;
            SetProperties();
        }

        /// <summary>
        /// Returns the length of the arc.
        /// </summary>
        public double Length2d()
        {
            return length;
        }

        /// <summary>
        /// Returns a point along the arc at parameter value t [0,1].
        /// </summary>
        public Point GetPoint(double t)
        {
            if (Math.Abs(t) < 1e-14)
                return p1;
            if (Math.Abs(t - 1.0) < 1e-14)
                return p2;

            double d = t * length;
            if (!dir) d = -d;
            Point v = p1 - c;
            v.XyRotate(d / radius);
            return v + c;
        }

        /// <summary>
        /// Returns the absolute included angle (in radians) between 
        /// two vectors v1 and v2 in the direction of dir ( true=acw  false=cw)
        /// </summary>
        public double XyIncludedAngle(Point v1, Point v2, bool dir = true)
        {
            int d = dir ? 1 : -1;
            double inc_ang = v1.Dot(v2);
            if (inc_ang > 1.0 - 1.0e-10)
                return 0;
            if (inc_ang < -1.0 + 1.0e-10)
                inc_ang = Math.PI;
            else
            {
                if (inc_ang > 1.0)
                    inc_ang = 1.0;
                inc_ang = Math.Acos(inc_ang);

                double x = v1.x * v2.y - v1.y * v2.x;
                if (d * x < 0)
                    inc_ang = 2 * Math.PI - inc_ang;
            }
            return d * inc_ang;
        }

        /// <summary>
        /// Set arc properties.
        /// </summary>
        private void SetProperties()
        {
            Point vs = (p1 - c).XyPerp();
            Point ve = (p2 - c).XyPerp();
            radius = vs.XyNorm();
            vs.Normalize();
            ve.Normalize();
            length = Math.Abs(XyIncludedAngle(vs, ve, dir)) * radius;
        }

        public override string ToString()
        {
            return $"({p1}, {p2}, {c}, {dir})";
        }
    }
}