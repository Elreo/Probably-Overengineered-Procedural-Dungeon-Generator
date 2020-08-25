using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using procedural_dungeon_generator.Components;
using procedural_dungeon_generator.Common;
using procedural_dungeon_generator.Exceptions;

namespace procedural_dungeon_generator.Adjusters {
    /// <summary>
    /// This static class contains all assortments of static methods that can
    /// be used to adjust the position of cells.
    /// </summary>
    public static class CellDistributor {

        /// <summary>
        /// Check for intersection of the cells.
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="cellList"></param>
        /// <returns></returns>
        private static List<Cell> FindIntersections(Cell checkCell, List<Cell> cellList) {
            List<Cell> output = new List<Cell>();

            foreach (Cell cell in cellList) {
                if (!cell.Equals(checkCell) && cell.CheckCollision(checkCell)) {
                    output.Add(cell);
                }
            }

            return output;
        }

        /// <summary>
        /// This method is used to get the overall bounding box of the overall cells.
        /// </summary>
        /// <param name="cellList"></param>
        /// <returns></returns>
        private static Cell CellSum(List<Cell> cellList) {
            // Get top left
            Point topLeft = new Point(int.MaxValue, int.MaxValue);
            foreach (Cell cell in cellList) {
                //if (cell.Location < topLeft) topLeft = cell.Location;
                if (cell.Location.X < topLeft.X) topLeft.X = cell.Location.X;
                if (cell.Location.Y < topLeft.Y) topLeft.Y = cell.Location.Y;
            }

            // Compare size and get the biggest one.
            Point bottomRight = new Point(0, 0);
            foreach (Cell cell in cellList) {
                if (cell.Location.X + cell.Size.X > bottomRight.X) bottomRight.X = cell.Location.X + cell.Size.X;
                if (cell.Location.Y + cell.Size.Y > bottomRight.Y) bottomRight.Y = cell.Location.Y + cell.Size.Y;
            }
            bottomRight -= topLeft;

            return new Cell(topLeft, bottomRight);
        }

        /// <summary>
        /// This method separates all the cells from each other.
        /// 
        /// This method will throw "OutOfIterationException" when it has hit the iteration limit.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="iterationLimit">This one is used to limit how many iterations it can go before throwing an error.</param>
        /// <returns></returns>
        public static List<Cell> FlockingSeparation(List<Cell> input, int iterationLimit) {
            //Point center = new Point(100, 100); // Center point of all cells.
            List<Cell> output = input.OrderBy(i => i.SizeDeterminant).Reverse().ToList();
            Point center = CellSum(output).Center;
            int movementFactor = 5;
            bool hasIntersections = true;

            // Note: Iteration is required just in case it takes too long to separate them.
            // TODO: Find a better way to fix this. Maybe randomize cell locations every time it failed?
            int iteration = 0;

            while (hasIntersections && iteration <= iterationLimit) {
                iteration++;
                hasIntersections = false;
                foreach (Cell cell in output) {
                    List<Cell> intersectingCells = FindIntersections(cell, output);
                    if (intersectingCells.Count != 0) {
                        Point movementVector = new Point(0, 0);
                        Point centerC = cell.Center;

                        // For each cell that overlaps each other.
                        foreach (Cell cPrime in intersectingCells) {
                            Point centerCPrime = cPrime.Center;

                            int xTransInner = centerC.X - centerCPrime.X;
                            int yTransInner = centerC.Y - centerCPrime.Y;

                            // Add a vector to v proportional to the vector between the center of C and C'.
                            // TODO: Make sure it's pushing towards the right direction, instead of just one.
                            movementVector.X += xTransInner < 0 ? -movementFactor : movementFactor;
                            movementVector.Y += yTransInner < 0 ? -movementFactor : movementFactor;
                        }

                        int xTrans = centerC.X - center.X;
                        int yTrans = centerC.Y - center.Y;

                        movementVector.X += xTrans < 0 ? -movementFactor : movementFactor;
                        movementVector.Y += yTrans < 0 ? -movementFactor : movementFactor;

                        cell.Location += movementVector;

                        hasIntersections = true;
                    }
                }

                // Throw an error if it hit the limit.
                if (iteration >= iterationLimit) { throw new OutOfIterationException(iteration); }
            }
            return output;
        }

        /// <summary>
        /// Use this function to get only specified amount of cells that you want. Typically, this
        /// will sort the cell in descending order first based off the `SizeDeterminant` number, before
        /// returning the one you want. This can be changed in the paramter.
        /// 
        /// It can throw `OverflowException` if the amount is higher than input's count.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<Cell> TrimCells(List<Cell> input, int amount, bool desc = true) {
            if (amount > input.Count) throw new OverflowException("The given amount parameter is higher than input's count.");
            if (desc) {
                return input.OrderBy(i => i.SizeDeterminant).Reverse().ToList().GetRange(0, amount);
            } else {
                return input.OrderBy(i => i.SizeDeterminant).ToList().GetRange(0, amount);
            }
        }
    }
}
