using System;
using System.Collections.Generic;
using System.Text;

using procedural_dungeon_generator.Common;
using procedural_dungeon_generator.Exceptions;

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

        public double SizeDeterminant { get; }

        public Point Center {
            get => Size / new Point(2, 2);
        }

        public Point LocationCenter {
            get => (Size / new Point(2, 2)) + Location;
        }

        public Cell(int sizeX, int sizeY) {
            Size = new Point(sizeX, sizeY);
            Location = new Point(0, 0);
            SizeDeterminant = 0;
        }

        public Cell(int sizeX, int sizeY, int locationX, int locationY) {
            Size = new Point(sizeX, sizeY);
            Location = new Point(locationX, locationY);
            SizeDeterminant = 0;
        }

        public Cell(int sizeX, int sizeY, int locationX, int locationY, double sizeDeterminant) {
            Size = new Point(sizeX, sizeY);
            Location = new Point(locationX, locationY);
            SizeDeterminant = sizeDeterminant;
        }

        public Cell(Point size) {
            Size = size;
            Location = new Point(0, 0);
            SizeDeterminant = 0;
        }

        public Cell(Point size, double sizeDeterminant) {
            Size = size;
            Location = new Point(0, 0);
            SizeDeterminant = sizeDeterminant;
        }

        public Cell(Point size, Point location) {
            Size = size;
            Location = location;
            SizeDeterminant = 0;
        }

        public Cell(Point size, Point location, double sizeDeterminant) {
            Size = size;
            Location = location;
            SizeDeterminant = sizeDeterminant;
        }

        /// <summary>
        /// Check if this cell collides with another cell, based on size and
        /// location.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CheckCollision(Cell other) {
            return Math.Abs(Location.X - other.Location.X) * 2 < (Size.X + other.Size.X) &&
                Math.Abs(Location.Y - other.Location.Y) * 2 < (Size.Y + other.Size.Y);
        }

        /// <summary>
        /// This method will compare its own center distance with other cell.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public double RadiusDistanceFrom(Cell other) {
            return Math.Sqrt((double)(
                Math.Pow(other.LocationCenter.X - LocationCenter.X, 2) + 
                Math.Pow(other.LocationCenter.Y - LocationCenter.Y, 2)));
        }
    }
}
