using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace procedural_dungeon_generator.Algorithms.MinimumSpanningTree.PrimsAlgorithm {
    class Graph<T> where T: IComparable<T> {
        private readonly Dictionary<T, List<Edge<T>>> _adj;

        public Graph() {
            _adj = new Dictionary<T, List<Edge<T>>>();
        }

        public Dictionary<T, T> PrimMST(T start) {
            // TODO: Implement this.
            throw new NotImplementedException();
        }

        public void AddEdge(T source) {
            _adj[source] = new List<Edge<T>>();
        }

        public void AddEdge(T source, T dest, int weight) {
            List<Edge<T>> vertices;
            if (!_adj.TryGetValue(source, out vertices)) {
                vertices = new List<Edge<T>>();
            }

            vertices.Add(new Edge<T>(dest, weight));
            _adj[source] = vertices;
        }
    }
}
