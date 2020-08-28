using System;
using System.Collections.Generic;
using System.Text;

namespace procedural_dungeon_generator.Algorithms.MinimumSpanningTree.PrimsAlgorithm {
    class Edge<T> {
        public T Node { get; }
        public int Weight { get; }

        public Edge(T node, int weight) {
            Node = node;
            Weight = weight;
        }
    }
}
