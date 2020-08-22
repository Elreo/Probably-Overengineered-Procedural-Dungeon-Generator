using System;
using System.Collections.Generic;
using System.Text;

using procedural_dungeon_generator.Components;
using procedural_dungeon_generator.Common;

namespace procedural_dungeon_generator.Adjusters {
    /// <summary>
    /// This static class contains all assortments of static methods that can
    /// be used to adjust the position of cells.
    /// </summary>
    public static class CellDistributor {
        /// <summary>
        /// This method separates all the cells from each other.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<Cell> FlockingSeparation(List<Cell> input) {
            foreach (Cell cell in input) {
                Point point = new Point();
                int neighborCount = 0;
                foreach (Cell innerCell in input) {
                    if (cell != innerCell) {

                    }
                }
            }

            return input;
        }
    }
}
