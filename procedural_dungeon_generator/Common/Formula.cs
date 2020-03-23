using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace procedural_dungeon_generator.Common {
    /// <summary>
    /// This class contains commonly used formula that is used in this library.
    /// </summary>
    public class Formula {
        private static Random random = new Random();

        public static double StandardDeviation(IEnumerable<double> values) {
            if (values.Any()) {
                double average = values.Average();
                double sum = values.Sum(d => Math.Pow(d - average, 2));
                return Math.Sqrt(sum / (values.Count() - 1));
            }
            throw new ArgumentException("'values' does not contain any value.");
        }

        public static double Mean(IEnumerable<double> values) {
            if (values.Any()) {
                return values.Average();
            }
            throw new ArgumentException("'values' does not contain any value.");
        }

        public static List<(double, double)> NormalDistribution(int value, int rand_max = 32767) {
            int m = value + value % 2;

            List<(double, double)> output = new List<(double, double)>();
            
            for (int a = 0; a < m; a += 2) {
                double x, y, rsq, f;
                do {
                    x = 2.0 * random.Next(rand_max) / rand_max - 1.0;
                    y = 2.0 * random.Next(rand_max) / rand_max - 1.0;
                    rsq = x * x + y * y;
                } while (rsq >= 1 || rsq == 0);
                f = Math.Sqrt(-2.0 * Math.Log(rsq) / rsq);
                output.Add((x * f, y * f));
            }

            return output;
        }
    }
}
