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
    class Cell {
        public Point Size { get; }
        public Point Location { get; }

        public Point Center {
            get => Size / Location;
        }

        public Cell(float sizeX, float sizeY) {
            Size = new Point(sizeX, sizeY);
            Location = new Point(0, 0);
        }

        public Cell(float sizeX, float sizeY, float locationX, float locationY) {
            Size = new Point(sizeX, sizeY);
            Location = new Point(0, 0);
        }

        
    }
}
