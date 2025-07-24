using System;
using System.Diagnostics;
using System.IO;
using Xunit;
using Ocl;

namespace CAMLib.Geo.Tests;

public class CCPointTests
{
    // Test for the default constructor CCPoint()
    [Fact]
    public void DefaultConstructor_InitializesToZeroZero()
    {
        // Arrange
        // No specific arrangement needed for default constructor,
        // as we are testing its default initialization.

        // Act
        CCPoint point = new CCPoint();

        // Assert
        Assert.Equal(0.0, point.x);
        Assert.Equal(0.0, point.y);
        Assert.Equal(0.0, point.z);
        Assert.Equal(CCType.NONE, point.type);
    }

    // Test for the constructor CCPoint(double X, double Y, double Z)
    [Theory]
    [InlineData(1.0, 2.0, 3.0)]
    [InlineData(-5.0, 0.0, 10.0)]
    [InlineData(0.0, 0.0, 0.0)]
    public void ConstructorXYZ_InitializesCorrectly(double x, double y, double z)
    {
        // Arrange
        // Input values are provided by InlineData.

        // Act
        CCPoint point = new CCPoint(x, y, z);

        // Assert
        Assert.Equal(x, point.x);
        Assert.Equal(y, point.y);
        Assert.Equal(z, point.z);
        Assert.Equal(CCType.NONE, point.type);
    }

    // Test for the constructor CCPoint(double X, double Y, double Z, CCType t)
    [Theory]
    [InlineData(1.0, 2.0, 3.0, CCType.VERTEX)]
    [InlineData(-5.0, 0.0, 10.0, CCType.EDGE)]
    [InlineData(0.0, 0.0, 0.0, CCType.FACET)]
    public void ConstructorXYZCCType_InitializesCorrectly(double x, double y, double z, CCType t)
    {
        // Arrange
        // Input values are provided by InlineData.

        // Act
        CCPoint point = new CCPoint(x, y, z, t);

        // Assert
        Assert.Equal(x, point.x);
        Assert.Equal(y, point.y);
        Assert.Equal(z, point.z);
        Assert.Equal(t, point.type);
    }

    // Test for the constructor CCPoint(Point p)
    [Fact]
    public void ConstructorPoint_InitializesCorrectly()
    {
        // Arrange
        Point p = new Point(1.0, 2.0, 3.0); // Example point            
        CCType expectedType = CCType.NONE;
        // Act
        CCPoint ccPoint = new CCPoint(p);
        // Assert
        Assert.Equal(p.x, ccPoint.x);
        Assert.Equal(p.y, ccPoint.y);
        Assert.Equal(p.z, ccPoint.z);
        Assert.Equal(expectedType, ccPoint.type);
    }

    // Test for the constructor CCPoint(Point p, CCType t)
    [Fact]
    public void ConstructorPointCCType_InitializesCorrectly()
    {
        // Arrange
        Point p = new Point(1.0, 2.0, 3.0); // Example point
        CCType expectedType = CCType.VERTEX; // Example type        
                                             // Act
        CCPoint ccPoint = new CCPoint(p, expectedType);
        // Assert
        Assert.Equal(p.x, ccPoint.x);
        Assert.Equal(p.y, ccPoint.y);
        Assert.Equal(p.z, ccPoint.z);
        Assert.Equal(expectedType, ccPoint.type);
    }

    // Test for the Assign method
    [Fact]
    public void Assign_UpdatesCoordinatesAndType()
    {
        // Arrange
        CCPoint ccPoint = new CCPoint(1.0, 2.0, 3.0, CCType.VERTEX);
        Point newPoint = new Point(4.0, 5.0, 6.0);

        // Act
        ccPoint.Assign(newPoint);

        // Assert
        Assert.Equal(newPoint.x, ccPoint.x);
        Assert.Equal(newPoint.y, ccPoint.y);
        Assert.Equal(newPoint.z, ccPoint.z);
        Assert.Equal(CCType.NONE, ccPoint.type); // Type should be reset to NONE
    }

    // Test for ToString method
    [Fact]
    public void ToString_ReturnsCorrectFormat()
    {
        // Arrange
        CCPoint ccPoint = new CCPoint(1.0, 2.0, 3.0, CCType.VERTEX);
        // Act
        string result = ccPoint.ToString();
        // Assert
        Assert.Equal("CC(1, 2, 3, t=VERTEX)", result);
    }


}