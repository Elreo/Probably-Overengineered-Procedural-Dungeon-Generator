using System;
using System.Collections.Generic;
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

        public double Distance(Point point) {
            return Math.Sqrt(Math.Pow(point.X - X, 2) + Math.Pow(point.Y - Y, 2));
        }

        public static Point operator +(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);
        public static Point operator -(Point a, Point b) => new Point(a.X - b.X, a.Y - b.Y);
        public static Point operator *(Point a, Point b) => new Point(a.X * b.X, a.Y * b.Y);
        public static Point operator /(Point a, Point b) => new Point(a.X / b.X, a.Y / b.Y);
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
