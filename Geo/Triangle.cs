using System;
using System.Collections.Generic;

namespace Ocl
{
    /// <summary>
    /// A Triangle defined by its three vertices
    /// </summary>
    public class Triangle
    {
        // The three vertex Points of the Triangle
        public Point[] p = new Point[3];
        // Normal vector
        public Point n;
        // Bounding box
        public Bbox bb;

        // Default constructor
        public Triangle()
        {
            p[0] = new Point(1, 0, 0);
            p[1] = new Point(0, 1, 0);
            p[2] = new Point(0, 0, 1);
            CalcNormal();
            CalcBB();
        }

        // Copy constructor
        public Triangle(Triangle t)
        {
            p[0] = new Point(t.p[0]);
            p[1] = new Point(t.p[1]);
            p[2] = new Point(t.p[2]);
            CalcNormal();
            CalcBB();
        }

        // Create a triangle with the vertices p1, p2, and p3
        public Triangle(Point p1, Point p2, Point p3)
        {
            p[0] = p1;
            p[1] = p2;
            p[2] = p3;
            CalcNormal();
            CalcBB();
        }

        /// <summary>
        /// Return true if Triangle is sliced by a z-plane at z=zcut.
        /// Modify p1 and p2 so that they are intersections of the triangle edges and the plane.
        /// </summary>
        public bool ZSliceVerts(out Point p1, out Point p2, double zcut)
        {
            p1 = null;
            p2 = null;

            if (zcut <= this.bb.MinPt.Z || zcut >= this.bb.MaxPt.Z)
                return false; // no zslice

            var below = new List<Point>();
            var above = new List<Point>();
            for (int m = 0; m < 3; ++m)
            {
                if (p[m].Z <= zcut)
                    below.Add(p[m]);
                else
                    above.Add(p[m]);
            }

            if (!(below.Count == 1 || below.Count == 2))
            {
                Console.WriteLine("Triangle.cs: ZSliceVerts() error while trying to z-slice");
                Console.WriteLine($" triangle={this}");
                Console.WriteLine($" zcut={zcut}");
                Console.WriteLine($"{above.Count} above points:");
                foreach (var pt in above)
                    Console.WriteLine($"   {pt}");
                Console.WriteLine($"{below.Count} below points:");
                foreach (var pt in below)
                    Console.WriteLine($"   {pt}");
            }

            if (below.Count == 2)
            {
                // find two new intersection points
                double t1 = (zcut - above[0].Z) / (below[0].Z - above[0].Z);
                double t2 = (zcut - above[0].Z) / (below[1].Z - above[0].Z);
                p1 = above[0] + (below[0] - above[0]) * t1;
                p2 = above[0] + (below[1] - above[0]) * t2;
                return true;
            }
            else if (below.Count == 1)
            {
                double t1 = (zcut - above[0].Z) / (below[0].Z - above[0].Z);
                double t2 = (zcut - above[1].Z) / (below[0].Z - above[1].Z);
                p1 = above[0] + (below[0] - above[0]) * t1;
                p2 = above[1] + (below[0] - above[1]) * t2;
                return true;
            }
            else
            {
                throw new InvalidOperationException("Unexpected triangle slicing state.");
            }
        }

        /// <summary>
        /// Rotate triangle xrot radians around X-axis, yrot radians around Y-axis and zrot radians around Z-axis
        /// </summary>
        public void Rotate(double xrot, double yrot, double zrot)
        {
            for (int nIdx = 0; nIdx < 3; ++nIdx)
            {
                p[nIdx].XRotate(xrot);
                p[nIdx].YRotate(yrot);
                p[nIdx].ZRotate(zrot);
            }
            CalcNormal();
            CalcBB();
        }

        /// <summary>
        /// Calculate, normalize, and set the Triangle normal
        /// </summary>
        protected void CalcNormal()
        {
            Point v1 = p[0] - p[1];
            Point v2 = p[0] - p[2];
            Point ntemp = v1.Cross(v2);
            ntemp.Normalize();
            n = new Point(ntemp.X, ntemp.Y, ntemp.Z);
        }

        /// <summary>
        /// Calculate bounding box values
        /// </summary>
        protected void CalcBB()
        {
            if (bb == null)
                bb = new Bbox();
            bb.Clear();
            bb.AddTriangle(this);
        }

        /// <summary>
        /// Return normal vector with positive z-coordinate 
        /// </summary>
        public Point UpNormal()
        {
            return (n.Z < 0) ? n * -1.0 : n;
        }

        public override string ToString()
        {
            return $"T: {p[0]} {p[1]} {p[2]} n={n}";
        }
    }
}