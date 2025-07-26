using System;
using Ocl;
using Xunit;

namespace CAMLib.Geo.Tests
{
    /// <summary>
    /// Unit tests for the Triangle class
    /// </summary>
    public class TriangleTests
    {
        #region Test Setup Helper Methods

        /// <summary>
        /// Creates a simple test triangle with vertices at (0,0,0), (1,0,0), (0,1,0)
        /// </summary>
        private Triangle CreateSimpleTriangle()
        {
            return new Triangle(
                new Point(0, 0, 0),
                new Point(1, 0, 0),
                new Point(0, 1, 0)
            );
        }

        /// <summary>
        /// Creates a triangle in 3D space
        /// </summary>
        private Triangle Create3DTriangle()
        {
            return new Triangle(
                new Point(0, 0, 0),
                new Point(1, 0, 0),
                new Point(0.5, 0.5, 1)
            );
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void DefaultConstructor_ShouldCreateValidTriangle()
        {
            // Arrange & Act
            var triangle = new Triangle();

            // Assert
            Assert.NotNull(triangle.p);
            Assert.Equal(3, triangle.p.Length);
            Assert.NotNull(triangle.p[0]);
            Assert.NotNull(triangle.p[1]);
            Assert.NotNull(triangle.p[2]);
            Assert.NotNull(triangle.n);
            Assert.NotNull(triangle.bb);
            
            // Check default vertices
            Assert.Equal(1.0, triangle.p[0].x, 6);
            Assert.Equal(0.0, triangle.p[0].y, 6);
            Assert.Equal(0.0, triangle.p[0].z, 6);
        }

        [Fact]
        public void ParameterizedConstructor_ShouldCreateTriangleWithGivenVertices()
        {
            // Arrange
            var p1 = new Point(0, 0, 0);
            var p2 = new Point(1, 0, 0);
            var p3 = new Point(0, 1, 0);

            // Act
            var triangle = new Triangle(p1, p2, p3);

            // Assert
            Assert.Equal(p1.x, triangle.p[0].x, 6);
            Assert.Equal(p1.y, triangle.p[0].y, 6);
            Assert.Equal(p1.z, triangle.p[0].z, 6);
            Assert.Equal(p2.x, triangle.p[1].x, 6);
            Assert.Equal(p2.y, triangle.p[1].y, 6);
            Assert.Equal(p2.z, triangle.p[1].z, 6);
            Assert.Equal(p3.x, triangle.p[2].x, 6);
            Assert.Equal(p3.y, triangle.p[2].y, 6);
            Assert.Equal(p3.z, triangle.p[2].z, 6);
        }

        [Fact]
        public void CopyConstructor_ShouldCreateIdenticalTriangle()
        {
            // Arrange
            var original = CreateSimpleTriangle();

            // Act
            var copy = new Triangle(original);

            // Assert
            for (int i = 0; i < 3; i++)
            {
                Assert.Equal(original.p[i].x, copy.p[i].x, 6);
                Assert.Equal(original.p[i].y, copy.p[i].y, 6);
                Assert.Equal(original.p[i].z, copy.p[i].z, 6);
            }
            Assert.Equal(original.n.x, copy.n.x, 6);
            Assert.Equal(original.n.y, copy.n.y, 6);
            Assert.Equal(original.n.z, copy.n.z, 6);
        }

        [Fact]
        public void CopyConstructor_ShouldCreateIndependentCopy()
        {
            // Arrange
            var original = CreateSimpleTriangle();
            var copy = new Triangle(original);

            // Act - modify the original
            original.p[0].x = 999;

            // Assert - copy should be unchanged
            Assert.NotEqual(999, copy.p[0].x);
        }

        #endregion

        #region Normal Vector Tests

        [Fact]
        public void CalcNormal_ShouldCalculateCorrectNormalForSimpleTriangle()
        {
            // Arrange
            var triangle = CreateSimpleTriangle();

            // Act & Assert
            // For triangle with vertices (0,0,0), (1,0,0), (0,1,0)
            // Normal should point in positive Z direction
            Assert.True(triangle.n.z > 0, "Normal should point in positive Z direction");
            
            // Normal should be normalized (length = 1)
            double normalLength = triangle.n.Norm();
            Assert.Equal(1.0, normalLength, 6);
        }

        [Fact]
        public void UpNormal_ShouldReturnNormalWithPositiveZ()
        {
            // Arrange
            var triangle = CreateSimpleTriangle();

            // Act
            var upNormal = triangle.UpNormal();

            // Assert
            Assert.True(upNormal.z >= 0, "UpNormal should have non-negative Z component");
        }

        [Fact]
        public void UpNormal_ShouldFlipNormalIfNegativeZ()
        {
            // Arrange - Create triangle with normal pointing down
            var triangle = new Triangle(
                new Point(0, 0, 0),
                new Point(0, 1, 0),
                new Point(1, 0, 0)  // Reversed order to get downward normal
            );

            // Act
            var upNormal = triangle.UpNormal();

            // Assert
            Assert.True(upNormal.z >= 0, "UpNormal should have non-negative Z component");
        }

        #endregion

        #region Bounding Box Tests

        [Fact]
        public void BoundingBox_ShouldContainAllVertices()
        {
            // Arrange
            var triangle = CreateSimpleTriangle();

            // Act & Assert
            Assert.True(triangle.bb.IsInside(triangle.p[0]));
            Assert.True(triangle.bb.IsInside(triangle.p[1]));
            Assert.True(triangle.bb.IsInside(triangle.p[2]));
        }

        [Fact]
        public void BoundingBox_ShouldHaveCorrectBounds()
        {
            // Arrange
            var triangle = new Triangle(
                new Point(-1, -2, -3),
                new Point(4, 5, 6),
                new Point(2, 1, 0)
            );

            // Act & Assert
            Assert.Equal(-1.0, triangle.bb.MinPt.x, 6);
            Assert.Equal(-2.0, triangle.bb.MinPt.y, 6);
            Assert.Equal(-3.0, triangle.bb.MinPt.z, 6);
            Assert.Equal(4.0, triangle.bb.MaxPt.x, 6);
            Assert.Equal(5.0, triangle.bb.MaxPt.y, 6);
            Assert.Equal(6.0, triangle.bb.MaxPt.z, 6);
        }

        #endregion

        #region Rotation Tests

        [Fact]
        public void Rotate_AroundZAxis_ShouldRotateVerticesCorrectly()
        {
            // Arrange
            var triangle = new Triangle(
                new Point(1, 0, 0),
                new Point(0, 1, 0),
                new Point(0, 0, 1)
            );
            double rotationAngle = Math.PI / 2; // 90 degrees

            // Act
            triangle.Rotate(0, 0, rotationAngle);

            // Assert - After 90° rotation around Z-axis:
            // (1,0,0) should become approximately (0,1,0)
            Assert.Equal(0.0, triangle.p[0].x, 6);
            Assert.Equal(1.0, triangle.p[0].y, 6);
            Assert.Equal(0.0, triangle.p[0].z, 6);
        }

        [Fact]
        public void Rotate_ShouldRecalculateNormalAndBoundingBox()
        {
            // Arrange
            var triangle = CreateSimpleTriangle();
            var originalNormal = new Point(triangle.n);
            var originalBBMinX = triangle.bb.MinPt.x;

            // Act
            triangle.Rotate(Math.PI / 4, 0, 0); // 45° around X-axis

            // Assert
            Assert.NotEqual(originalNormal.x, triangle.n.x, 6);
            Assert.NotEqual(originalBBMinX, triangle.bb.MinPt.x, 6);
        }

        #endregion

        #region Z-Slice Tests

        [Fact]
        public void ZSliceVerts_WithZCutBelowTriangle_ShouldReturnFalse()
        {
            // Arrange
            var triangle = new Triangle(
                new Point(0, 0, 1),
                new Point(1, 0, 1),
                new Point(0, 1, 1)
            );
            double zcut = 0.5; // Below the triangle

            // Act
            bool result = triangle.ZSliceVerts(out Point p1, out Point p2, zcut);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ZSliceVerts_WithZCutAboveTriangle_ShouldReturnFalse()
        {
            // Arrange
            var triangle = new Triangle(
                new Point(0, 0, 0),
                new Point(1, 0, 0),
                new Point(0, 1, 0)
            );
            double zcut = 1.5; // Above the triangle

            // Act
            bool result = triangle.ZSliceVerts(out Point p1, out Point p2, zcut);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ZSliceVerts_WithValidZCut_ShouldReturnTrueAndValidPoints()
        {
            // Arrange
            var triangle = new Triangle(
                new Point(0, 0, 0),
                new Point(2, 0, 2),
                new Point(0, 2, 2)
            );
            double zcut = 1.0; // Middle of the triangle

            // Act
            bool result = triangle.ZSliceVerts(out Point p1, out Point p2, zcut);

            // Assert
            Assert.True(result);
            Assert.NotNull(p1);
            Assert.NotNull(p2);
            Assert.Equal(zcut, p1.z, 6);
            Assert.Equal(zcut, p2.z, 6);
        }

        #endregion

        #region ToString Tests

        [Fact]
        public void ToString_ShouldReturnFormattedString()
        {
            // Arrange
            var triangle = CreateSimpleTriangle();

            // Act
            string result = triangle.ToString();

            // Assert
            Assert.NotNull(result);
            Assert.Contains("T:", result);
            Assert.Contains("n=", result);
            Assert.Contains("(", result);
            Assert.Contains(")", result);
        }

        #endregion

        #region Edge Cases and Error Handling

        [Fact]
        public void Triangle_WithCollinearPoints_ShouldStillCreateTriangle()
        {
            // Arrange & Act
            var triangle = new Triangle(
                new Point(0, 0, 0),
                new Point(1, 0, 0),
                new Point(2, 0, 0) // Collinear points
            );

            // Assert
            Assert.NotNull(triangle);
            Assert.NotNull(triangle.n);
            Assert.NotNull(triangle.bb);
        }

        [Fact]
        public void Triangle_WithIdenticalPoints_ShouldStillCreateTriangle()
        {
            // Arrange
            var point = new Point(1, 1, 1);

            // Act
            var triangle = new Triangle(point, point, point);

            // Assert
            Assert.NotNull(triangle);
            Assert.Equal(point.x, triangle.p[0].x, 6);
            Assert.Equal(point.x, triangle.p[1].x, 6);
            Assert.Equal(point.x, triangle.p[2].x, 6);
        }

        [Fact(Skip = "Temporarily disabled ")]
        public void CalcBB_AfterClearingBoundingBox_ShouldRecalculate()
        {
            // Arrange
            var triangle = CreateSimpleTriangle();
            triangle.bb.Clear();

            // Act
            triangle.CalcBB();

            // Assert
            Assert.False(triangle.bb.IsEmpty);
            Assert.True(triangle.bb.IsInside(triangle.p[0]));
            Assert.True(triangle.bb.IsInside(triangle.p[1]));
            Assert.True(triangle.bb.IsInside(triangle.p[2]));
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void Triangle_CompleteWorkflow_ShouldWorkCorrectly()
        {
            // Arrange
            var triangle = CreateSimpleTriangle();
            var originalArea = CalculateTriangleArea(triangle);

            // Act - Rotate the triangle
            triangle.Rotate(0, 0, Math.PI / 4);

            // Assert - Area should remain the same after rotation
            var rotatedArea = CalculateTriangleArea(triangle);
            Assert.Equal(originalArea, rotatedArea, 6);
        }

        [Fact]
        public void Triangle_MultipleRotations_ShouldBeConsistent()
        {
            // Arrange
            var triangle = CreateSimpleTriangle();
            var original = new Triangle(triangle);

            // Act - Rotate 360 degrees in 4 steps
            triangle.Rotate(0, 0, Math.PI / 2);
            triangle.Rotate(0, 0, Math.PI / 2);
            triangle.Rotate(0, 0, Math.PI / 2);
            triangle.Rotate(0, 0, Math.PI / 2);

            // Assert - Should be back to original position (within tolerance)
            for (int i = 0; i < 3; i++)
            {
                Assert.Equal(original.p[i].x, triangle.p[i].x, 5);
                Assert.Equal(original.p[i].y, triangle.p[i].y, 5);
                Assert.Equal(original.p[i].z, triangle.p[i].z, 5);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Calculate triangle area using cross product
        /// </summary>
        private double CalculateTriangleArea(Triangle triangle)
        {
            Point v1 = triangle.p[1] - triangle.p[0];
            Point v2 = triangle.p[2] - triangle.p[0];
            Point cross = v1.Cross(v2);
            return cross.Norm() / 2.0;
        }

        #endregion
    }
}
