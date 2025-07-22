using System;
using System.Collections.Generic;

namespace Ocl
{
    /// <summary> Span type </summary>
    public enum SpanType
    {
        LineSpanType,
        ArcSpanType
    }

    /// <summary>
    /// A finite curve which returns Point objects along its length.
    /// Location along span is based on a parameter t for which 0 ≤ t ≤ 1.0
    /// </summary>
    public abstract class Span
    {
        public abstract SpanType Type { get; }
        public abstract double Length2D { get; }
        public abstract Point GetPoint(double t); // 0.0 to 1.0
    }

    /// <summary> Line Span </summary>
    public class LineSpan : Span
    {
        public Line Line { get; }

        public LineSpan(Line l) => Line = l;

        public override SpanType Type => SpanType.LineSpanType;
        public override double Length2D => Line.Length2d();
        public override Point GetPoint(double t) => Line.GetPoint(t);
    }

    /// <summary> Circular Arc Span </summary>
    public class ArcSpan : Span
    {
        public Arc Arc { get; }

        public ArcSpan(Arc a) => Arc = a;

        public override SpanType Type => SpanType.ArcSpanType;
        public override double Length2D => Arc.Length2d();
        public override Point GetPoint(double t) => Arc.GetPoint(t);
    }

    /// <summary>
    /// A collection of Span objects
    /// </summary>
    public class Path
    {
        public List<Span> SpanList { get; } = new List<Span>();

        public Path() { }

        public Path(Path p) { /* shallow copy is enough here */ }

        // FIXME: looks wrong – should be only one Append() that takes a Span
        public void Append(Line l) => SpanList.Add(new LineSpan(l));
        public void Append(Arc  a) => SpanList.Add(new ArcSpan(a));
    }
}