using System;
using System.Collections.Generic;
using System.Text;

namespace procedural_dungeon_generator.Common {
    class Point {

        public double X { get; }
        public double Y { get; }

        public Point(double x, double y) {
            X = x;
            Y = y;
        }

        public Point() : this(0, 0) { }

        public static Point operator +(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);
        public static Point operator -(Point a, Point b) => new Point(a.X - b.X, a.Y - b.Y);
        public static Point operator *(Point a, Point b) => new Point(a.X * b.X, a.Y * b.Y);
        public static Point operator /(Point a, Point b) => new Point(a.X / b.X, a.Y / b.Y); 
    }
}
