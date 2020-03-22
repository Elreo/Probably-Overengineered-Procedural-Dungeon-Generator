using System;
using System.Collections.Generic;
using System.Text;

using procedural_dungeon_generator.Common;

namespace procedural_dungeon_generator.Components {

    /// <summary>
    /// This cell class contains information and data about a single cell.
    /// 
    /// A single cell is a square-shaped 'room'. It is advised to generate these
    /// cells from CellGenerator class.
    /// </summary>
    public class Cell {
        public Point Size { get; }
        public Point Location { get; set; }

        public Point Center {
            get => new Point(2, 2) / Size;
        }

        public Point LocationCenter {
            get => Size / Location;
        }

        public Cell(int sizeX, int sizeY) {
            Size = new Point(sizeX, sizeY);
            Location = new Point(0, 0);
        }

        public Cell(int sizeX, int sizeY, int locationX, int locationY) {
            Size = new Point(sizeX, sizeY);
            Location = new Point(0, 0);
        }

        public Cell(Point size) {
            Size = size;
        }

        public Cell(Point size, Point location) {
            Size = size;
            Location = location;
        }
        
    }
}
