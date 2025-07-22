using System;
using System.Text;
using System.Threading;

namespace Ocl
{
    /// <summary>
    /// Cutter-Location (CL) point.
    /// </summary>
    public class CLPoint : Point
    {
        /// <summary>
        /// Atomic reference to the corresponding CCPoint, protected against concurrent replacement in liftZ.
        /// </summary>
        private CCPoint _cc;
        private readonly object _ccLock = new object();

        /// <summary>
        /// Get or set the CCPoint atomically.
        /// </summary>
        public CCPoint CC
        {
            get { lock (_ccLock) { return _cc; } }
            set { lock (_ccLock) { _cc = value; } }
        }

        /// <summary>
        /// CLPoint at (0,0,0)
        /// </summary>
        public CLPoint() : base()
        {
            _cc = new CCPoint();
        }

        /// <summary>
        /// CLPoint at (x,y,z)
        /// </summary>
        public CLPoint(double x, double y, double z) : base(x, y, z)
        {
            _cc = new CCPoint();
        }

        /// <summary>
        /// CLPoint at (x,y,z) with CCPoint ccp
        /// </summary>
        public CLPoint(double x, double y, double z, CCPoint ccp) : base(x, y, z)
        {
            _cc = new CCPoint(ccp);
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        public CLPoint(CLPoint cl) : base(cl.x, cl.y, cl.z)
        {
            _cc = new CCPoint(cl.CC);
        }

        /// <summary>
        /// cl-point at Point p
        /// </summary>
        public CLPoint(Point p) : base(p.x, p.y, p.z)
        {
            _cc = new CCPoint();
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~CLPoint()
        {
            // nothing to do, managed memory
        }

        /// <summary>
        /// Return true if cl-point is below triangle
        /// </summary>
        public bool Below(Triangle t)
        {
            return z < t.bb.MaxPt.z;
        }

        /// <summary>
        /// If zin > z, lift CLPoint and return true
        /// </summary>
        public bool LiftZ(double zin)
        {
            if (zin > z)
            {
                z = zin;
                return true;
            }
            return false;
        }

        /// <summary>
        /// If zin > z, lift CLPoint and update cc-point, and return true
        /// Thread-safe.
        /// </summary>
        public bool LiftZ(double zin, CCPoint ccp)
        {
            if (zin > z)
            {
                z = zin;
                var newCC = new CCPoint(ccp);
                lock (_ccLock)
                {
                    _cc = newCC;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// If cc is in the edge p1-p2, test if clpoint needs to be lifted to z
        /// If so, set cc = cc_tmp and return true
        /// </summary>
        public bool LiftZIfInsidePoints(double zin, CCPoint cc_tmp, Point p1, Point p2)
        {
            if (cc_tmp.IsInside(p1, p2))
                return this.LiftZ(zin, cc_tmp);
            return false;
        }

        /// <summary>
        /// If cc is in Triangle facet, test if clpoint needs to be lifted
        /// If so, set cc=cc_tmp and return true
        /// </summary>
        public bool LiftZIfInFacet(double zin, CCPoint cc_tmp, Triangle t)
        {
            if (cc_tmp.IsInside(t))
                return this.LiftZ(zin, cc_tmp);
            return false;
        }

        /// <summary>
        /// Assignment
        /// </summary>
        public CLPoint Assign(CLPoint clp)
        {
            if (ReferenceEquals(this, clp))
                return this;
            x = clp.x;
            y = clp.y;
            z = clp.z;
            lock (_ccLock)
            {
                _cc = new CCPoint(clp.CC);
            }
            return this;
        }

        /// <summary>
        /// Addition with CLPoint
        /// </summary>
        public static CLPoint operator +(CLPoint a, CLPoint b)
        {
            return new CLPoint(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        /// <summary>
        /// Addition with Point
        /// </summary>
        public static CLPoint operator +(CLPoint a, Point b)
        {
            return new CLPoint(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        /// <summary>
        /// Return the CCPoint (for python)
        /// </summary>
        public CCPoint GetCC()
        {
            lock (_ccLock)
            {
                return _cc;
            }
        }

        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString()
        {
            return $"CL({x}, {y}, {z}) cc={_cc}";
        }

        /// <summary>
        /// String repr
        /// </summary>
        public string Str()
        {
            return this.ToString();
        }
    }
}