using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Ocl
{
    /// <summary>
    /// Represents an STL (Stereolithography) surface, storing points and triangles.
    /// Handles reading from and writing to STL files, and provides methods for
    /// calculating bounding box and triangle normals.
    /// This class uses the 'Point' structure for its vertices and normals.
    /// </summary>
    public class STLSurf
    {
        // Private members corresponding to the C++ class.
        private List<Point> _points; // Changed from CCPoint to Point
        private List<int> _tris;     // Stores indices into the _points list: _tris[i], _tris[i+1], _tris[i+2] form a triangle
        private string _filename;
        private bool _normalGiven;
        private List<Point> _triNormals; // Normals for each triangle, changed from CCPoint to Point
        
        // Bounding box dimensions and center
        private double _sizeX, _sizeY, _sizeZ;
        private double _centerX, _centerY, _centerZ;
        private double _minX, _minY, _minZ;
        private double _maxX, _maxY, _maxZ;

        /// <summary>
        /// Initializes a new instance of the <see cref="STLSurf"/> class.
        /// Clears all internal data.
        /// </summary>
        public STLSurf()
        {
            Clear();
        }

        /// <summary>
        /// Clears all points, triangles, normals, and bounding box data.
        /// </summary>
        public void Clear()
        {
            _points = new List<Point>();
            _tris = new List<int>();
            _triNormals = new List<Point>();
            _filename = "";
            _normalGiven = false;

            _sizeX = _sizeY = _sizeZ = 0.0;
            _centerX = _centerY = _centerZ = 0.0;
            _minX = _minY = _minZ = 0.0;
            _maxX = _maxY = _maxZ = 0.0;
        }

        /// <summary>
        /// Reads an STL file from the specified path.
        /// Supports both ASCII and binary STL formats.
        /// </summary>
        /// <param name="filename">The path to the STL file.</param>
        /// <returns>True if the file was read successfully, false otherwise.</returns>
        public bool ReadStlFile(string filename)
        {
            Clear(); // Clear any existing data
            _filename = filename;

            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        // Read 80-character header
                        byte[] headerBytes = br.ReadBytes(80);
                        string header = Encoding.ASCII.GetString(headerBytes).Trim();

                        // Check if it's an ASCII STL by looking for "solid" keyword at the beginning
                        if (header.StartsWith("solid", StringComparison.OrdinalIgnoreCase))
                        {
                            fs.Seek(0, SeekOrigin.Begin); // Rewind to read as ASCII
                            return ReadAsciiStl(filename);
                        }
                        else
                        {
                            // Assume binary STL
                            // Read number of triangles (4 bytes, unsigned int)
                            uint numberOfTriangles = br.ReadUInt32();

                            _triNormals.Capacity = (int)numberOfTriangles;
                            _tris.Capacity = (int)numberOfTriangles * 3;
                            _points.Capacity = (int)numberOfTriangles * 3; // Initial capacity, might grow if vertices are unique

                            // Using a dictionary for efficient lookup and deduplication of points.
                            // Point is a struct, so it has value-based equality by default.
                            Dictionary<Point, int> pointMap = new Dictionary<Point, int>();
                            int pointIndex = 0;

                            for (int i = 0; i < numberOfTriangles; ++i)
                            {
                                // Read normal (3 floats)
                                float nx = br.ReadSingle();
                                float ny = br.ReadSingle();
                                float nz = br.ReadSingle();
                                _triNormals.Add(new Point(nx, ny, nz)); // Use Point constructor
                                _normalGiven = true; // Normals are present in binary STL

                                // Read 3 vertices (each 3 floats)
                                Point[] triPoints = new Point[3]; // Changed from CCPoint to Point
                                for (int j = 0; j < 3; ++j)
                                {
                                    float px = br.ReadSingle();
                                    float py = br.ReadSingle();
                                    float pz = br.ReadSingle();
                                    triPoints[j] = new Point(px, py, pz); // Use Point constructor
                                }

                                // Add points to list and store indices, deduplicating
                                foreach (var p in triPoints)
                                {
                                    if (!pointMap.TryGetValue(p, out int index))
                                    {
                                        index = pointIndex++;
                                        pointMap[p] = index;
                                        _points.Add(p);
                                    }
                                    _tris.Add(index);
                                }

                                // Read attribute byte count (2 bytes, unsigned short, usually 0)
                                ushort attributeByteCount = br.ReadUInt16();
                                // We don't use this, but need to consume it
                            }
                        }
                    }
                }

                CalculateBBox();
                if (!_normalGiven) // If normals were not in the file (e.g., ASCII), calculate them
                {
                    CalculateNormals();
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading STL file: {ex.Message}");
                Clear();
                return false;
            }
        }

        /// <summary>
        /// Reads an ASCII STL file.
        /// This is an internal helper method used by ReadStlFile.
        /// </summary>
        /// <param name="filename">The path to the STL file.</param>
        /// <returns>True if the file was read successfully, false otherwise.</returns>
        private bool ReadAsciiStl(string filename)
        {
            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    string line;
                    Dictionary<Point, int> pointMap = new Dictionary<Point, int>(); // Changed from CCPoint to Point
                    int pointIndex = 0;

                    // Skip header line (e.g., "solid OpenSCAD_Model")
                    sr.ReadLine();

                    while ((line = sr.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (line.StartsWith("endsolid", StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }

                        if (line.StartsWith("facet normal", StringComparison.OrdinalIgnoreCase))
                        {
                            string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            // Parse floats for normal coordinates
                            if (parts.Length >= 4 && float.TryParse(parts[2], out float nx) &&
                                                        float.TryParse(parts[3], out float ny) &&
                                                        float.TryParse(parts[4], out float nz))
                            {
                                _triNormals.Add(new Point(nx, ny, nz)); // Use Point constructor
                                _normalGiven = true;
                            }
                            else
                            {
                                // Handle error or skip if normal parsing fails
                                _triNormals.Add(new Point(0, 0, 0)); // Placeholder if normal is malformed
                            }

                            // Skip "outer loop"
                            sr.ReadLine();

                            Point[] triPoints = new Point[3]; // Changed from CCPoint to Point
                            for (int i = 0; i < 3; ++i)
                            {
                                line = sr.ReadLine()?.Trim();
                                if (line == null || !line.StartsWith("vertex", StringComparison.OrdinalIgnoreCase))
                                {
                                    throw new InvalidDataException("Malformed STL file: Expected 'vertex' line.");
                                }
                                parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                // Parse floats for vertex coordinates
                                if (parts.Length >= 4 && float.TryParse(parts[1], out float px) &&
                                                            float.TryParse(parts[2], out float py) &&
                                                            float.TryParse(parts[3], out float pz))
                                {
                                    triPoints[i] = new Point(px, py, pz); // Use Point constructor
                                }
                                else
                                {
                                    throw new InvalidDataException("Malformed STL file: Could not parse vertex coordinates.");
                                }
                            }

                            // Skip "endloop" and "endfacet"
                            sr.ReadLine();
                            sr.ReadLine();

                            // Add points to list and store indices, deduplicating
                            foreach (var p in triPoints)
                            {
                                if (!pointMap.TryGetValue(p, out int index))
                                {
                                    index = pointIndex++;
                                    pointMap[p] = index;
                                    _points.Add(p);
                                }
                                _tris.Add(index);
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading ASCII STL file: {ex.Message}");
                Clear();
                return false;
            }
        }


        /// <summary>
        /// Checks if the STL surface has been successfully loaded and contains data.
        /// </summary>
        /// <returns>True if the surface contains points and triangles, false otherwise.</returns>
        public bool IsOk()
        {
            return _points.Count > 0 && _tris.Count > 0 && (_tris.Count % 3 == 0);
        }

        /// <summary>
        /// Calculates the bounding box (min/max coordinates, size, and center) of the STL surface.
        /// This method should be called after loading points.
        /// </summary>
        public void CalculateBBox()
        {
            if (_points.Count == 0)
            {
                _minX = _minY = _minZ = _maxX = _maxY = _maxZ = 0.0;
                _sizeX = _sizeY = _sizeZ = 0.0;
                _centerX = _centerY = _centerZ = 0.0;
                return;
            }

            // Initialize min/max with the first point's coordinates
            _minX = _maxX = _points[0].X;
            _minY = _maxY = _points[0].Y;
            _minZ = _maxZ = _points[0].Z;

            // Iterate through the rest of the points to find true min/max
            for (int i = 1; i < _points.Count; ++i)
            {
                Point p = _points[i];
                if (p.X < _minX) _minX = p.X;
                if (p.X > _maxX) _maxX = p.X;
                if (p.Y < _minY) _minY = p.Y;
                if (p.Y > _maxY) _maxY = p.Y;
                if (p.Z < _minZ) _minZ = p.Z;
                if (p.Z > _maxZ) _maxZ = p.Z;
            }

            // Calculate size and center based on min/max
            _sizeX = _maxX - _minX;
            _sizeY = _maxY - _minY;
            _sizeZ = _maxZ - _minZ;

            _centerX = _minX + _sizeX / 2.0;
            _centerY = _minY + _sizeY / 2.0;
            _centerZ = _minZ + _sizeZ / 2.0;
        }

        /// <summary>
        /// Returns the total number of unique points (vertices) in the STL surface.
        /// </summary>
        /// <returns>The number of points.</returns>
        public int NumberOfPoints()
        {
            return _points.Count;
        }

        /// <summary>
        /// Returns the total number of triangles in the STL surface.
        /// </summary>
        /// <returns>The number of triangles.</returns>
        public int NumberOfTriangles()
        {
            return _tris.Count / 3;
        }

        /// <summary>
        /// Gets a point (vertex) by its index.
        /// </summary>
        /// <param name="i">The index of the point.</param>
        /// <returns>The <see cref="Point"/> at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is out of bounds.</exception>
        public Point GetPoint(int i) // Changed return type to Point
        {
            if (i < 0 || i >= _points.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(i), "Point index out of bounds.");
            }
            return _points[i];
        }

        /// <summary>
        /// Gets the triangle by its index.
        /// </summary>
        /// <param name="i">The index of the triangle (0-based).</param>
        public Triangle GetTriangle(int i)
        {
            int baseIndex = i * 3;
            if (baseIndex < 0 || baseIndex + 2 >= _tris.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(i), "Triangle index out of bounds.");
            }
            return new Triangle(
                _points[_tris[baseIndex]],
                _points[_tris[baseIndex + 1]],
                _points[_tris[baseIndex + 2]]);
        }

        /// <summary>
        /// Gets the three vertices of a triangle by its index.
        /// </summary>
        /// <param name="i">The index of the triangle (0-based).</param>
        /// <param name="p1">Output parameter for the first vertex.</param>
        /// <param name="p2">Output parameter for the second vertex.</param>
        /// <param name="p3">Output parameter for the third vertex.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the triangle index is out of bounds.</exception>
        public void GetTrianglePoints(int i, out Point p1, out Point p2, out Point p3) // Changed parameter types to Point
        {
            int baseIndex = i * 3;
            if (baseIndex < 0 || baseIndex + 2 >= _tris.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(i), "Triangle index out of bounds.");
            }

            p1 = _points[_tris[baseIndex]];
            p2 = _points[_tris[baseIndex + 1]];
            p3 = _points[_tris[baseIndex + 2]];
        }

        /// <summary>
        /// Gets the indices of the three vertices of a triangle by its index.
        /// These indices refer to the internal point list.
        /// </summary>
        /// <param name="i">The index of the triangle (0-based).</param>
        /// <param name="i1">Output parameter for the index of the first vertex.</param>
        /// <param name="i2">Output parameter for the index of the second vertex.</param>
        /// <param name="i3">Output parameter for the index of the third vertex.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the triangle index is out of bounds.</exception>
        public void GetTriangleInd(int i, out int i1, out int i2, out int i3)
        {
            int baseIndex = i * 3;
            if (baseIndex < 0 || baseIndex + 2 >= _tris.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(i), "Triangle index out of bounds.");
            }

            i1 = _tris[baseIndex];
            i2 = _tris[baseIndex + 1];
            i3 = _tris[baseIndex + 2];
        }

        /// <summary>
        /// Gets the normal vector for a specific triangle.
        /// </summary>
        /// <param name="i">The index of the triangle (0-based).</param>
        /// <returns>The <see cref="Point"/> representing the normal vector.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the triangle index is out of bounds.</exception>
        public Point GetNormal(int i) // Changed return type to Point
        {
            if (i < 0 || i >= _triNormals.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(i), "Normal index out of bounds.");
            }
            return _triNormals[i];
        }

        /// <summary>
        /// Indicates whether the STL file provided explicit normal vectors for triangles.
        /// </summary>
        /// <returns>True if normals were given in the file, false otherwise.</returns>
        public bool HasNormal()
        {
            return _normalGiven;
        }

        /// <summary>
        /// Calculates and stores the normal vector for each triangle in the surface.
        /// If normals were already given in the file, this method will overwrite them.
        /// </summary>
        public void CalculateNormals()
        {
            _triNormals.Clear();
            _triNormals.Capacity = NumberOfTriangles();

            for (int i = 0; i < NumberOfTriangles(); ++i)
            {
                var t = GetTriangle(i); // Changed parameter types to Point

                // Calculate two edges of the triangle
                Point edge1 = t.p[1] - t.p[0]; // Using Point's operator-
                Point edge2 = t.p[2] - t.p[0]; // Using Point's operator-

                // Calculate the cross product to get the normal
                Point normal = edge1.Cross(edge2); // Using Point.CrossProduct static method

                // Normalize the normal vector
                normal.Normalize(); // Using Point.Normalize static method
                _triNormals.Add(normal); // Using Point.Normalize static method
            }
            _normalGiven = true;
        }

        /// <summary>
        /// Gets the minimum X-coordinate of the bounding box.
        /// </summary>
        public double GetMinX() => _minX;

        /// <summary>
        /// Gets the minimum Y-coordinate of the bounding box.
        /// </summary>
        public double GetMinY() => _minY;

        /// <summary>
        /// Gets the minimum Z-coordinate of the bounding box.
        /// </summary>
        public double GetMinZ() => _minZ;

        /// <summary>
        /// Gets the maximum X-coordinate of the bounding box.
        /// </summary>
        public double GetMaxX() => _maxX;

        /// <summary>
        /// Gets the maximum Y-coordinate of the bounding box.
        /// </summary>
        public double GetMaxY() => _maxY;

        /// <summary>
        /// Gets the maximum Z-coordinate of the bounding box.
        /// </summary>
        public double GetMaxZ() => _maxZ;

        /// <summary>
        /// Gets the size of the bounding box along the X-axis.
        /// </summary>
        public double GetSizeX() => _sizeX;

        /// <summary>
        /// Gets the size of the bounding box along the Y-axis.
        /// </summary>
        public double GetSizeY() => _sizeY;

        /// <summary>
        /// Gets the size of the bounding box along the Z-axis.
        /// </summary>
        public double GetSizeZ() => _sizeZ;

        /// <summary>
        /// Gets the X-coordinate of the center of the bounding box.
        /// </summary>
        public double GetCenterX() => _centerX;

        /// <summary>
        /// Gets the Y-coordinate of the center of the bounding box.
        /// </summary>
        public double GetCenterY() => _centerY;

        /// <summary>
        /// Gets the Z-coordinate of the center of the bounding box.
        /// </summary>
        public double GetCenterZ() => _centerZ;

        /// <summary>
        /// Writes the STL surface data to a binary STL file.
        /// </summary>
        /// <param name="filename">The path to the output STL file.</param>
        /// <returns>True if the file was written successfully, false otherwise.</returns>
        public bool WriteToFile(string filename)
        {
            if (!IsOk())
            {
                Debug.WriteLine("Error: No STL data to write.");
                return false;
            }

            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        // Write 80-character header (can be anything, often empty or a model name)
                        string header = "Binary STL created by CAMLib.Geo.STLSurf";
                        byte[] headerBytes = Encoding.ASCII.GetBytes(header.PadRight(80, '\0').Substring(0, 80));
                        bw.Write(headerBytes);

                        // Write number of triangles (uint)
                        bw.Write((uint)NumberOfTriangles());

                        for (int i = 0; i < NumberOfTriangles(); ++i)
                        {
                            // Get normal (3 floats)
                            Point normal = _triNormals[i]; // Changed from CCPoint to Point
                            bw.Write((float)normal.X);
                            bw.Write((float)normal.Y);
                            bw.Write((float)normal.Z);

                            // Get 3 vertices (each 3 floats)
                            GetTrianglePoints(i, out Point p1, out Point p2, out Point p3); // Changed to Point
                            bw.Write((float)p1.X); bw.Write((float)p1.Y); bw.Write((float)p1.Z);
                            bw.Write((float)p2.X); bw.Write((float)p2.Y); bw.Write((float)p2.Z);
                            bw.Write((float)p3.X); bw.Write((float)p3.Y); bw.Write((float)p3.Z);

                            // Write attribute byte count (ushort, usually 0)
                            bw.Write((ushort)0);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error writing binary STL file: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Writes the STL surface data to an ASCII STL file.
        /// </summary>
        /// <param name="filename">The path to the output STL file.</param>
        /// <returns>True if the file was written successfully, false otherwise.</returns>
        public bool WriteAsciiToFile(string filename)
        {
            if (!IsOk())
            {
                Debug.WriteLine("Error: No STL data to write.");
                return false;
            }

            try
            {
                using (StreamWriter sw = new StreamWriter(filename))
                {
                    sw.WriteLine("solid CAMLibModel"); // Header

                    for (int i = 0; i < NumberOfTriangles(); ++i)
                    {
                        Point normal = _triNormals[i]; // Changed from CCPoint to Point
                        var t = GetTriangle(i); // Changed to Point

                        // Using :F6 for fixed-point format with 6 decimal places, common for STL
                        sw.WriteLine($"  facet normal {normal.X:F6} {normal.Y:F6} {normal.Z:F6}");
                        sw.WriteLine("    outer loop");
                        sw.WriteLine($"      vertex {t.p[0].X:F6} {t.p[0].Y:F6} {t.p[0].Z:F6}");
                        sw.WriteLine($"      vertex {t.p[1].X:F6} {t.p[1].Y:F6} {t.p[1].Z:F6}");
                        sw.WriteLine($"      vertex {t.p[2].X:F6} {t.p[2].Y:F6} {t.p[2].Z:F6}");
                        sw.WriteLine("    endloop");
                        sw.WriteLine("  endfacet");
                    }
                    sw.WriteLine("endsolid CAMLibModel"); // Footer
                }
                return true;
            }
            catch (Exception ex)
            {
            Debug.WriteLine($"Error writing ASCII STL file: {ex.Message}");
                return false;
            }
        }
    }
}