using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace procedural_dungeon_generator.Common {
    /// <summary>
    /// The point class.
    /// </summary>
    public class Point {

        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y) {
            X = x;
            Y = y;
        }

        public Point() : this(0, 0) { }

        /// <summary>
        /// Get distance of this point with that point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double Distance(Point point) {
            return Math.Sqrt(Math.Pow(point.X - X, 2) + Math.Pow(point.Y - Y, 2));
        }

        /// <summary>
        /// Get angle or curve between this point and that point.
        /// Reference: https://stackoverflow.com/questions/7586063/how-to-calculate-the-angle-between-a-line-and-the-horizontal-axis
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double GetAngle(Point point) {
            int deltaY = point.Y - Y;
            int deltaX = point.X - X;

            return Math.Atan2(deltaY, deltaX) * 180 / Math.PI;
        }

        public static Point operator +(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);
        public static Point operator -(Point a, Point b) => new Point(a.X - b.X, a.Y - b.Y);
        public static Point operator *(Point a, Point b) => new Point(a.X * b.X, a.Y * b.Y);
        public static Point operator /(Point a, Point b) => new Point(a.X / b.X, a.Y / b.Y);
        public static bool operator >(Point a, Point b) => a.X > b.X && a.Y > b.Y;
        public static bool operator >=(Point a, Point b) => a.X >= b.X && a.Y >= b.Y;
        public static bool operator <(Point a, Point b) => a.X < b.X && a.Y < b.Y;
        public static bool operator <=(Point a, Point b) => a.X <= b.X && a.Y <= b.Y;
        public override string ToString() => $"X: {X} / {Y}";

        public override int GetHashCode() {
            return X ^ Y;
        }

        public override bool Equals(object obj) {
            if (obj.GetType() != GetType()) return false;
            Point point = obj as Point;
            return X == point.X && Y == point.Y;
        }
    }
}
