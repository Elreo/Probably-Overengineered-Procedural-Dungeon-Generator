using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace procedural_dungeon_generator.Common {
    /// <summary>
    /// This class contains commonly used formula that is used in this library.
    /// </summary>
    public static class Utility {

        /// <summary>
        /// This methods crates a list of points that goes from point A to B.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="pointsAmount"></param>
        /// <returns></returns>
        public static List<Point> CreatePathPoints(Point a, Point b, int pointsAmount) {
            // https://stackoverflow.com/questions/21249739/how-to-calculate-the-points-between-two-given-points-and-given-distance
            pointsAmount += 1;
            double d = Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y)) / pointsAmount;
            double fi = Math.Atan2(b.Y - a.Y, b.X - a.X);

            //var outputPoints = new List<Point>(pointsAmount + 1);
            var outputPoints = new List<Point>();
            for (int i = 0; i <= pointsAmount; ++i) {
                outputPoints.Add(new Point((int) (a.X + i * d * Math.Cos(fi)), (int) (a.Y + i * d * Math.Sin(fi))));
            }
            return outputPoints;
        }
    }
}
