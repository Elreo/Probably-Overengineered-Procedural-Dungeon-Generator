using System;
using System.Collections.Generic;
using System.Text;

using procedural_dungeon_generator.Common;
using procedural_dungeon_generator.Components;

namespace procedural_dungeon_generator.Generators {
    /// <summary>
    /// This is used to generate rows of cells.
    /// </summary>
    public class CellGenerator {
        private Random randomGenerator;

        public CellGenerator() {
            randomGenerator = new Random();
        }

        /// <summary>
        /// This is used to obtain random area inside of a circle.
        /// </summary>
        /// <returns></returns>
        private Point GetRandomPointInCircle(int radius) {
            double t = 2 * Math.PI * randomGenerator.NextDouble();
            double u = randomGenerator.NextDouble() + randomGenerator.NextDouble();
            double r = (u > 1) ? 2 - u : u;
            return new Point(Convert.ToInt32(radius * r * Math.Cos(t)), 
                Convert.ToInt32(radius * r * Math.Sin(t)));
        }

        /// <summary>
        /// It generates a single random cell that can be used. The location is also
        /// randomized as well.
        /// </summary>
        /// <param name="min">Minimum width/length</param>
        /// <param name="max">Maximum width/length</param>
        /// <returns>The resulting Cell type</returns>
        public Cell GenerateCell(int min, int max, int radius) {
            Cell output = GenerateCell(() => {
                return new Point(randomGenerator.Next(min, max),
                    randomGenerator.Next(min, max));
            });
            output.Location = GetRandomPointInCircle(radius);
            return output;
        }

        public List<Cell> GenerateCellList(int amount, int min, int max, int radius) {
            // Note: Need to use Park-Miller Normal Distribution.
            // Note 2: Changing this to Box-Mueller since I have no idea how to do NGD.
            List<Cell> output = new List<Cell>();

            // TODO: This should be interchangable
            double mean = 1;
            double std = .32;

            for (int a = 0; a < amount; a++) {
                double boxmueller = Formula.BoxMuellerND(mean, std);
                if (boxmueller < 0.0) boxmueller *= -1;
                output.Add(GenerateCell((int)(min * boxmueller), (int)(max * boxmueller), radius));
            }

            return output;
        }

        /// <summary>
        /// Use this function to generate a cell of your preference.
        /// </summary>
        /// <remarks>
        /// This generates a cell, based from the information given in the function and
        /// input. It uses the function to calculate the size. The input is provided for
        /// calculation automation within the function.
        /// </remarks>
        /// <typeparam name="K">Key type for input</typeparam>
        /// <typeparam name="V">Value type for input</typeparam>
        /// <param name="function">Function that produces the point</param>
        /// <param name="input">Dictionary input for the function</param>
        /// <returns></returns>
        public Cell GenerateCell<K, V>(Func<Dictionary<K, V>, Point> function, Dictionary<K, V> input = null) {
            return new Cell(function(input));
        }

        /// <summary>
        /// Use this function to generate a cell of your preference.
        /// </summary>
        /// <remarks>
        /// This generates a cell, based from the information given in the function. It
        /// uses the function to calculate the size.
        /// </remarks>
        /// <param name="function">Function that produces the point</param>
        /// <returns></returns>
        public Cell GenerateCell(Func<Point> function) {
            return new Cell(function());
        }
    }
}
