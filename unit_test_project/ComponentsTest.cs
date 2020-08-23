using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using procedural_dungeon_generator.Components;
using procedural_dungeon_generator.Common;

namespace unit_test_project {
    [TestClass]
    public class ComponentsTest {
        
        [TestMethod]
        public void CellCollisionTest1() {
            Cell cell1 = new Cell(50, 50, 0, 0);
            Cell cell2 = new Cell(50, 50, 25, 25);

            // Check if collided from bottom right.
            Assert.IsTrue(cell1.CheckCollision(cell2), "Bottom right failed.");

            // Check if collided from top left.
            Assert.IsTrue(cell2.CheckCollision(cell1), "Top left failed.");
        }

        [TestMethod]
        public void CellCollisionTest2() {
            Cell cell1 = new Cell(50, 50, 50, 0);
            Cell cell2 = new Cell(50, 50, 25, 25);

            // Check if collided from top right.
            Assert.IsTrue(cell1.CheckCollision(cell2), "Top right failed.");

            // Check if collided from bottom left.
            Assert.IsTrue(cell2.CheckCollision(cell1), "Bottom left failed.");
        }

        [TestMethod]
        public void CellCollisionTest3() {
            // Check for non-collision bottom.
            Cell cell1 = new Cell(50, 50, 0, 0);
            Cell cell2 = new Cell(50, 50, 51, 1);

            // Check if not collided.
            Assert.IsFalse(cell1.CheckCollision(cell2));
            Assert.IsFalse(cell2.CheckCollision(cell1));
        }

        [TestMethod]
        public void CellCollisionTest4() {
            // Check for non-collision right.
            Cell cell1 = new Cell(50, 50, 0, 0);
            Cell cell2 = new Cell(50, 50, 1, 51);

            // Check if not collided.
            Assert.IsFalse(cell1.CheckCollision(cell2));
            Assert.IsFalse(cell2.CheckCollision(cell1));
        }

        [TestMethod]
        public void CellCollisionTest5() {
            // Check for collision corner.
            Cell cell1 = new Cell(50, 50, 0, 0);
            Cell cell2 = new Cell(50, 50, 51, 51);

            // Check if not collided.
            Assert.IsFalse(cell1.CheckCollision(cell2));
            Assert.IsFalse(cell2.CheckCollision(cell1));
        }

        [TestMethod]
        public void CellDistanceFromTest1() {
            Cell cell1 = new Cell(0, 0, 0, 0);
            Cell cell2 = new Cell(0, 0, 6, 8);

            Assert.AreEqual(10.0, cell1.RadiusDistanceFrom(cell2));
        }

        [TestMethod]
        public void CellDistanceFromTest2() {
            Cell cell1 = new Cell(0, 0, 2, 4);
            Cell cell2 = new Cell(0, 0, 26, 9);

            //Assert.AreEqual(24.5, cell1.DistanceFrom(cell2));

            // Note: There's a floating point error here.
            Assert.AreEqual(24.515301344262525, cell1.RadiusDistanceFrom(cell2));
        }

        
    }
}
