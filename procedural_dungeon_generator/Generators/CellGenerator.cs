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

        /// <summary>
        /// This is used to generate a list of cells. It should be noted that this one randomizes itself
        /// using mean and standard deviation number, indicated in the inputs. By default, mean is 1 and 
        /// stadard deviation is 0.32.
        /// 
        /// The cells location are generated in a shape of a circle. Use the locationRadius variable to adjust the
        /// size of the circle radius. It should be noted that all the cells will be overlapping.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="locationRadius"></param>
        /// <returns></returns>
        public List<Cell> GenerateCellList(int amount, int min, int max, int locationRadius, double mean = 1, double std = .32) {
            // Note: Need to use Park-Miller Normal Distribution.
            // Note 2: Changing this to Box-Mueller since I have no idea how to do NGD.
            List<Cell> output = new List<Cell>();

            for (int a = 0; a < amount; a++) {
                double boxmuller = Formula.BoxMullerMD(mean, std);
                if (boxmuller < 0.0) boxmuller *= -1;
                // output.Add(GenerateCell((int)(min * boxmueller), (int)(max * boxmueller), radius));
                // We can't use the one above since it didn't add the box muller 
                output.Add(new Cell(new Point(randomGenerator.Next((int) (min * boxmuller), (int) (max * boxmuller)),
                    randomGenerator.Next((int) (min * boxmuller), (int) (max * boxmuller))), GetRandomPointInCircle(locationRadius), boxmuller));
            }

            return output;
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
