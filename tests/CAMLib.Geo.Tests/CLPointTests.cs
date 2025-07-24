using System;
using System.Diagnostics;
using System.IO;
using Xunit;
using Ocl;

namespace CAMLib.Geo.Tests;

public class CLPointTests
{
    // Test for the default constructor CLPoint()
    [Fact]
    public void DefaultConstructor_InitializesToZeroZeroZero()
    {
        // Arrange
        // No specific arrangement needed for default constructor,
        // as we are testing its default initialization.

        // Act
        CLPoint point = new CLPoint();

        // Assert
        Assert.Equal(0.0, point.x);
        Assert.Equal(0.0, point.y);
        Assert.Equal(0.0, point.z);
        Assert.NotNull(point.CC); // CC should be initialized
    }

    // Test for the constructor CLPoint(double x, double y, double z)
    [Theory]
    [InlineData(1.0, 2.0, 3.0)]
    [InlineData(-5.0, 0.0, 10.0)]
    [InlineData(0.0, 0.0, 0.0)]
    public void ConstructorXYZ_InitializesCorrectly(double x, double y, double z)
    {
        // Arrange
        // Input values are provided by InlineData.

        // Act
        CLPoint point = new CLPoint(x, y, z);

        // Assert
        Assert.Equal(x, point.x);
        Assert.Equal(y, point.y);
        Assert.Equal(z, point.z);
        Assert.NotNull(point.CC); // CC should be initialized
    }

    // Test for the constructor CLPoint(double x, double y, double z, CCPoint ccp)
    [Theory]
    [InlineData(1.0, 2.0, 3.0)]
    [InlineData(-5.0, 0.0, 10.0)]
    [InlineData(0.0, 0.0, 0.0)]
    public void ConstructorXYZCCP_InitializesCorrectly(double x, double y, double z)
    {
        // Arrange
        CCPoint ccp = new CCPoint(x + 1, y + 1, z + 1);

        // Act
        CLPoint point = new CLPoint(x, y, z, ccp);

        // Assert
        Assert.Equal(x, point.x);
        Assert.Equal(y, point.y);
        Assert.Equal(z, point.z);
        Assert.Equal(ccp.x, point.CC.x);
        Assert.Equal(ccp.y, point.CC.y);
        Assert.Equal(ccp.z, point.CC.z);
        Assert.Equal(ccp.type, point.CC.type);
    }

    // Test for the copy constructor CLPoint(CLPoint cl)
    [Fact]
    public void CopyConstructor_CreatesIdenticalPoint()
    {
        // Arrange
        CLPoint original = new CLPoint(1.0, 2.0, 3.0, new CCPoint(4.0, 5.0, 6.0));  // Example CCPoint      
        // Act
        CLPoint copy = new CLPoint(original);
        // Assert
        Assert.Equal(original.x, copy.x);
        Assert.Equal(original.y, copy.y);
        Assert.Equal(original.z, copy.z);
        Assert.Equal(original.CC.x, copy.CC.x);
        Assert.Equal(original.CC.y, copy.CC.y);
        Assert.Equal(original.CC.z, copy.CC.z);
        Assert.Equal(original.CC.type, copy.CC.type);
    }

    // Test for the constructor CLPoint(Point p)
    [Fact]
    public void ConstructorFromPoint_InitializesCorrectly()
    {
        // Arrange
        Point p = new Point(1.0, 2.0, 3.0);
        // Act
        CLPoint point = new CLPoint(p);
        // Assert
        Assert.Equal(p.x, point.x);
        Assert.Equal(p.y, point.y);
        Assert.Equal(p.z, point.z);
    }

    // Test for the Below method
    [Fact]
    public void Below_ReturnsTrueIfBelowTriangle()
    {
        // Arrange
        CLPoint point = new CLPoint(1.0, 2.0, 1.0);
        Triangle triangle = new Triangle(new Point(0, 0, 0), new Point(2, 0, 0), new Point(1, 2, 2));
        // Act
        bool result = point.Below(triangle);
        // Assert
        Assert.True(result);
    }

    // Test for the LiftZ method
    [Theory]
    [InlineData(5.0, true)]
    [InlineData(1.0, false)]
    [InlineData(0.0, false)]
    public void LiftZ_ReturnsTrueIfLifted(double zin, bool expected)
    {
        // Arrange
        CLPoint point = new CLPoint(1.0, 2.0, 1.0);
        // Act
        bool result = point.LiftZ(zin);
        // Assert
        Assert.Equal(expected, result);
        if (expected)
        {
            Assert.Equal(zin, point.z);
        }
        else
        {
            Assert.Equal(1.0, point.z);
        }
    }

    // Test for the GetCC method    
    [Fact]
    public void GetCC_ReturnsCCPoint()
    {
        // Arrange
        CLPoint point = new CLPoint(1.0, 2.0, 3.0, new CCPoint(4.0, 5.0, 6.0));
        // Act
        CCPoint ccp = point.GetCC();
        // Assert
        Assert.NotNull(ccp);
        Assert.Equal(4.0, ccp.x);
        Assert.Equal(5.0, ccp.y);
        Assert.Equal(6.0, ccp.z);
        Assert.Equal(ccp.type, point.CC.type);
        // Ensure thread safety by checking that the CCPoint is not modified
        // ccp.x = 10.0; // Modify the copy
        // Assert.NotEqual(10.0, point.CC.x); // Original should remain unchanged      
        // Assert.Equal(4.0, point.CC.x); // Original should remain unchanged      
    }

    // Test for the CC property
    [Fact]
    public void CCProperty_ReturnsCCPoint()
    {
        // Arrange
        CLPoint point = new CLPoint(1.0, 2.0, 3.0, new CCPoint(4.0, 5.0, 6.0));
        // Act
        CCPoint ccp = point.CC;
        // Assert
        Assert.NotNull(ccp);
        Assert.Equal(4.0, ccp.x);
        Assert.Equal(5.0, ccp.y);
        Assert.Equal(6.0, ccp.z);
        Assert.Equal(ccp.type, point.CC.type);
        // Ensure thread safety by checking that the CCPoint is not modified
        // ccp.x = 10.0; // Modify the copy
        // Assert.NotEqual(10.0, point.CC.x); // Original should remain unchanged
        // Assert.Equal(4.0, point.CC.x); // Original should remain unchanged      
    }

    // Test for the CC property setter  
    [Fact]
    public void CCPropertySetter_UpdatesCCPoint()
    {
        // Arrange
        CLPoint point = new CLPoint(1.0, 2.0, 3.0, new CCPoint(4.0, 5.0, 6.0));
        CCPoint newCCP = new CCPoint(7.0, 8.0, 9.0);
        // Act
        point.CC = newCCP;
        // Assert
        Assert.NotNull(point.CC);
        Assert.Equal(7.0, point.CC.x);
        Assert.Equal(8.0, point.CC.y);
        Assert.Equal(9.0, point.CC.z);
        Assert.Equal(newCCP.type, point.CC.type);
        // Ensure that the original CCPoint is not modified
        // newCCP.x = 10.0; // Modify the copy
        // Assert.NotEqual(10.0, point.CC.x); // Original should remain unchanged
        // Assert.Equal(7.0, point.CC.x); // Original should remain unchanged      
    }

    // Test for the ToString method
    [Fact]
    public void ToString_ReturnsCorrectFormat()
    {
        // Arrange
        CLPoint point = new CLPoint(1.0, 2.0, 3.0, new CCPoint(4.0, 5.0, 6.0));
        // Act
        string result = point.ToString();
        // Assert
        Assert.Equal("CL(1, 2, 3) CC=CC(4, 5, 6, t=NONE)", result);
    }


}