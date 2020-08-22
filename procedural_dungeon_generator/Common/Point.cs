using System;
using System.Collections.Generic;
using System.Text;

namespace procedural_dungeon_generator.Common {
    /// <summary>
    /// The point class.
    /// </summary>
    public class Point {

        public int X { get; }
        public int Y { get; }

        public Point(int x, int y) {
            X = x;
            Y = y;
        }

        public Point() : this(0, 0) { }

        public static Point operator +(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);
        public static Point operator -(Point a, Point b) => new Point(a.X - b.X, a.Y - b.Y);
        public static Point operator *(Point a, Point b) => new Point(a.X * b.X, a.Y * b.Y);
        public static Point operator /(Point a, Point b) => new Point(a.X / b.X, a.Y / b.Y);
        public override string ToString() => $"X: {X} / {Y}";
    }
}
