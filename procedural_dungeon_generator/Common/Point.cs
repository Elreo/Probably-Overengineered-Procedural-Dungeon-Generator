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

        /// <summary>
        /// It returns the Euclidean Length of itself.
        /// </summary>
        /// <returns></returns>
        public int Length() {
            // return Math.Sqrt(X * X + Y * Y);
            return X * X + Y * Y;
        }

        /// <summary>
        /// Get the normalized variation of this point.
        /// </summary>
        /// <returns></returns>
        public Point Normalize() {
            int x = X;
            int y = Y;
            int length = Length();
            if (length != 0) {
                x /= length;
                y /= length;
            }
            return new Point(x, y);
        }

        /// <summary>
        /// Get the scaled by scalar variation of this point.
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public Point ScaleScalar(int scalar) {
            return new Point(X * scalar, Y * scalar);
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
