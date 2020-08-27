using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
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

        /// <summary>
        /// This list contains information about which other cells it is connected to.
        /// Typically, they are connected via tunnels.
        /// </summary>
        //public List<int> ConnectedCell { get; set; }
        public HashSet<int> ConnectedCell { get; set; }

        public Cell(int sizeX, int sizeY, int locationX, int locationY, double sizeDeterminant) : 
            this(new Point(sizeX, sizeY), new Point(locationX, locationY), sizeDeterminant) { }

        public Cell(int sizeX, int sizeY, int locationX, int locationY) : 
            this(sizeX, sizeY, locationX, locationY, 0) { }

        public Cell(Point size) : 
            this(size, new Point(0, 0), 0) { }

        public Cell(Point size, double sizeDeterminant) : 
            this(size, new Point(0, 0), sizeDeterminant) { }

        public Cell(Point size, Point location) : 
            this(size, location, 0) { }

        public Cell(Point size, Point location, double sizeDeterminant) {
            Size = size;
            Location = location;
            SizeDeterminant = sizeDeterminant;
            ConnectedCell = new HashSet<int>();
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
        /// Check if this cell collides with a point, based on size and location.
        /// 
        /// Based from .NET Core repo (https://github.com/dotnet/runtime/blob/master/src/libraries/System.Drawing.Primitives/src/System/Drawing/Rectangle.cs#L222).
        /// Somehow, even tho I copied it from there, it's broken.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CheckCollision(Point other) =>
            Location.X <= other.X &&
            other.X < Location.X + Size.X &&
            Location.Y <= other.Y &&
            other.Y < Location.Y + Size.Y;

        //public bool CheckCollision(Point other) {
        //return Location >= other && other <= (Location + Size);
        //return other.X > Location.X && other.X < (Location + Size).X &&
        //    other.Y > Location.Y && other.Y < (Location + Size).Y;
        //}

        //public bool CheckCollision(Point other) {

        //}

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

        public override int GetHashCode() {
            return Size.GetHashCode() ^ Location.GetHashCode() ^ (int)(SizeDeterminant * 1000);
        }

        public override string ToString() {
            return $"HashCode: {GetHashCode()}, LocationCenter: {LocationCenter}, " +
                $"ConnectedCell: {{ { string.Join(", ", ConnectedCell.Select(n => n.ToString()).ToArray()) } }}";
        }
    }
}
