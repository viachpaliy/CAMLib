using System;

namespace Ocl
{
    // Заглушка для Triangle, оскільки її визначення не наведене
   // public class Triangle { }

    /// <summary>
    /// Точка або вектор у 3D-просторі, визначений координатами (x, y, z)
    /// </summary>
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        // Створити точку в (0,0,0)
        public Point()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        // Створити точку в (x, y, z)
        public Point(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        // Створити точку в (x, y, 0)
        public Point(double x, double y)
        {
            X = x;
            Y = y;
            Z = 0;
        }

        // Конструктор копіювання
        public Point(Point p)
        {
            X = p.X;
            Y = p.Y;
            Z = p.Z;
        }

        // Деструктор не потрібен в C# (є GC)

        // Скалярний добуток
        public double Dot(Point p)
            => X * p.X + Y * p.Y + Z * p.Z;

        // Векторний добуток
        public Point Cross(Point p)
            => new Point(
                Y * p.Z - Z * p.Y,
                Z * p.X - X * p.Z,
                X * p.Y - Y * p.X
            );

        // Норма вектора (довжина)
        public double Norm()
            => Math.Sqrt(X * X + Y * Y + Z * Z);

        // Нормалізація
        public void Normalize()
        {
            double n = Norm();
            if (n > 0)
            {
                X /= n;
                Y /= n;
                Z /= n;
            }
        }

        // Відстань у XY площині
        public double XYDistance(Point p)
            => Math.Sqrt((X - p.X) * (X - p.X) + (Y - p.Y) * (Y - p.Y));

        // Довжина у XY площині
        public double XYNorm()
            => Math.Sqrt(X * X + Y * Y);

        // Нормалізація в XY площині
        public void XYNormalize()
        {
            double n = XYNorm();
            if (n > 0)
            {
                X /= n;
                Y /= n;
            }
        }

        // Перпендикуляр у XY площині (90 градусів вліво)
        public Point XYPerp()
            => new Point(-Y, X, Z);

        // Проекція точки по Z на відрізок p1-p2
        public void ZProjectOntoEdge(Point p1, Point p2)
        {
            // Переносимо X,Y на p1-p2, залишаємо Z від поточної точки
            Point closest = ClosestPoint(p1, p2);
            X = closest.X;
            Y = closest.Y;
            // Z не змінюємо
        }

        // Поворот у XY площині за cos і sin кута
        public void XYRotate(double cosa, double sina)
        {
            double xNew = X * cosa - Y * sina;
            double yNew = X * sina + Y * cosa;
            X = xNew;
            Y = yNew;
        }

        // Поворот у XY площині на кут (рад)
        public void XYRotate(double angle)
        {
            double cosa = Math.Cos(angle);
            double sina = Math.Sin(angle);
            XYRotate(cosa, sina);
        }

        // Поворот навколо X
        public void XRotate(double theta)
        {
            double cosT = Math.Cos(theta);
            double sinT = Math.Sin(theta);
            double yNew = Y * cosT - Z * sinT;
            double zNew = Y * sinT + Z * cosT;
            Y = yNew;
            Z = zNew;
        }

        // Поворот навколо Y
        public void YRotate(double theta)
        {
            double cosT = Math.Cos(theta);
            double sinT = Math.Sin(theta);
            double xNew = X * cosT + Z * sinT;
            double zNew = -X * sinT + Z * cosT;
            X = xNew;
            Z = zNew;
        }

        // Поворот навколо Z
        public void ZRotate(double theta)
        {
            double cosT = Math.Cos(theta);
            double sinT = Math.Sin(theta);
            double xNew = X * cosT - Y * sinT;
            double yNew = X * sinT + Y * cosT;
            X = xNew;
            Y = yNew;
        }

        // Матричний поворот
        public void MatrixRotate(double a, double b, double c,
                                 double d, double e, double f,
                                 double g, double h, double i)
        {
            double xNew = a * X + b * Y + c * Z;
            double yNew = d * X + e * Y + f * Z;
            double zNew = g * X + h * Y + i * Z;
            X = xNew;
            Y = yNew;
            Z = zNew;
        }

        // Відстань до прямої у XY площині
        public double XYDistanceToLine(Point p1, Point p2)
        {
            // Вектор p1->p2
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            double numerator = Math.Abs(dy * X - dx * Y + p2.X * p1.Y - p2.Y * p1.X);
            double denominator = Math.Sqrt(dx * dx + dy * dy);
            return denominator == 0 ? 0 : numerator / denominator;
        }

        // Найближча точка на прямій (3D)
        public Point ClosestPoint(Point p1, Point p2)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            double dz = p2.Z - p1.Z;
            double lengthSquared = dx * dx + dy * dy + dz * dz;
            if (lengthSquared == 0)
                return new Point(p1);
            double t = ((X - p1.X) * dx + (Y - p1.Y) * dy + (Z - p1.Z) * dz) / lengthSquared;
            return new Point(p1.X + t * dx, p1.Y + t * dy, p1.Z + t * dz);
        }

        // Найближча точка на прямій (XY)
        public Point XYClosestPoint(Point p1, Point p2)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            double lengthSquared = dx * dx + dy * dy;
            if (lengthSquared == 0)
                return new Point(p1);
            double t = ((X - p1.X) * dx + (Y - p1.Y) * dy) / lengthSquared;
            return new Point(p1.X + t * dx, p1.Y + t * dy, Z);
        }

        // Чи точка справа від прямої через p1,p2 (XY)
        public bool IsRight(Point p1, Point p2)
            => ((p2.X - p1.X) * (Y - p1.Y) - (p2.Y - p1.Y) * (X - p1.X)) < 0;

        // Чи точка всередині трикутника (заглушка, треба реалізувати)
        public bool IsInside(Triangle t)
        {
             Point p = this;
            Point a = t.p[0];
            Point b = t.p[1];
            Point c = t.p[2];

            // Compute barycentric coordinates (2D, XY-plane)
            double denominatorU = a.Y * c.X - a.X * c.Y + (c.Y - a.Y) * b.X + (a.X - c.X) * b.Y;
            double numeratorU = a.Y * c.X - a.X * c.Y + (c.Y - a.Y) * p.X + (a.X - c.X) * p.Y;
            double u = numeratorU / denominatorU;

            double denominatorV = a.X * b.Y - a.Y * b.X + (a.Y - b.Y) * c.X + (b.X - a.X) * c.Y;
            double numeratorV = a.X * b.Y - a.Y * b.X + (a.Y - b.Y) * p.X + (b.X - a.X) * p.Y;
            double v = numeratorV / denominatorV;

            // Check if point is inside triangle (in XY)
            return u > 0.0 && v > 0.0 && (u + v) < 1.0;
        }

        // Чи точка всередині відрізка p1-p2
        public bool IsInside(Point p1, Point p2)
        {
            // Для 3D
            return
                Math.Min(p1.X, p2.X) <= X && X <= Math.Max(p1.X, p2.X) &&
                Math.Min(p1.Y, p2.Y) <= Y && Y <= Math.Max(p1.Y, p2.Y) &&
                Math.Min(p1.Z, p2.Z) <= Z && Z <= Math.Max(p1.Z, p2.Z);
        }

        // Чи x і y компоненти обидва нулі
        public bool XParallel() => X == 0 && Y == 0;
        // Чи паралельно осі Y
        public bool YParallel() => X == 0 && Z == 0;
        // Чи паралельно осі Z
        public bool ZParallel() => Y == 0 && X == 0;

        // Оператор присвоєння (C# за замовчуванням копіює по посиланню для класів)
        public Point Assign(Point p)
        {
            X = p.X; Y = p.Y; Z = p.Z;
            return this;
        }

        // Додавання
        public static Point operator +(Point a, Point b)
            => new Point(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        // Віднімання
        public static Point operator -(Point a, Point b)
            => new Point(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        // Додавання до себе
        //public static Point operator +=(Point a, Point b)
        //{
        //    a.X += b.X; a.Y += b.Y; a.Z += b.Z;
        //    return a;
        //}

        // Віднімання від себе
        //public static Point operator -=(Point a, Point b)
        //{
        //    a.X -= b.X; a.Y -= b.Y; a.Z -= b.Z;
        //    return a;
        //}

        // Множення на скаляр (Point * scalar)
        public static Point operator *(Point p, double a)
            => new Point(p.X * a, p.Y * a, p.Z * a);

        // Множення на скаляр (scalar * Point)
        public static Point operator *(double a, Point p)
            => p * a;

        // Множення на скаляр до себе
        //public static Point operator *=(Point p, double a)
        //{
        //    p.X *= a; p.Y *= a; p.Z *= a;
        //    return p;
        //}

        // Рівність
        public override bool Equals(object obj)
        {
            if (obj is Point p)
                return X == p.X && Y == p.Y && Z == p.Z;
            return false;
        }

        public bool Equals(Point p)
            => X == p.X && Y == p.Y && Z == p.Z;

        // Нерівність
        public static bool operator ==(Point a, Point b)
            => a.Equals(b);

        public static bool operator !=(Point a, Point b)
            => !a.Equals(b);

        public override int GetHashCode()
            => HashCode.Combine(X, Y, Z);

        // ToString для репрезентації
        public override string ToString()
            => $"({X}, {Y}, {Z})";

        // Для сумісності з C++ str()
        public string Str() => ToString();
    }
}