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
        public HashSet<Triangle> AdjactmentTriangles { get; }

        public DTPoint(int x, int y) {
            X = x;
            Y = y;
            AdjactmentTriangles = new HashSet<Triangle>();
        }

        public override string ToString() {
            return $"{nameof(DTPoint)} {_instanceId} {{ X: {X}, Y: {Y} }}";
        }
    }
}
