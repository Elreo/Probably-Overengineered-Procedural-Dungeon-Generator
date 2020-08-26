using procedural_dungeon_generator.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace procedural_dungeon_generator.DelaunayTriangulation {
    /// <summary>
    /// This class represents an edge - a two points in space that is connected
    /// with each other, from A to B.
    /// </summary>
    class Edge {
        public DTPoint PointA { get; }
        public DTPoint PointB { get; }

        public Edge(DTPoint a, DTPoint b) {
            PointA = a;
            PointB = b;
        }

        public override bool Equals(object obj) {
            if (obj == null || obj.GetType() != GetType()) return false;
            Edge edge = obj as Edge;

            // If they are the same, even if reversed, they are equal.
            return (edge.PointA == PointA && edge.PointB == PointB) || (edge.PointA == PointB && edge.PointB == PointA);
        }
    }
}
