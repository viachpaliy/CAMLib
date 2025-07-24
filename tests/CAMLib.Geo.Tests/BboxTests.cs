using Xunit;
using System;
using Ocl;

namespace CAMLib.Tests.Geo
{
    public class BboxTests
    {
        // Test case for default constructor
        [Fact]
        public void DefaultConstructor_CreatesEmptyBbox()
        {
            // Arrange & Act
            var bbox = new Bbox();

            // Assert
            Assert.True(bbox.IsEmpty);
            Assert.Equal(double.MaxValue, bbox.MinPt.x);
            Assert.Equal(double.MaxValue, bbox.MinPt.y);
            Assert.Equal(double.MaxValue, bbox.MinPt.z);
            Assert.Equal(double.MinValue, bbox.MaxPt.x);
            Assert.Equal(double.MinValue, bbox.MaxPt.y);
            Assert.Equal(double.MinValue, bbox.MaxPt.z);
        }

        // Test case for constructor with two points
        [Fact]
        public void TwoPointConstructor_CreatesCorrectBbox()
        {
            // Arrange
            var p1 = new Point(1.0, 2.0, 3.0);
            var p2 = new Point(5.0, 1.0, 4.0);

            // Act
            var bbox = new Bbox(p1, p2);

            // Assert
            Assert.False(bbox.IsEmpty);
            Assert.Equal(1.0, bbox.MinPt.x);
            Assert.Equal(1.0, bbox.MinPt.y);
            Assert.Equal(3.0, bbox.MinPt.z);
            Assert.Equal(5.0, bbox.MaxPt.x);
            Assert.Equal(2.0, bbox.MaxPt.y);
            Assert.Equal(4.0, bbox.MaxPt.z);
        }

        // Test case for constructor with min and max coordinates
        [Fact]
        public void MinMaxCoordinatesConstructor_CreatesCorrectBbox()
        {
            // Arrange
            double Minx = 0.0, Miny = 0.0, Minz = 0.0;
            double Maxx = 10.0, Maxy = 20.0, Maxz = 30.0;

            // Act
            var bbox = new Bbox(Minx, Miny, Minz, Maxx, Maxy, Maxz);

            // Assert
            Assert.False(bbox.IsEmpty);
            Assert.Equal(Minx, bbox.MinPt.x);
            Assert.Equal(Miny, bbox.MinPt.y);
            Assert.Equal(Minz, bbox.MinPt.z);
            Assert.Equal(Maxx, bbox.MaxPt.x);
            Assert.Equal(Maxy, bbox.MaxPt.y);
            Assert.Equal(Maxz, bbox.MaxPt.z);
        }

        // Test case for constructor with existing Bbox
        [Fact]
        public void ExistingBboxConstructor_CreatesCopy()
        {
            // Arrange
            var originalBbox = new Bbox(1.0, 2.0, 3.0, 4.0, 5.0, 6.0);

            // Act
            var copiedBbox = new Bbox(originalBbox);

            // Assert
            Assert.Equal(originalBbox.MinPt.x, copiedBbox.MinPt.x);
            Assert.Equal(originalBbox.MinPt.y, copiedBbox.MinPt.y);
            Assert.Equal(originalBbox.MinPt.z, copiedBbox.MinPt.z);
            Assert.Equal(originalBbox.MaxPt.x, copiedBbox.MaxPt.x);
            Assert.Equal(originalBbox.MaxPt.y, copiedBbox.MaxPt.y);
            Assert.Equal(originalBbox.MaxPt.z, copiedBbox.MaxPt.z);
            Assert.False(ReferenceEquals(originalBbox, copiedBbox)); // Ensure it's a deep copy, not same reference
        }

        // Test case for Add(Point) method
        [Fact]
        public void AddPoint_ExpandsBboxCorrectly()
        {
            // Arrange
            var bbox = new Bbox(0, 0, 0, 10, 10, 10);
            var point = new Point(15, 5, 2);

            // Act
            bbox.AddPoint(point);

            // Assert
            Assert.Equal(0, bbox.MinPt.x);
            Assert.Equal(0, bbox.MinPt.y);
            Assert.Equal(0, bbox.MinPt.z);
            Assert.Equal(15, bbox.MaxPt.x);
            Assert.Equal(10, bbox.MaxPt.y);
            Assert.Equal(10, bbox.MaxPt.z);
        }

        // Test case for Add(Bbox) method
        [Fact]
        public void AddBbox_ExpandsBboxCorrectly()
        {
            // Arrange
            var bbox1 = new Bbox(0, 0, 0, 10, 10, 10);
            var bbox2 = new Bbox(5, 5, 5, 15, 15, 15);

            // Act
            bbox1.AddBbox(bbox2);

            // Assert
            Assert.Equal(0, bbox1.MinPt.x);
            Assert.Equal(0, bbox1.MinPt.y);
            Assert.Equal(0, bbox1.MinPt.z);
            Assert.Equal(15, bbox1.MaxPt.x);
            Assert.Equal(15, bbox1.MaxPt.y);
            Assert.Equal(15, bbox1.MaxPt.z);
        }

        // Test case for Contains(Point) method
        [Theory]
        [InlineData(5.0, 5.0, 5.0, true)] // Inside
        [InlineData(0.0, 0.0, 0.0, true)] // On min boundary
        [InlineData(10.0, 10.0, 10.0, true)] // On max boundary
        [InlineData(-1.0, 5.0, 5.0, false)] // Outside MinPt.x
        [InlineData(11.0, 5.0, 5.0, false)] // Outside MaxPt.x
        public void ContainsPoint_ReturnsCorrectBoolean(double x, double y, double z, bool expected)
        {
            // Arrange
            var bbox = new Bbox(0, 0, 0, 10, 10, 10);
            var point = new Point(x, y, z);

            // Act
            bool result = bbox.IsInside(point);

            // Assert
            Assert.Equal(expected, result);
        }

        // Test case for Intersects(Bbox) method
        [Theory]
        [InlineData(0, 0, 0, 5, 5, 5, true)] // Intersects completely inside
        [InlineData(-5, -5, -5, 5, 5, 5, true)] // Intersects partially, overlapping min
        [InlineData(5, 5, 5, 15, 15, 15, true)] // Intersects partially, overlapping max
        [InlineData(-5, -5, -5, -1, -1, -1, false)] // No intersection (outside min)
        [InlineData(11, 11, 11, 15, 15, 15, false)] // No intersection (outside max)
        [InlineData(0, 0, 0, 10, 10, 10, true)] // Same bbox (intersects)
        public void IntersectsBbox_ReturnsCorrectBoolean(double Minx, double Miny, double Minz, double Maxx, double Maxy, double Maxz, bool expected)
        {
            // Arrange
            var bbox1 = new Bbox(0, 0, 0, 10, 10, 10);
            var bbox2 = new Bbox(Minx, Miny, Minz, Maxx, Maxy, Maxz);

            // Act
            bool result = bbox1.Intersects(bbox2);

            // Assert
            Assert.Equal(expected, result);
        }

        // Test case for Width property
        [Fact]
        public void Width_ReturnsCorrectValue()
        {
            // Arrange
            var bbox = new Bbox(1.0, 2.0, 3.0, 11.0, 5.0, 6.0);

            // Act & Assert
            Assert.Equal(10.0, bbox.Width);
        }

        // Test case for Height property
        [Fact]
        public void Height_ReturnsCorrectValue()
        {
            // Arrange
            var bbox = new Bbox(1.0, 2.0, 3.0, 4.0, 12.0, 6.0);

            // Act & Assert
            Assert.Equal(10.0, bbox.Height);
        }

        // Test case for Depth property
        [Fact]
        public void Depth_ReturnsCorrectValue()
        {
            // Arrange
            var bbox = new Bbox(1.0, 2.0, 3.0, 4.0, 5.0, 13.0);

            // Act & Assert
            Assert.Equal(10.0, bbox.Depth);
        }

        // Test case for IsEmpty property after adding a point
        [Fact]
        public void IsEmpty_BecomesFalse_AfterAddingPoint()
        {
            // Arrange
            var bbox = new Bbox(); // Initially empty

            // Act
            bbox.AddPoint(new Point(1, 2, 3));

            // Assert
            Assert.False(bbox.IsEmpty);
        }

        // Test case for IsEmpty property for a defined bbox
        [Fact]
        public void IsEmpty_IsFalse_ForDefinedBbox()
        {
            // Arrange
            var bbox = new Bbox(0, 0, 0, 1, 1, 1);

            // Act & Assert
            Assert.False(bbox.IsEmpty);
        }
        
        // Test case for IsEmpty property for a defined bbox with equal min/max values
        [Fact]
        public void IsEmpty_IsFalse_ForPointBbox()
        {
            // Arrange
            var bbox = new Bbox(0, 0, 0, 0, 0, 0);

            // Act & Assert
            Assert.False(bbox.IsEmpty);
        }

        // Test case for Center property
        [Fact]
        public void Center_ReturnsCorrectPoint()
        {
            // Arrange
            var bbox = new Bbox(0, 0, 0, 10, 20, 30);

            // Act
            var center = bbox.Center;

            // Assert
            Assert.Equal(5.0, center.x);
            Assert.Equal(10.0, center.y);
            Assert.Equal(15.0, center.z);
        }

        // Test case for Size property
        [Fact]
        public void Size_ReturnsCorrectVector()
        {
            // Arrange
            var bbox = new Bbox(0, 0, 0, 10, 20, 30);

            // Act
            var size = bbox.Size;

            // Assert
            Assert.Equal(10.0, size.x);
            Assert.Equal(20.0, size.y);
            Assert.Equal(30.0, size.z);
        }

        // // Test case for Transform method
        // [Fact]
        // public void Transform_TransformsBboxCorrectly()
        // {
        //     // Arrange
        //     var bbox = new Bbox(1, 1, 1, 2, 2, 2);
        //     // Simple translation matrix: move by (10, 20, 30)
        //     // A more complex Matrix4x4 would require a more complex test setup.
        //     var transform = Matrix4x4.CreateTranslation(10, 20, 30); 

        //     // Act
        //     bbox.Transform(transform);

        //     // Assert (Expected values after translation)
        //     Assert.Equal(11, bbox.MinPt.x);
        //     Assert.Equal(21, bbox.MinPt.y);
        //     Assert.Equal(31, bbox.MinPt.z);
        //     Assert.Equal(12, bbox.MaxPt.x);
        //     Assert.Equal(22, bbox.MaxPt.y);
        //     Assert.Equal(32, bbox.MaxPt.z);
        // }

        // Test case for Equals method with identical objects
        [Fact]
        public void Equals_WithIdenticalObjects_ReturnsTrue()
        {
            // Arrange
            var bbox1 = new Bbox(1, 2, 3, 4, 5, 6);
            var bbox2 = new Bbox(1, 2, 3, 4, 5, 6);

            // Act & Assert
            Assert.True(bbox1.Equals(bbox2));
        }

        // Test case for Equals method with different objects
        [Fact]
        public void Equals_WithDifferentObjects_ReturnsFalse()
        {
            // Arrange
            var bbox1 = new Bbox(1, 2, 3, 4, 5, 6);
            var bbox2 = new Bbox(10, 20, 30, 40, 50, 60);

            // Act & Assert
            Assert.False(bbox1.Equals(bbox2));
        }

        // Test case for Equals method with null
        [Fact]
        public void Equals_WithNull_ReturnsFalse()
        {
            // Arrange
            var bbox = new Bbox(1, 2, 3, 4, 5, 6);

            // Act & Assert
            Assert.False(bbox.Equals(null));
        }

        // Test case for Equals method with non-Bbox object
        [Fact]
        public void Equals_WithNonBboxObject_ReturnsFalse()
        {
            // Arrange
            var bbox = new Bbox(1, 2, 3, 4, 5, 6);
            var otherObject = new object();

            // Act & Assert
            Assert.False(bbox.Equals(otherObject));
        }

        // Test case for GetHashCode method (should be consistent for equal objects)
        [Fact]
        public void GetHashCode_ReturnsConsistentHashCode()
        {
            // Arrange
            var bbox1 = new Bbox(1, 2, 3, 4, 5, 6);
            var bbox2 = new Bbox(1, 2, 3, 4, 5, 6);

            // Act & Assert
            Assert.Equal(bbox1.GetHashCode(), bbox2.GetHashCode());
        }

        // Test case for ToString method (basic check)
        [Fact]
        public void ToString_ReturnsFormattedString()
        {
            // Arrange
            var bbox = new Bbox(0, 0, 0, 1, 1, 1);

            // Act
            string result = bbox.ToString();

            // Assert (Check if it contains expected values, format might vary slightly)
            Assert.Contains("MinPt.x=0", result);
            Assert.Contains("MaxPt.x=1", result);
            Assert.Contains("MinPt.y=0", result);
            Assert.Contains("MaxPt.y=1", result);
            Assert.Contains("MinPt.z=0", result);
            Assert.Contains("MaxPt.z=1", result);
        }
    }
}