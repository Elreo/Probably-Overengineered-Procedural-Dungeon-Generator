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

        /// <summary>
        /// It's basically a size with extra multiplier. It's used for 
        /// cell distributor to set a distance.
        /// </summary>
        public Point IllusionSize { get; }

        public Point Location { get; set; }

        public double SizeDeterminant { get; }

        public Point Center {
            get => Size / new Point(2, 2);
        }

        public Point LocationCenter {
            get => (Size / new Point(2, 2)) + Location;
        }

        /// <summary>
        /// The illusionary center area.
        /// </summary>
        public Point IllusionLocationCenter {
            get => (IllusionSize / new Point(2, 2)) + Location;
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

        public Cell(Point size, Point location) : 
            this(size, location, 0) { }

        public Cell(Point size, Point location, double sizeDeterminant) : 
            this(size, location, sizeDeterminant, 0) {
        }

        public Cell(Point size, Point location, double sizeDeterminant, int illusionIncrement) {
            Size = size;
            Location = location;
            SizeDeterminant = sizeDeterminant;
            ConnectedCell = new HashSet<int>();
            IllusionSize = size + new Point(illusionIncrement, illusionIncrement);
        }

        /// <summary>
        /// Check if this cell collides with another cell, based on size and
        /// location. It returns true if it collides.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CheckCollision(Cell other) {
            return Math.Abs(Location.X - other.Location.X) * 2 < (Size.X + other.Size.X) &&
                Math.Abs(Location.Y - other.Location.Y) * 2 < (Size.Y + other.Size.Y);
        }

        /// <summary>
        /// Check if this cell collides with another cell, based on illusionary size and
        /// location. It returns true if it collides.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CheckCollisionWithIllusion(Cell other) {
            return Math.Abs(Location.X - other.Location.X) * 2 < (Size.X + other.IllusionSize.X) &&
                Math.Abs(Location.Y - other.Location.Y) * 2 < (Size.Y + other.IllusionSize.Y);
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

        public bool CheckCollisionWithIllusion(Point other) =>
            Location.X <= other.X &&
            other.X < Location.X + IllusionSize.X &&
            Location.Y <= other.Y &&
            other.Y < Location.Y + IllusionSize.Y;

        /// <summary>
        /// This implementation to get intersection with another line.
        /// 
        /// Reference: https://gamedev.stackexchange.com/questions/26004/how-to-detect-2d-line-on-line-collision
        /// </summary>
        /// <param name="linePointA"></param>
        /// <param name="linePointB"></param>
        /// <returns></returns>
        public bool LineIntersection(Point linePointA, Point linePointB) {
            Point LocationSize = Location + Size;

            double denominator = ((LocationSize.X - Location.X) * (linePointB.Y - linePointA.Y)) -
                ((LocationSize.Y - Location.Y) * (linePointB.X - linePointA.X));

            double numerator1 = ((Location.Y - linePointA.Y) * (linePointB.X - linePointA.X)) -
                ((Location.X - linePointA.X) * (linePointB.Y - linePointA.Y));

            double numerator2 = ((Location.Y - linePointA.Y) * (LocationSize.X - Location.X)) -
                ((Location.X - linePointA.X) * (LocationSize.Y - Location.Y));

            if (denominator == 0) return numerator1 == 0 && numerator2 == 0;

            double r = numerator1 / denominator;
            double s = numerator2 / denominator;

            bool result = (r >= 0 && r <= 1) && (s >= 0 && s <= 1);

            if (result) return result;

            // If it does not result true, check if the point ends juuuust right in the cell, but
            // did not touch the collision line.
            return CheckCollision(linePointA) || CheckCollision(linePointB);
        }

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
            //return Math.Sqrt((double)(
            //    Math.Pow(other.LocationCenter.X - LocationCenter.X, 2) + 
            //    Math.Pow(other.LocationCenter.Y - LocationCenter.Y, 2)));
            return LocationCenter.Distance(other.LocationCenter);
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
