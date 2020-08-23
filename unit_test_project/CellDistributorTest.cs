using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using procedural_dungeon_generator.Generators;
using procedural_dungeon_generator.Components;
using procedural_dungeon_generator.Adjusters;
using procedural_dungeon_generator.Exceptions;

namespace unit_test_project {
    [TestClass]
    public class CellDistributorTest {
        [TestMethod]
        public void FlockingSeparationTest1() {
            CellGenerator cellGenerator = new CellGenerator();
            List<Cell> cells = cellGenerator.GenerateCellList(150, 50, 250, 750);
            //List<Cell> rearrangedCells = CellDistributor.FlockingSeparation(cells, cells.Count * 2);
            List<Cell> rearrangedCells = null;
            while (true) {
                try {
                    rearrangedCells = CellDistributor.FlockingSeparation(cells, cells.Count * 2);
                    break;
                } catch (OutOfIterationException exception) {
                    Console.WriteLine("WARNING: Separation iteration has been exhausted. " +
                        "Iteration limit is " + exception.IterationNumber + ". Retrying with new dataset ...\n");
                    cells = cellGenerator.GenerateCellList(150, 50, 250, 750);
                }
            }

            foreach (Cell cell in rearrangedCells) {
                foreach (Cell otherCell in rearrangedCells) {
                    if (cell == otherCell) continue;
                    Assert.IsFalse(cell.CheckCollision(otherCell));
                }
            }

            Console.WriteLine("Separation and checking successful. It's clear.");
        }
    }
}
