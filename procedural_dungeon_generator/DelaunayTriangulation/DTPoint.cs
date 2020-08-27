using procedural_dungeon_generator.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace procedural_dungeon_generator.DelaunayTriangulation {
    /// <summary>
    /// This point class is reserved only for this particular namespace.
    /// </summary>
    class DTPoint {
        /// <summary>
        /// This is used to generate unique ID for every instance of this class being generated.
        /// </summary>
        private static int _counter;

        /// <summary>
        /// This is used to identify an instance of a class.
        /// </summary>
        private readonly int _instanceId = _counter++;

        public int X { get; }
        public int Y { get; }
        public HashSet<Triangle> AdjacentTriangles { get; }

        /// <summary>
        /// This one is used to identify who this point originally belongs to.
        /// It results in -1 if it does not reference anyone. It usually contains
        /// the hash code of the cell.
        /// </summary>
        public int ReferenceCellInstance { get; }


        public DTPoint(int x, int y) {
            X = x;
            Y = y;
            AdjacentTriangles = new HashSet<Triangle>();
            ReferenceCellInstance = -1;
        }

        public DTPoint(Point point) : this(point.X, point.Y) { }

        public DTPoint(int x, int y, int cellInstance) : this(x, y) {
            ReferenceCellInstance = cellInstance;
        }

        public DTPoint(Point point, int cellInstance) : this(point.X, point.Y, cellInstance) { }

        public override string ToString() {
            return $"{nameof(DTPoint)} {_instanceId} {{ X: {X}, Y: {Y} }}";
        }
    }
}
