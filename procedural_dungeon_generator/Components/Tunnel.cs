using procedural_dungeon_generator.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace procedural_dungeon_generator.Components {
    /// <summary>
    /// A tunnel is comprised of cell A and B. This particular implementation
    /// is for a blocky tunnel.
    /// </summary>
    public class Tunnel {
        int CellHashA { get; }
        int CellHashB { get; }
        List<Point> AnglePoint { get; }

        public Tunnel(int cellHashA, int cellHashB) {
            CellHashA = cellHashA;
            CellHashB = cellHashB;
            AnglePoint = new List<Point>();
        }

        public Tunnel(int cellHashA, int cellHashB, List<Point> anglePoint) {
            CellHashA = cellHashA;
            CellHashB = cellHashB;
            AnglePoint = anglePoint;
        }

        public Tunnel (Cell a, Cell b) : this(a.GetHashCode(), b.GetHashCode()) { }
        public Tunnel (Cell a, Cell b, List<Point> anglePoint) : 
            this(a.GetHashCode(), b.GetHashCode(), anglePoint) { }
    }
}
