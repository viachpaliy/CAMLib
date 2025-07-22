using System;
using Xunit;
using Ocl;
namespace CAMLib.Geo.Tests;

public class PointTests
{
    private const double Tolerance = 1e-9; // Допустима похибка для порівняння чисел з плаваючою комою

    // Тести конструкторів
    [Fact]
    public void Constructor_Default_InitializesToZero()
    {
        var p = new Point();
        Assert.Equal(0.0, p.x, Tolerance);
        Assert.Equal(0.0, p.y, Tolerance);
        Assert.Equal(0.0, p.z, Tolerance);
    }

    [Fact]
    public void Constructor_ThreeDoubles_InitializesCorrectly()
    {
        var p = new Point(1.0, 2.0, 3.0);
        Assert.Equal(1.0, p.x, Tolerance);
        Assert.Equal(2.0, p.y, Tolerance);
        Assert.Equal(3.0, p.z, Tolerance);
    }

    [Fact]
    public void Constructor_TwoDoubles_InitializesZToZero()
    {
        var p = new Point(1.0, 2.0);
        Assert.Equal(1.0, p.x, Tolerance);
        Assert.Equal(2.0, p.y, Tolerance);
        Assert.Equal(0.0, p.z, Tolerance);
    }

    [Fact]
    public void Constructor_CopyPoint_InitializesCorrectly()
    {
        var original = new Point(1.0, 2.0, 3.0);
        var copy = new Point(original);
        Assert.Equal(original.x, copy.x, Tolerance);
        Assert.Equal(original.y, copy.y, Tolerance);
        Assert.Equal(original.z, copy.z, Tolerance);
        Assert.NotSame(original, copy); // Перевірка, що це копія, а не той самий об'єкт
    }

    // Тести математичних операцій
    [Fact]
    public void Dot_ReturnsCorrectScalarProduct()
    {
        var p1 = new Point(1.0, 2.0, 3.0);
        var p2 = new Point(4.0, 5.0, 6.0);
        double expected = 1.0 * 4.0 + 2.0 * 5.0 + 3.0 * 6.0; // 4 + 10 + 18 = 32
        Assert.Equal(expected, p1.Dot(p2), Tolerance);
    }

    [Fact]
    public void Cross_ReturnsCorrectVectorProduct()
    {
        var p1 = new Point(1.0, 0.0, 0.0); // i
        var p2 = new Point(0.0, 1.0, 0.0); // j
        var expected = new Point(0.0, 0.0, 1.0); // k (i x j = k)
        var result = p1.Cross(p2);
        Assert.Equal(expected.x, result.x, Tolerance);
        Assert.Equal(expected.y, result.y, Tolerance);
        Assert.Equal(expected.z, result.z, Tolerance);

        p1 = new Point(1.0, 2.0, 3.0);
        p2 = new Point(4.0, 5.0, 6.0);
        // xc = 2*6 - 3*5 = 12 - 15 = -3
        // yc = 3*4 - 1*6 = 12 - 6 = 6
        // zc = 1*5 - 2*4 = 5 - 8 = -3
        expected = new Point(-3.0, 6.0, -3.0);
        result = p1.Cross(p2);
        Assert.Equal(expected.x, result.x, Tolerance);
        Assert.Equal(expected.y, result.y, Tolerance);
        Assert.Equal(expected.z, result.z, Tolerance);
    }

    [Fact]
    public void Norm_ReturnsCorrectMagnitude()
    {
        var p = new Point(3.0, 4.0, 0.0);
        Assert.Equal(5.0, p.Norm(), Tolerance);

        p = new Point(1.0, 1.0, 1.0);
        Assert.Equal(Math.Sqrt(3), p.Norm(), Tolerance);
    }

    [Fact]
    public void Normalize_NormalizesVectorToUnitLength()
    {
        var p = new Point(3.0, 4.0, 0.0);
        p.Normalize();
        Assert.Equal(1.0, p.Norm(), Tolerance);
        Assert.Equal(0.6, p.x, Tolerance);
        Assert.Equal(0.8, p.y, Tolerance);
        Assert.Equal(0.0, p.z, Tolerance);
    }

    [Fact]
    public void Normalize_ZeroVector_RemainsZero()
    {
        var p = new Point(0.0, 0.0, 0.0);
        p.Normalize();
        Assert.Equal(0.0, p.x, Tolerance);
        Assert.Equal(0.0, p.y, Tolerance);
        Assert.Equal(0.0, p.z, Tolerance);
    }

    [Fact]
    public void XyNorm_ReturnsCorrect2DMagnitude()
    {
        var p = new Point(3.0, 4.0, 10.0);
        Assert.Equal(5.0, p.XyNorm(), Tolerance);
    }

    [Fact]
    public void XyNormalize_NormalizesXYComponentsToUnitLength()
    {
        var p = new Point(3.0, 4.0, 10.0);
        p.XyNormalize();
        Assert.Equal(1.0, p.XyNorm(), Tolerance);
        Assert.Equal(0.6, p.x, Tolerance);
        Assert.Equal(0.8, p.y, Tolerance);
        Assert.Equal(10.0, p.z, Tolerance); // Z should remain unchanged
    }

    [Fact]
    public void XyNormalize_ZeroXYVector_RemainsZeroXY()
    {
        var p = new Point(0.0, 0.0, 5.0);
        p.XyNormalize();
        Assert.Equal(0.0, p.x, Tolerance);
        Assert.Equal(0.0, p.y, Tolerance);
        Assert.Equal(5.0, p.z, Tolerance);
    }

    [Fact]
    public void XyPerp_ReturnsCorrectPerpendicularVector()
    {
        var p = new Point(1.0, 2.0, 3.0);
        var result = p.XyPerp();
        Assert.Equal(-2.0, result.x, Tolerance);
        Assert.Equal(1.0, result.y, Tolerance);
        Assert.Equal(3.0, result.z, Tolerance); // Z should remain unchanged
    }

    [Fact]
    public void XyRotate_Angle_RotatesCorrectly()
    {
        var p = new Point(1.0, 0.0, 0.0);
        p.XyRotate(Math.PI / 2); // Rotate 90 degrees counter-clockwise
        Assert.Equal(0.0, p.x, Tolerance);
        Assert.Equal(1.0, p.y, Tolerance);
        Assert.Equal(0.0, p.z, Tolerance); // Z should remain unchanged

        p = new Point(1.0, 1.0, 5.0);
        p.XyRotate(Math.PI); // Rotate 180 degrees
        Assert.Equal(-1.0, p.x, Tolerance);
        Assert.Equal(-1.0, p.y, Tolerance);
        Assert.Equal(5.0, p.z, Tolerance);
    }

    [Fact]
    public void XyRotate_CosSin_RotatesCorrectly()
    {
        var p = new Point(1.0, 0.0, 0.0);
        double cosa = Math.Cos(Math.PI / 2); // cos(90) = 0
        double sina = Math.Sin(Math.PI / 2); // sin(90) = 1
        p.XyRotate(cosa, sina);
        Assert.Equal(0.0, p.x, Tolerance);
        Assert.Equal(1.0, p.y, Tolerance);
        Assert.Equal(0.0, p.z, Tolerance);
    }

    [Fact]
    public void XRotate_RotatesAroundXAxis()
    {
        var p = new Point(0.0, 1.0, 0.0);
        p.XRotate(Math.PI / 2); // Rotate 90 degrees around X
        Assert.Equal(0.0, p.x, Tolerance);
        Assert.Equal(0.0, p.y, Tolerance);
        Assert.Equal(1.0, p.z, Tolerance);
    }

    [Fact]
    public void YRotate_RotatesAroundYAxis()
    {
        var p = new Point(1.0, 0.0, 0.0);
        p.YRotate(Math.PI / 2); // Rotate 90 degrees around Y
        Assert.Equal(0.0, p.x, Tolerance);
        Assert.Equal(0.0, p.y, Tolerance);
        Assert.Equal(-1.0, p.z, Tolerance);
    }

    [Fact]
    public void ZRotate_RotatesAroundZAxis()
    {
        var p = new Point(1.0, 0.0, 0.0);
        p.ZRotate(Math.PI / 2); // Rotate 90 degrees around Z
        Assert.Equal(0.0, p.x, Tolerance);
        Assert.Equal(1.0, p.y, Tolerance);
        Assert.Equal(0.0, p.z, Tolerance);
    }

    [Fact]
    public void MatrixRotate_AppliesCorrectTransformation()
    {
        var p = new Point(1.0, 2.0, 3.0);
        // Identity matrix
        p.MatrixRotate(1, 0, 0,
                       0, 1, 0,
                       0, 0, 1);
        Assert.Equal(1.0, p.x, Tolerance);
        Assert.Equal(2.0, p.y, Tolerance);
        Assert.Equal(3.0, p.z, Tolerance);

        p = new Point(1.0, 0.0, 0.0);
        // Rotation around Z by 90 degrees
        p.MatrixRotate(0, -1, 0,
                       1, 0, 0,
                       0, 0, 1);
        Assert.Equal(0.0, p.x, Tolerance);
        Assert.Equal(1.0, p.y, Tolerance);
        Assert.Equal(0.0, p.z, Tolerance);
    }

    [Fact]
    public void XyDistance_ReturnsCorrect2DDistance()
    {
        var p1 = new Point(0.0, 0.0, 10.0);
        var p2 = new Point(3.0, 4.0, 20.0);
        Assert.Equal(5.0, p1.XyDistance(p2), Tolerance);
    }

    [Fact]
    public void XyDistanceToLine_ReturnsCorrectDistance()
    {
        var p = new Point(0.0, 5.0, 0.0);
        var p1 = new Point(0.0, 0.0, 0.0);
        var p2 = new Point(10.0, 0.0, 0.0);
        // Точка (0,5) до лінії через (0,0) і (10,0)
        Assert.Equal(5.0, p.XyDistanceToLine(p1, p2), Tolerance);

        p = new Point(5.0, 5.0, 0.0);
        p1 = new Point(0.0, 0.0, 0.0);
        p2 = new Point(10.0, 10.0, 0.0);
        // Точка (5,5) на лінії через (0,0) і (10,10), відстань 0
        Assert.Equal(0.0, p.XyDistanceToLine(p1, p2), Tolerance);
    }

    [Fact]
    public void XyDistanceToLine_SamePoints_ReturnsNegativeOneAndLogsError()
    {
        var p = new Point(0.0, 5.0, 0.0);
        var p1 = new Point(1.0, 1.0, 0.0);
        var p2 = new Point(1.0, 1.0, 0.0);

        // Для перехоплення виводу Debug.WriteLine
        var listener = new StringWriter();
        Debug.Listeners.Add(new TextWriterTraceListener(listener));

        double result = p.XyDistanceToLine(p1, p2);

        Debug.Listeners.RemoveAt(Debug.Listeners.Count - 1); // Видаляємо слухача
        Debug.Flush(); // Забезпечуємо запис у слухача

        Assert.Equal(-1.0, result, Tolerance);
        Assert.Contains("ERROR: Can't calculate distance from this to line through p1 and p2 in XY plane", listener.ToString());
    }


    [Fact]
    public void ClosestPoint_ReturnsCorrectClosestPointOnLine()
    {
        var p = new Point(5.0, 0.0, 0.0); // Точка поза лінією
        var p1 = new Point(0.0, 0.0, 0.0); // Початок лінії
        var p2 = new Point(10.0, 0.0, 0.0); // Кінець лінії
        var expected = new Point(5.0, 0.0, 0.0);
        var result = p.ClosestPoint(p1, p2);
        Assert.Equal(expected.x, result.x, Tolerance);
        Assert.Equal(expected.y, result.y, Tolerance);
        Assert.Equal(expected.z, result.z, Tolerance);

        p = new Point(0.0, 5.0, 0.0); // Точка поза лінією
        p1 = new Point(0.0, 0.0, 0.0);
        p2 = new Point(0.0, 10.0, 0.0);
        expected = new Point(0.0, 5.0, 0.0);
        result = p.ClosestPoint(p1, p2);
        Assert.Equal(expected.x, result.x, Tolerance);
        Assert.Equal(expected.y, result.y, Tolerance);
        Assert.Equal(expected.z, result.z, Tolerance);

        p = new Point(15.0, 0.0, 0.0); // Точка за межами сегменту, але на лінії
        p1 = new Point(0.0, 0.0, 0.0);
        p2 = new Point(10.0, 0.0, 0.0);
        expected = new Point(15.0, 0.0, 0.0); // Проекція на лінію, а не сегмент
        result = p.ClosestPoint(p1, p2);
        Assert.Equal(expected.x, result.x, Tolerance);
        Assert.Equal(expected.y, result.y, Tolerance);
        Assert.Equal(expected.z, result.z, Tolerance);
    }

    [Fact]
    public void ClosestPoint_SamePoints_ThrowsException()
    {
        var p = new Point(5.0, 0.0, 0.0);
        var p1 = new Point(0.0, 0.0, 0.0);
        var p2 = new Point(0.0, 0.0, 0.0);
        Assert.Throws<Exception>(() => p.ClosestPoint(p1, p2));
    }

    [Fact]
    public void XyClosestPoint_ReturnsCorrectClosestPointOnLineInXY()
    {
        var p = new Point(5.0, 0.0, 10.0); // Точка поза лінією
        var p1 = new Point(0.0, 0.0, 0.0); // Початок лінії
        var p2 = new Point(10.0, 0.0, 0.0); // Кінець лінії
        var expected = new Point(5.0, 0.0, 0.0); // Z має бути 0
        var result = p.XyClosestPoint(p1, p2);
        Assert.Equal(expected.x, result.x, Tolerance);
        Assert.Equal(expected.y, result.y, Tolerance);
        Assert.Equal(expected.z, result.z, Tolerance); // Z має бути 0
    }

    [Fact]
    public void XyClosestPoint_PointsDoNotMakeLineInXY_ThrowsExceptionAndLogsError()
    {
        var p = new Point(0.0, 0.0, 0.0);
        var p1 = new Point(1.0, 1.0, 5.0);
        var p2 = new Point(1.0, 1.0, 10.0); // Ці точки не утворюють лінію в XY площині

        // Для перехоплення виводу Debug.WriteLine
        var listener = new StringWriter();
        Debug.Listeners.Add(new TextWriterTraceListener(listener));

        Assert.Throws<Exception>(() => p.XyClosestPoint(p1, p2));

        Debug.Listeners.RemoveAt(Debug.Listeners.Count - 1);
        Debug.Flush();

        Assert.Contains("ERROR: Can't calculate closest point in XY plane", listener.ToString());
    }

    [Fact]
    public void IsRight_ReturnsTrueIfPointIsToTheRight()
    {
        var p = new Point(1.0, -1.0, 0.0); // Точка праворуч від лінії (0,0)-(0,1)
        var p1 = new Point(0.0, 0.0, 0.0);
        var p2 = new Point(0.0, 1.0, 0.0);
        Assert.True(p.IsRight(p1, p2));

        p = new Point(-1.0, -1.0, 0.0); // Точка ліворуч
        Assert.False(p.IsRight(p1, p2));

        p = new Point(0.0, 0.5, 0.0); // Точка на лінії
        Assert.False(p.IsRight(p1, p2)); // Через tolerance, може бути false
    }

    [Fact]
    public void IsInside_Triangle_ReturnsTrueIfInside()
    {
        var p = new Point(0.5, 0.5, 0.0);
        var t = new Triangle(new Point(0, 0, 0), new Point(1, 0, 0), new Point(0, 1, 0));
        Assert.True(p.IsInside(t));

        p = new Point(1.0, 1.0, 0.0); // Поза трикутником
        Assert.False(p.IsInside(t));

        p = new Point(0.0, 0.0, 0.0); // На вершині
        Assert.False(p.IsInside(t)); // За визначенням (u > 0.0 && v > 0.0 && (u + v) < 1.0)
    }

    [Fact]
    public void IsInside_LineSegment_ReturnsTrueIfInsideSegment()
    {
        var p = new Point(5.0, 0.0, 0.0);
        var p1 = new Point(0.0, 0.0, 0.0);
        var p2 = new Point(10.0, 0.0, 0.0);
        Assert.True(p.IsInside(p1, p2));

        p = new Point(-1.0, 0.0, 0.0); // Поза сегментом
        Assert.False(p.IsInside(p1, p2));

        p = new Point(11.0, 0.0, 0.0); // Поза сегментом
        Assert.False(p.IsInside(p1, p2));

        p = new Point(0.0, 0.0, 0.0); // На початку сегмента
        Assert.True(p.IsInside(p1, p2));

        p = new Point(10.0, 0.0, 0.0); // На кінці сегмента
        Assert.True(p.IsInside(p1, p2));
    }

    [Fact]
    public void XParallel_ReturnsTrueIfParallelToXAxis()
    {
        var p = new Point(5.0, 0.0, 0.0);
        Assert.True(p.XParallel());

        p = new Point(5.0, 0.0, 0.0000000001); // В межах tolerance
        Assert.True(p.XParallel());

        p = new Point(5.0, 1.0, 0.0);
        Assert.False(p.XParallel());
    }

    [Fact]
    public void YParallel_ReturnsTrueIfParallelToYAxis()
    {
        var p = new Point(0.0, 5.0, 0.0);
        Assert.True(p.YParallel());

        p = new Point(0.0000000001, 5.0, 0.0); // В межах tolerance
        Assert.True(p.YParallel());

        p = new Point(0.0, 5.0, 1.0);
        Assert.False(p.YParallel());
    }

    [Fact]
    public void ZParallel_ReturnsTrueIfParallelToZAxis()
    {
        var p = new Point(0.0, 0.0, 5.0);
        Assert.True(p.ZParallel());

        p = new Point(0.0000000001, 0.0000000001, 5.0); // В межах tolerance
        // ZParallel використовує x == 0.0 && y == 0.0, що є строгим порівнянням
        // Можливо, варто використовувати IsZeroTol для x та y тут.
        // Згідно поточного коду, це буде false.
        Assert.False(p.ZParallel());

        p = new Point(0.0, 0.0, 0.0);
        Assert.True(p.ZParallel());
    }

    [Fact]
    public void ZProjectOntoEdge_ProjectsZCoordinateCorrectly()
    {
        var p = new Point(5.0, 5.0, 0.0); // Точка для проекції
        var p1 = new Point(0.0, 0.0, 10.0); // Початок лінії з Z=10
        var p2 = new Point(10.0, 10.0, 20.0); // Кінець лінії з Z=20

        p.ZProjectOntoEdge(p1, p2);
        // Точка (5,5) знаходиться рівно посередині між (0,0) і (10,10) в XY.
        // Отже, Z має бути посередині між 10 і 20, тобто 15.
        Assert.Equal(15.0, p.z, Tolerance);

        p = new Point(0.0, 0.0, 0.0);
        p1 = new Point(0.0, 0.0, 10.0);
        p2 = new Point(10.0, 0.0, 20.0);
        p.ZProjectOntoEdge(p1, p2);
        // Точка (0,0) знаходиться на початку лінії в XY.
        // Отже, Z має бути 10.
        Assert.Equal(10.0, p.z, Tolerance);
    }

    // Тести операторів
    [Fact]
    public void Operator_Plus_AddsPointsCorrectly()
    {
        var p1 = new Point(1.0, 2.0, 3.0);
        var p2 = new Point(4.0, 5.0, 6.0);
        var result = p1 + p2;
        Assert.Equal(5.0, result.x, Tolerance);
        Assert.Equal(7.0, result.y, Tolerance);
        Assert.Equal(9.0, result.z, Tolerance);
    }

    [Fact]
    public void Operator_Minus_SubtractsPointsCorrectly()
    {
        var p1 = new Point(5.0, 7.0, 9.0);
        var p2 = new Point(4.0, 5.0, 6.0);
        var result = p1 - p2;
        Assert.Equal(1.0, result.x, Tolerance);
        Assert.Equal(2.0, result.y, Tolerance);
        Assert.Equal(3.0, result.z, Tolerance);
    }

    [Fact]
    public void Operator_MultiplyScalar_MultipliesPointCorrectly()
    {
        var p = new Point(1.0, 2.0, 3.0);
        double scalar = 2.0;
        var result = p * scalar;
        Assert.Equal(2.0, result.x, Tolerance);
        Assert.Equal(4.0, result.y, Tolerance);
        Assert.Equal(6.0, result.z, Tolerance);
    }

    // Тести Equals та операторів порівняння
    [Fact]
    public void Equals_ReturnsTrueForEqualPoints()
    {
        var p1 = new Point(1.0, 2.0, 3.0);
        var p2 = new Point(1.0, 2.0, 3.0);
        Assert.True(p1.Equals(p2));
    }

    [Fact]
    public void Equals_ReturnsFalseForDifferentPoints()
    {
        var p1 = new Point(1.0, 2.0, 3.0);
        var p2 = new Point(1.1, 2.0, 3.0);
        Assert.False(p1.Equals(p2));
    }

    [Fact]
    public void Equals_ReturnsFalseForNullOrNonPointObject()
    {
        var p1 = new Point(1.0, 2.0, 3.0);
        Assert.False(p1.Equals(null));
        Assert.False(p1.Equals("not a point"));
    }

    [Fact]
    public void Operator_Equality_ReturnsTrueForEqualPoints()
    {
        var p1 = new Point(1.0, 2.0, 3.0);
        var p2 = new Point(1.0, 2.0, 3.0);
        Assert.True(p1 == p2);
    }

    [Fact]
    public void Operator_Equality_ReturnsFalseForDifferentPoints()
    {
        var p1 = new Point(1.0, 2.0, 3.0);
        var p2 = new Point(1.1, 2.0, 3.0);
        Assert.False(p1 == p2);
    }

    [Fact]
    public void Operator_Equality_HandlesNullsCorrectly()
    {
        Point p1 = null;
        Point p2 = null;
        Point p3 = new Point(1, 2, 3);

        Assert.True(p1 == p2);
        Assert.False(p1 == p3);
        Assert.False(p3 == p1);
    }

    [Fact]
    public void Operator_Inequality_ReturnsTrueForDifferentPoints()
    {
        var p1 = new Point(1.0, 2.0, 3.0);
        var p2 = new Point(1.1, 2.0, 3.0);
        Assert.True(p1 != p2);
    }

    [Fact]
    public void Operator_Inequality_ReturnsFalseForEqualPoints()
    {
        var p1 = new Point(1.0, 2.0, 3.0);
        var p2 = new Point(1.0, 2.0, 3.0);
        Assert.False(p1 != p2);
    }

    [Fact]
    public void GetHashCode_ReturnsConsistentHashCodeForEqualPoints()
    {
        var p1 = new Point(1.0, 2.0, 3.0);
        var p2 = new Point(1.0, 2.0, 3.0);
        Assert.Equal(p1.GetHashCode(), p2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ReturnsDifferentHashCodeForDifferentPoints()
    {
        var p1 = new Point(1.0, 2.0, 3.0);
        var p2 = new Point(1.1, 2.0, 3.0);
        // Це не гарантує, що хеш-коди будуть різними, але це хороший тест для звичайних випадків
        Assert.NotEqual(p1.GetHashCode(), p2.GetHashCode());
    }

    // Тести ToString та Str
    [Fact]
    public void ToString_ReturnsCorrectStringRepresentation()
    {
        var p = new Point(1.23, 4.56, 7.89);
        Assert.Equal("(1.23, 4.56, 7.89)", p.ToString());
    }

    [Fact]
    public void Str_ReturnsSameStringAsToString()
    {
        var p = new Point(1.23, 4.56, 7.89);
        Assert.Equal(p.ToString(), p.Str());
    }
}
