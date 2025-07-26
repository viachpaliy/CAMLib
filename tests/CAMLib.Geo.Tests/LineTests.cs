using System;
using System.Diagnostics;
using System.IO;
 using Ocl;
 using Xunit;
 
 namespace CAMLib.Geo.Tests
 {
     /// <summary
     /// Unit tests for the Line class
     /// </summary
     public class LineTests
     {
         #region Constructor Tests
 
         [Fact]
         public void DefaultConstructor_ShouldCreateLineWithZeroPoints()
         {
             // Arrange & Act
             var line = new Line();
 
             // Assert
             Assert.NotNull(line.p1);
             Assert.NotNull(line.p2);
             Assert.Equal(0.0, line.p1.x);
             Assert.Equal(0.0, line.p1.y);
             Assert.Equal(0.0, line.p1.z);
             Assert.Equal(0.0, line.p2.x);
             Assert.Equal(0.0, line.p2.y);
             Assert.Equal(0.0, line.p2.z);
         }
 
         [Fact]
         public void ParameterizedConstructor_ShouldCreateLineWithSpecifiedPoints()
         {
             // Arrange
             var p1 = new Point(1.0, 2.0, 3.0);
             var p2 = new Point(4.0, 5.0, 6.0);
 
             // Act
             var line = new Line(p1, p2);
 
             // Assert
             Assert.NotSame(p1, line.p1); // Should be copies, not references
             Assert.NotSame(p2, line.p2);
             Assert.Equal(p1.x, line.p1.x);
             Assert.Equal(p1.y, line.p1.y);
             Assert.Equal(p1.z, line.p1.z);
             Assert.Equal(p2.x, line.p2.x);
             Assert.Equal(p2.y, line.p2.y);
             Assert.Equal(p2.z, line.p2.z);
         }
 
         [Fact]
         public void CopyConstructor_ShouldCreateCopyOfOriginalLine()
         {
             // Arrange
             var originalLine = new Line(new Point(1.0, 2.0, 3.0), new Point(4.0, 5.0, 6.0));
 
             // Act
             var copiedLine = new Line(originalLine);
 
             // Assert
             Assert.NotSame(originalLine.p1, copiedLine.p1); // Should be copies, not references
             Assert.NotSame(originalLine.p2, copiedLine.p2);
             Assert.Equal(originalLine.p1.x, copiedLine.p1.x);
             Assert.Equal(originalLine.p1.y, copiedLine.p1.y);
             Assert.Equal(originalLine.p1.z, copiedLine.p1.z);
             Assert.Equal(originalLine.p2.x, copiedLine.p2.x);
             Assert.Equal(originalLine.p2.y, copiedLine.p2.y);
             Assert.Equal(originalLine.p2.z, copiedLine.p2.z);
         }
 
         #endregion
 
         #region Length2d Tests
 
         [Fact]
         public void Length2d_ShouldReturnZeroForZeroLengthLine()
         {
             // Arrange
             var line = new Line(new Point(0, 0, 0), new Point(0, 0, 5));
 
             // Act
             var length = line.Length2d();
 
             // Assert
             Assert.Equal(0.0, length);
         }
 
         [Fact]
         public void Length2d_ShouldReturnCorrectLengthForHorizontalLine()
         {
             // Arrange
             var line = new Line(new Point(0, 0, 0), new Point(3, 0, 0));
 
             // Act
             var length = line.Length2d();
 
             // Assert
             Assert.Equal(3.0, length);
         }
 
         [Fact]
         public void Length2d_ShouldReturnCorrectLengthForVerticalLine()
         {
             // Arrange
             var line = new Line(new Point(0, 0, 0), new Point(0, 4, 0));
 
             // Act
             var length = line.Length2d();
 
             // Assert
             Assert.Equal(4.0, length);
         }
 
         [Fact]
         public void Length2d_ShouldReturnCorrectLengthForDiagonalLine()
         {
             // Arrange
             var line = new Line(new Point(0, 0, 0), new Point(3, 4, 0));
 
             // Act
             var length = line.Length2d();
 
             // Assert
             Assert.Equal(5.0, length, 6); // 3-4-5 triangle
         }
 
         [Fact]
         public void Length2d_ShouldIgnoreZComponent()
         {
             // Arrange
             var line1 = new Line(new Point(0, 0, 0), new Point(3, 4, 0));
             var line2 = new Line(new Point(0, 0, 10), new Point(3, 4, 20));
 
             // Act
             var length1 = line1.Length2d();
             var length2 = line2.Length2d();
 
             // Assert
             Assert.Equal(length1, length2);
         }
 
         #endregion
 
         #region GetPoint Tests
 
         [Fact]
         public void GetPoint_WithParameterZero_ShouldReturnStartPoint()
         {
             // Arrange
             var p1 = new Point(1, 2, 3);
             var p2 = new Point(4, 5, 6);
             var line = new Line(p1, p2);
 
             // Act
             var point = line.GetPoint(0.0);
 
             // Assert
             Assert.Equal(p1.x, point.x);
             Assert.Equal(p1.y, point.y);
             Assert.Equal(p1.z, point.z);
         }
 
         [Fact]
         public void GetPoint_WithParameterOne_ShouldReturnEndPoint()
         {
             // Arrange
             var p1 = new Point(1, 2, 3);
             var p2 = new Point(4, 5, 6);
             var line = new Line(p1, p2);
 
             // Act
             var point = line.GetPoint(1.0);
 
             // Assert
             Assert.Equal(p2.x, point.x);
             Assert.Equal(p2.y, point.y);
             Assert.Equal(p2.z, point.z);
         }
 
         [Fact]
         public void GetPoint_WithParameterHalf_ShouldReturnMidpoint()
         {
             // Arrange
             var p1 = new Point(0, 0, 0);
             var p2 = new Point(2, 4, 6);
             var line = new Line(p1, p2);
 
             // Act
             var point = line.GetPoint(0.5);
 
             // Assert
             Assert.Equal(1.0, point.x);
             Assert.Equal(2.0, point.y);
             Assert.Equal(3.0, point.z);
         }
 
         [Fact]
         public void GetPoint_WithParameterBeyondRange_ShouldExtrapolate()
         {
             // Arrange
             var p1 = new Point(0, 0, 0);
             var p2 = new Point(1, 1, 1);
             var line = new Line(p1, p2);
 
             // Act
             var pointBefore = line.GetPoint(-1.0);
             var pointAfter = line.GetPoint(2.0);
 
             // Assert
             Assert.Equal(-1.0, pointBefore.x);
             Assert.Equal(-1.0, pointBefore.y);
             Assert.Equal(-1.0, pointBefore.z);
             Assert.Equal(2.0, pointAfter.x);
             Assert.Equal(2.0, pointAfter.y);
             Assert.Equal(2.0, pointAfter.z);
         }
 
         #endregion
 
         #region Near Tests
 
         [Fact]
         public void Near_WithPointOnLine_ShouldReturnSamePoint()
         {
             // Arrange
             var line = new Line(new Point(0, 0, 0), new Point(2, 0, 0));
             var pointOnLine = new Point(1, 0, 0);
 
             // Act
             var nearestPoint = line.Near(pointOnLine);
 
             // Assert
             Assert.Equal(pointOnLine.x, nearestPoint.x, 6);
             Assert.Equal(pointOnLine.y, nearestPoint.y, 6);
             Assert.Equal(pointOnLine.z, nearestPoint.z, 6);
         }
 
         [Fact]
         public void Near_WithPointAboveLine_ShouldReturnProjection()
         {
             // Arrange
             var line = new Line(new Point(0, 0, 0), new Point(2, 0, 0));
             var pointAbove = new Point(1, 1, 0);
 
             // Act
             var nearestPoint = line.Near(pointAbove);
 
             // Assert
             Assert.Equal(1.0, nearestPoint.x, 6);
             Assert.Equal(0.0, nearestPoint.y, 6);
             Assert.Equal(0.0, nearestPoint.z, 6);
         }
 
         [Fact]
         public void Near_WithPointBeyondLineEnd_ShouldExtrapolateOnExtendedLine()
         {
             // Arrange
             var line = new Line(new Point(0, 0, 0), new Point(1, 0, 0));
             var pointBeyond = new Point(2, 1, 0);
 
             // Act
             var nearestPoint = line.Near(pointBeyond);
 
             // Assert
             Assert.Equal(2.0, nearestPoint.x, 6);
             Assert.Equal(0.0, nearestPoint.y, 6);
             Assert.Equal(0.0, nearestPoint.z, 6);
         }
 
         [Fact]
         public void Near_WithDiagonalLine_ShouldReturnCorrectProjection()
         {
             // Arrange
             var line = new Line(new Point(0, 0, 0), new Point(3, 4, 0));
             var point = new Point(2, 1, 0);
 
             // Act
             var nearestPoint = line.Near(point);
 
             // Assert
             // The projection of (2,1,0) onto the line from (0,0,0) to (3,4,0)
             // Direction vector: (3,4,0), normalized: (0.6, 0.8, 0)
             // Dot product: (2*0.6 + 1*0.8) = 2
             // Projection: (0,0,0) + 2*(0.6,0.8,0) = (1.2, 1.6, 0)
             Assert.Equal(1.2, nearestPoint.x, 6);
             Assert.Equal(1.6, nearestPoint.y, 6);
             Assert.Equal(0.0, nearestPoint.z, 6);
         }
 
         #endregion
 
         #region ToString Tests
 
         [Fact]
         public void ToString_ShouldReturnFormattedString()
         {
             // Arrange
             var line = new Line(new Point(1, 2, 3), new Point(4, 5, 6));
 
             // Act
             var result = line.ToString();
 
             // Assert
             Assert.Equal("((1, 2, 3), (4, 5, 6))", result);
         }
 
         [Fact]
         public void ToString_WithZeroPoints_ShouldReturnFormattedString()
         {
             // Arrange
             var line = new Line();
 
             // Act
             var result = line.ToString();
 
             // Assert
             Assert.Equal("((0, 0, 0), (0, 0, 0))", result);
         }
 
         #endregion
 
         #region Edge Cases and Special Scenarios
 
         [Fact]
         public void Line_WithIdenticalPoints_ShouldHandleGracefully()
         {
             // Arrange
             var point = new Point(1, 2, 3);
             var line = new Line(point, point);
 
             // Act & Assert
             Assert.Equal(0.0, line.Length2d());
             
             var midpoint = line.GetPoint(0.5);
             Assert.Equal(point.x, midpoint.x);
             Assert.Equal(point.y, midpoint.y);
             Assert.Equal(point.z, midpoint.z);
         }
 
         [Fact]
         public void Line_WithNegativeCoordinates_ShouldWorkCorrectly()
         {
             // Arrange
             var line = new Line(new Point(-2, -3, -1), new Point(1, 2, 3));
 
             // Act
             var length = line.Length2d();
             var midpoint = line.GetPoint(0.5);
 
             // Assert
             Assert.Equal(Math.Sqrt(34), length, 6); // sqrt((1-(-2))^2 + (2-(-3))^2) = sqrt(9+25) = sqrt(34)
             Assert.Equal(-0.5, midpoint.x);
             Assert.Equal(-0.5, midpoint.y);
             Assert.Equal(1.0, midpoint.z);
         }
 
         [Fact]
         public void Near_WithZeroLengthLine_ShouldHandleGracefully()
         {
             // Arrange
             var point = new Point(1, 1, 1);
             var line = new Line(point, point);
             var testPoint = new Point(2, 2, 2);
 
             // Act
             var nearestPoint = line.Near(testPoint);
 
             // Assert - should return a point on the line (which is just the single point)
             // Note: With zero-length line, the normalized direction vector will be (0,0,0)
             // so the result should be the line point itself
             Assert.Equal(point.x, nearestPoint.x, 6);
             Assert.Equal(point.y, nearestPoint.y, 6);
             Assert.Equal(point.z, nearestPoint.z, 6);
         }
 
         #endregion
     }
 }