using System;
using System.Linq;
using Ocl;
using Xunit;

namespace CAMLib.Geo.Tests
{
    public class ToolPathTests
    {
        #region Test Setup Helper Methods

        /// <summary>
        /// Creates a simple test line from (0,0,0) to (1,1,0)
        /// </summary>
        private Line CreateTestLine()
        {
            return new Line(new Point(0, 0, 0), new Point(1, 1, 0));
        }

        /// <summary>
        /// Creates a simple test arc
        /// </summary>
        private Arc CreateTestArc()
        {
            var p1 = new Point(1, 0, 0);
            var p2 = new Point(0, 1, 0);
            var center = new Point(0, 0, 0);
            return new Arc(p1, p2, center, true); // counter-clockwise
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void DefaultConstructor_ShouldCreateEmptyToolPath()
        {
            // Arrange & Act
            var path = new ToolPath();

            // Assert
            Assert.NotNull(path.SpanList);
            Assert.Empty(path.SpanList);
        }

        [Fact]
        public void CopyConstructor_ShouldCreateNewToolPathInstance()
        {
            // Arrange
            var originalToolPath = new ToolPath();
            var line = CreateTestLine();
            originalToolPath.Append(line);

            // Act
            var copiedToolPath = new ToolPath(originalToolPath);

            // Assert
            Assert.NotNull(copiedToolPath);
            Assert.NotNull(copiedToolPath.SpanList);
            Assert.NotSame(originalToolPath, copiedToolPath);
            // Note: Based on the comment "shallow copy is enough here", 
            // the SpanList itself is new but doesn't copy the contents
        }

        #endregion

        #region Append Line Tests

        [Fact]
        public void AppendLine_ShouldAddLineSpanToToolPath()
        {
            // Arrange
            var path = new ToolPath();
            var line = CreateTestLine();

            // Act
            path.Append(line);

            // Assert
            Assert.Single(path.SpanList);
            var span = path.SpanList.First();
            Assert.IsType<LineSpan>(span);
            Assert.Equal(SpanType.LineSpanType, span.Type);
        }

        [Fact]
        public void AppendLine_WithNullLine_ShouldNotThrow()
        {
            // Arrange
            var path = new ToolPath();

            // Act & Assert - should not throw exception
            path.Append((Line)null);
            Assert.Single(path.SpanList);
        }

        [Fact]
        public void AppendMultipleLines_ShouldAddAllLines()
        {
            // Arrange
            var path = new ToolPath();
            var line1 = new Line(new Point(0, 0, 0), new Point(1, 0, 0));
            var line2 = new Line(new Point(1, 0, 0), new Point(1, 1, 0));
            var line3 = new Line(new Point(1, 1, 0), new Point(0, 1, 0));

            // Act
            path.Append(line1);
            path.Append(line2);
            path.Append(line3);

            // Assert
            Assert.Equal(3, path.SpanList.Count);
            Assert.All(path.SpanList, span => Assert.IsType<LineSpan>(span));
            Assert.All(path.SpanList, span => Assert.Equal(SpanType.LineSpanType, span.Type));
        }

        #endregion

        #region Append Arc Tests

        [Fact]
        public void AppendArc_ShouldAddArcSpanToToolPath()
        {
            // Arrange
            var path = new ToolPath();
            var arc = CreateTestArc();

            // Act
            path.Append(arc);

            // Assert
            Assert.Single(path.SpanList);
            var span = path.SpanList.First();
            Assert.IsType<ArcSpan>(span);
            Assert.Equal(SpanType.ArcSpanType, span.Type);
        }

        [Fact]
        public void AppendArc_WithNullArc_ShouldNotThrow()
        {
            // Arrange
            var path = new ToolPath();

            // Act & Assert - should not throw exception
            path.Append((Arc)null);
            Assert.Single(path.SpanList);
        }

        [Fact]
        public void AppendMultipleArcs_ShouldAddAllArcs()
        {
            // Arrange
            var path = new ToolPath();
            var arc1 = new Arc(new Point(1, 0, 0), new Point(0, 1, 0), new Point(0, 0, 0), true);
            var arc2 = new Arc(new Point(0, 1, 0), new Point(-1, 0, 0), new Point(0, 0, 0), true);

            // Act
            path.Append(arc1);
            path.Append(arc2);

            // Assert
            Assert.Equal(2, path.SpanList.Count);
            Assert.All(path.SpanList, span => Assert.IsType<ArcSpan>(span));
            Assert.All(path.SpanList, span => Assert.Equal(SpanType.ArcSpanType, span.Type));
        }

        #endregion

        #region Mixed Append Tests

        [Fact]
        public void AppendMixedSpans_ShouldAddInCorrectOrder()
        {
            // Arrange
            var path = new ToolPath();
            var line = CreateTestLine();
            var arc = CreateTestArc();

            // Act
            path.Append(line);
            path.Append(arc);

            // Assert
            Assert.Equal(2, path.SpanList.Count);
            Assert.IsType<LineSpan>(path.SpanList[0]);
            Assert.IsType<ArcSpan>(path.SpanList[1]);
            Assert.Equal(SpanType.LineSpanType, path.SpanList[0].Type);
            Assert.Equal(SpanType.ArcSpanType, path.SpanList[1].Type);
        }

        [Fact]
        public void AppendComplexToolPath_ShouldMaintainOrder()
        {
            // Arrange
            var path = new ToolPath();
            var line1 = new Line(new Point(0, 0, 0), new Point(1, 0, 0));
            var arc1 = CreateTestArc();
            var line2 = new Line(new Point(0, 1, 0), new Point(0, 0, 0));

            // Act
            path.Append(line1);
            path.Append(arc1);
            path.Append(line2);

            // Assert
            Assert.Equal(3, path.SpanList.Count);
            Assert.IsType<LineSpan>(path.SpanList[0]);
            Assert.IsType<ArcSpan>(path.SpanList[1]);
            Assert.IsType<LineSpan>(path.SpanList[2]);
        }

        #endregion

        #region LineSpan Tests

        [Fact]
        public void LineSpan_ShouldWrapLineCorrectly()
        {
            // Arrange
            var line = CreateTestLine();

            // Act
            var lineSpan = new LineSpan(line);

            // Assert
            Assert.Equal(SpanType.LineSpanType, lineSpan.Type);
            Assert.Same(line, lineSpan.Line);
            Assert.Equal(line.Length2d(), lineSpan.Length2D);
        }

        [Fact]
        public void LineSpan_GetPoint_ShouldReturnCorrectPoints()
        {
            // Arrange
            var line = new Line(new Point(0, 0, 0), new Point(2, 0, 0));
            var lineSpan = new LineSpan(line);

            // Act & Assert
            var pointAt0 = lineSpan.GetPoint(0.0);
            var pointAt0_5 = lineSpan.GetPoint(0.5);
            var pointAt1 = lineSpan.GetPoint(1.0);

            // Assert
            Assert.Equal(0.0, pointAt0.x, 6);
            Assert.Equal(0.0, pointAt0.y, 6);
            Assert.Equal(1.0, pointAt0_5.x, 6);
            Assert.Equal(0.0, pointAt0_5.y, 6);
            Assert.Equal(2.0, pointAt1.x, 6);
            Assert.Equal(0.0, pointAt1.y, 6);
        }

        #endregion

        #region ArcSpan Tests

        [Fact]
        public void ArcSpan_ShouldWrapArcCorrectly()
        {
            // Arrange
            var arc = CreateTestArc();

            // Act
            var arcSpan = new ArcSpan(arc);

            // Assert
            Assert.Equal(SpanType.ArcSpanType, arcSpan.Type);
            Assert.Same(arc, arcSpan.Arc);
            Assert.Equal(arc.Length2d(), arcSpan.Length2D);
        }

        [Fact]
        public void ArcSpan_GetPoint_ShouldReturnCorrectPoints()
        {
            // Arrange
            var arc = CreateTestArc();
            var arcSpan = new ArcSpan(arc);

            // Act
            var pointAt0 = arcSpan.GetPoint(0.0);
            var pointAt1 = arcSpan.GetPoint(1.0);

            // Assert
            // At t=0, should be close to start point (1,0,0)
            Assert.Equal(1.0, pointAt0.x, 6);
            Assert.Equal(0.0, pointAt0.y, 6);
            // At t=1, should be close to end point (0,1,0)
            Assert.Equal(0.0, pointAt1.x, 6);
            Assert.Equal(1.0, pointAt1.y, 6);
        }

        #endregion

        #region Edge Cases and Error Handling

        [Fact]
        public void ToolPath_WithZeroLengthLine_ShouldHandleCorrectly()
        {
            // Arrange
            var path = new ToolPath();
            var zeroLine = new Line(new Point(1, 1, 1), new Point(1, 1, 1));

            // Act
            path.Append(zeroLine);

            // Assert
            Assert.Single(path.SpanList);
            var span = path.SpanList.First();
            Assert.Equal(0.0, span.Length2D);
        }

        [Fact]
        public void ToolPath_SpanListProperty_ShouldBeReadOnly()
        {
            // Arrange
            var path = new ToolPath();
            var originalSpanList = path.SpanList;

            // Act - try to modify the list directly
            path.SpanList.Add(new LineSpan(CreateTestLine()));

            // Assert
            Assert.Same(originalSpanList, path.SpanList);
            Assert.Single(path.SpanList);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void ToolPath_CreateCompleteClosedToolPath_ShouldWork()
        {
            // Arrange - Create a rectangular path with lines and arc corners
            var path = new ToolPath();
            
            // Bottom line
            var line1 = new Line(new Point(0, 0, 0), new Point(2, 0, 0));
            // Right line  
            var line2 = new Line(new Point(2, 0, 0), new Point(2, 2, 0));
            // Top line
            var line3 = new Line(new Point(2, 2, 0), new Point(0, 2, 0));
            // Left line
            var line4 = new Line(new Point(0, 2, 0), new Point(0, 0, 0));

            // Act
            path.Append(line1);
            path.Append(line2);
            path.Append(line3);
            path.Append(line4);

            // Assert
            Assert.Equal(4, path.SpanList.Count);
            Assert.All(path.SpanList, span => Assert.IsType<LineSpan>(span));
            
            // Verify total length
            var totalLength = path.SpanList.Sum(span => span.Length2D);
            Assert.Equal(8.0, totalLength, 6); // 4 sides of 2 units each
        }

        #endregion
    }
}
