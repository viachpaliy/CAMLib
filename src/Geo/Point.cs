using System;
using System.Diagnostics;

namespace Ocl
{
    public class Point
    {
        public double x, y, z;

        public Point() { x = 0; y = 0; z = 0; }
        public Point(double x, double y, double z) { this.x = x; this.y = y; this.z = z; }
        public Point(double x, double y) { this.x = x; this.y = y; this.z = 0.0; }
        public Point(Point p) { x = p.x; y = p.y; z = p.z; }

        public double Dot(Point p) => x * p.x + y * p.y + z * p.z;

        public Point Cross(Point p)
        {
            double xc = y * p.z - z * p.y;
            double yc = z * p.x - x * p.z;
            double zc = x * p.y - y * p.x;
            return new Point(xc, yc, zc);
        }

        public double Norm() => Math.Sqrt(Square(x) + Square(y) + Square(z));

        public void Normalize()
        {
            double norm = Norm();
            if (norm != 0.0)
            {
                x /= norm;
                y /= norm;
                z /= norm;
            }
        }

        public double XyNorm() => Math.Sqrt(Square(x) + Square(y));

        public void XyNormalize()
        {
            double norm = XyNorm();
            if (norm != 0.0)
            {
                x /= norm;
                y /= norm;
                // z stays the same
            }
        }

        public Point XyPerp() => new Point(-y, x, z);

        public void XyRotate(double cosa, double sina)
        {
            double temp = -y * sina + x * cosa;
            y = x * sina + cosa * y;
            x = temp;
        }

        public void XyRotate(double angle)
        {
            XyRotate(Math.Cos(angle), Math.Sin(angle));
        }

        public void XRotate(double theta)
        {
            MatrixRotate(1, 0, 0,
                         0, Math.Cos(theta), -Math.Sin(theta),
                         0, Math.Sin(theta), Math.Cos(theta));
        }

        public void YRotate(double theta)
        {
            MatrixRotate(Math.Cos(theta), 0, Math.Sin(theta),
                         0, 1, 0,
                         -Math.Sin(theta), 0, Math.Cos(theta));
        }

        public void ZRotate(double theta)
        {
            MatrixRotate(Math.Cos(theta), -Math.Sin(theta), 0,
                         Math.Sin(theta), Math.Cos(theta), 0,
                         0, 0, 1);
        }

        public void MatrixRotate(double a, double b, double c,
                                double d, double e, double f,
                                double g, double h, double i)
        {
            double xr = a * x + b * y + c * z;
            double yr = d * x + e * y + f * z;
            double zr = g * x + h * y + i * z;
            x = xr;
            y = yr;
            z = zr;
        }

        public double XyDistance(Point p)
        {
            return (this - p).XyNorm();
        }

        public double XyDistanceToLine(Point p1, Point p2)
        {
            if (p1.x == p2.x && p1.y == p2.y)
            {
                Debug.WriteLine("ERROR: Can't calculate distance from this to line through p1 and p2 in XY plane");
                return -1;
            }
            else
            {
                Point v = new Point(p2.y - p1.y, -(p2.x - p1.x), 0);
                v.Normalize();
                Point r = new Point(p1.x - x, p1.y - y, 0);
                return Math.Abs(v.Dot(r));
            }
        }

        public Point ClosestPoint(Point p1, Point p2)
        {
            Point v = p2 - p1;
            if (v.Norm() == 0.0) throw new Exception("ClosestPoint: p1 and p2 are the same point");
            double u = (this - p1).Dot(v) / v.Dot(v);
            return p1 + v * u;
        }

        public Point XyClosestPoint(Point p1, Point p2)
        {
            Point pt1 = p1;
            Point pt2 = p2;
            Point v = pt2 - pt1;
            if (IsZeroTol(v.XyNorm()))
            {
                Debug.WriteLine("ERROR: Can't calculate closest point in XY plane");
                throw new Exception("XyClosestPoint: p1 and p2 do not make a line in XY plane");
            }
            double u = (x - p1.x) * v.x + (y - p1.y) * v.y;
            u = u / (v.x * v.x + v.y * v.y);
            double x_ = p1.x + u * v.x;
            double y_ = p1.y + u * v.y;
            return new Point(x_, y_, 0);
        }

        public bool IsRight(Point p1, Point p2)
        {
            double a1 = p2.x - p1.x;
            double a2 = p2.y - p1.y;
            double t1 = a2;
            double t2 = -a1;
            double b1 = x - p1.x;
            double b2 = y - p1.y;
            double t = t1 * b1 + t2 * b2;
            return t > 1e-14;
        }

        public bool IsInside(Triangle t)
        {
            Point p = this;
            Point a = t.p[0];
            Point b = t.p[1];
            Point c = t.p[2];

            double u = (a.y * c.x - a.x * c.y + (c.y - a.y) * p.x + (a.x - c.x) * p.y)
                     / (a.y * c.x - a.x * c.y + (c.y - a.y) * b.x + (a.x - c.x) * b.y);

            double v = (a.x * b.y - a.y * b.x + (a.y - b.y) * p.x + (b.x - a.x) * p.y)
                     / (a.x * b.y - a.y * b.x + (a.y - b.y) * c.x + (b.x - a.x) * c.y);

            return u > 0.0 && v > 0.0 && (u + v) < 1.0;
        }

        public bool IsInside(Point p1, Point p2)
        {
            Point p2minusp1 = p2 - p1;
            Point thisminusp1 = this - p1;
            double t = thisminusp1.Dot(p2minusp1) / p2minusp1.Dot(p2minusp1);
            return t >= 0.0 && t <= 1.0;
        }

        public bool XParallel() => IsZeroTol(y) && IsZeroTol(z);

        public bool YParallel() => IsZeroTol(x) && IsZeroTol(z);

        public bool ZParallel() => x == 0.0 && y == 0.0;

        public void ZProjectOntoEdge(Point p1, Point p2)
        {
            double t;
            if (Math.Abs(p2.x - p1.x) > Math.Abs(p2.y - p1.y))
                t = (x - p1.x) / (p2.x - p1.x);
            else
                t = (y - p1.y) / (p2.y - p1.y);
            z = p1.z + t * (p2.z - p1.z);
        }

        // Operator overloads
        public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y, a.z + b.z);
        public static Point operator -(Point a, Point b) => new Point(a.x - b.x, a.y - b.y, a.z - b.z);
        public static Point operator *(Point p, double a) => new Point(p.x * a, p.y * a, p.z * a);

        public override bool Equals(object obj)
        {
            if (obj is Point p)
                return x == p.x && y == p.y && z == p.z;
            return false;
        }

        public static bool operator ==(Point a, Point b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool operator !=(Point a, Point b) => !(a == b);

        public override int GetHashCode() => x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();

        public override string ToString() => $"({x}, {y}, {z})";

        public string Str() => ToString();

        // Helper functions
        private static double Square(double val) => val * val;

        private static bool IsZeroTol(double val, double tol = 1e-9) => Math.Abs(val) < tol;
    }

   
}