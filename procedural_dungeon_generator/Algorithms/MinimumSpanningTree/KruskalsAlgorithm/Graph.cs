using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace procedural_dungeon_generator.Algorithms.MinimumSpanningTree.KruskalsAlgorithm {
    class Graph {
        public int VerticesCount { get; set; } // Cell amount.
        public int EdgesCount { get; set; } // Connection amount.

        
        public Edge[] Edges { get; set; }
        //public Dictionary<int, Edge> Edges { get; set; }

        public Graph(int verticesCount, int edgesCount) {
            VerticesCount = verticesCount;
            EdgesCount = edgesCount;
            Edges = new Edge[edgesCount];
            //Edges = new Dictionary<int, Edge>(edgesCount);
        }
    }
}
