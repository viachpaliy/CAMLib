using System;
using System.Text;

namespace Ocl
{
    /// <summary>
    /// Type of cc-point
    /// </summary>
    public enum CCType
    {
        NONE,
        VERTEX, VERTEX_CYL,
        EDGE, EDGE_HORIZ, EDGE_SHAFT,
        EDGE_HORIZ_CYL, EDGE_HORIZ_TOR, EDGE_BALL,
        EDGE_POS, EDGE_NEG, EDGE_CYL, EDGE_CONE, EDGE_CONE_BASE,
        FACET, FACET_TIP, FACET_CYL,
        ERROR
    }

    /// <summary>
    /// Cutter-Contact (CC) Point. A point with a CCType.
    /// </summary>
    public class CCPoint : Point
    {
        /// <summary>
        /// Specifies the type of the Cutter Contact point.
        /// </summary>
        public CCType type;

        /// <summary>
        /// Create a CCPoint at (0,0,0)
        /// </summary>
        public CCPoint() : base()
        {
            type = CCType.NONE;
        }

        /// <summary>
        /// Create a CCPoint at (x, y, z)
        /// </summary>
        public CCPoint(double x, double y, double z) : base(x, y, z)
        {
            type = CCType.NONE;
        }

        /// <summary>
        /// Create a CCPoint at (x, y, z) with type t
        /// </summary>
        public CCPoint(double x, double y, double z, CCType t) : base(x, y, z)
        {
            type = t;
        }

        /// <summary>
        /// Create a CCPoint at Point p
        /// </summary>
        public CCPoint(Point p) : base(p)
        {
            type = CCType.NONE;
        }

        /// <summary>
        /// Create a CCPoint at Point p with type t
        /// </summary>
        public CCPoint(Point p, CCType t) : base(p)
        {
            type = t;
        }

        /// <summary>
        /// Assign coordinates of Point to this CCPoint. Sets type=NONE
        /// </summary>
        public CCPoint Assign(Point p)
        {
            this.x = p.x;
            this.y = p.y;
            this.z = p.z;
            this.type = CCType.NONE;
            return this;
        }

        /// <summary>
        /// String representation of CCPoint
        /// </summary>
        public override string ToString()
        {
            return $"CC({x}, {y}, {z}, t={type})";
        }

        /// <summary>
        /// String repr (like str() in C++)
        /// </summary>
        public new string Str()
        {
            return this.ToString();
        }
    }
}