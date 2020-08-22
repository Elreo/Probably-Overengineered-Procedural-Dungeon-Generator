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

        public static double BoxMullerMD(double mean, double std) {
            double u1 = 1.0 - random.NextDouble();
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + std * randStdNormal;
        }

        //public static double NormalGaussianDistribution(double mean, double variance) {
        //    return Math.Sqrt(-2 * variance * Math.Log(random.NextDouble())) *
        //        Math.Cos(2 * Math.PI * random.NextDouble()) + mean;
        //}

        //public static UInt32 LcgRand(UInt32 state) {
        //    return state * 279470273u % 0xfffffffb;
        //}

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
    }
}
