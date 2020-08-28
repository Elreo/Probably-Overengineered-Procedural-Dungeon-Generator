using procedural_dungeon_generator.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace procedural_dungeon_generator.Algorithms.MinimumSpanningTree.KruskalsAlgorithm {
    // Reference: https://www.programmingalgorithms.com/algorithm/kruskal's-algorithm/

    struct Edge {
        public int source;      // Point A hash
        public int destination; // Point B hash
        public int weight;      // Distance
        public int hashRef;     // Hash reference of tunnel.
    }

    struct Subset {
        public int parent;
        public int rank;
    }

    class KruskalsMST {
        private Graph graph;
        List<Tunnel> tunnels;
        List<Cell> cells;

        public KruskalsMST(List<Cell> cells, List<Tunnel> tunnels) {
            // Implement this.
            // TODO: It's broken. It does not generate the intended solution.
            //throw new NotImplementedException();
            graph = new Graph(cells.Count, tunnels.Count);
            this.cells = cells;
            this.tunnels = tunnels;

            for (int a = 0; a < tunnels.Count; a++) {
                // TODO: This is a temporary fix. It follows the index number of the
                // cells that we have in this class.
                graph.Edges[a].source = cells.IndexOf(cells.Single(x => tunnels[a].CellHashA == x.GetHashCode()));
                graph.Edges[a].destination = cells.IndexOf(cells.Single(x => tunnels[a].CellHashB == x.GetHashCode()));
                //graph.Edges[a].source = tunnels[a].CellHashA;
                //graph.Edges[a].destination = tunnels[a].CellHashB;

                graph.Edges[a].weight = (int)cells.Single(o => tunnels[a].CellHashA == o.GetHashCode())
                    .LocationCenter.Distance(cells.Single(o => tunnels[a].CellHashB == o.GetHashCode())
                    .LocationCenter);
                graph.Edges[a].hashRef = tunnels[a].GetHashCode();
            }

            // Sort them.
            graph.Edges = graph.Edges.OrderBy(x => x.source).ThenBy(x => x.destination).ToArray();
        }

        /// <summary>
        /// Use this to get the processed cells.
        /// </summary>
        /// <returns></returns>
        public List<Cell> GetProcessedCells() {
            var result = GetValidTunnels();
            var output = new List<Cell>(cells);

            //We might need to get the cells that didn't make it.
            var discarded = tunnels.Where(x => !result.Contains(x)).ToList();

            // Remove all connections.
            foreach (Tunnel disc in discarded) {
                // Get the cells.
                var cellA = output.Single(x => x.GetHashCode() == disc.CellHashA);
                var cellB = output.Single(x => x.GetHashCode() == disc.CellHashB);

                // Remove references.
                output[output.IndexOf(cellA)].ConnectedCell.Remove(disc.CellHashB);
                output[output.IndexOf(cellB)].ConnectedCell.Remove(disc.CellHashA);
            }

            return output;
        }

        /// <summary>
        /// This is used to get all the valid tunnels.
        /// </summary>
        /// <returns></returns>
        public List<Tunnel> GetValidTunnels() {
            var result = GetValidTunnelsHashCode();
            return tunnels.Where(o => result.Contains(o.GetHashCode())).ToList();
        }

        /// <summary>
        /// Get list of valid tunnels via hash code.
        /// </summary>
        /// <returns></returns>
        public List<int> GetValidTunnelsHashCode() {
            return Kruskal(graph).Select(x => x.hashRef).ToList();
        }

        private int Find(Subset[] subsets, int i) {
            if (subsets[i].parent != i) {
                subsets[i].parent = Find(subsets, subsets[i].parent);
            }
            return subsets[i].parent;
        }

        private void Union(Subset[] subsets, int x, int y) {
            int xroot = Find(subsets, x);
            int yroot = Find(subsets, y);

            if (subsets[xroot].rank < subsets[yroot].rank) {
                subsets[xroot].parent = yroot;
            } else if (subsets[xroot].rank > subsets[yroot].rank) {
                subsets[yroot].parent = xroot;
            } else {
                subsets[yroot].parent = xroot;
                ++subsets[xroot].rank;
            }
        }

        private Edge[] Kruskal(Graph graph) {
            int verticesCount = graph.VerticesCount;
            Edge[] result = new Edge[verticesCount];
            int i = 0;
            int e = 0;

            Array.Sort(graph.Edges, (a, b) => {
                return a.weight.CompareTo(b.weight);
            });

            Subset[] subsets = new Subset[verticesCount];

            for (int v = 0; v < verticesCount; ++v) {
                subsets[v].parent = v;
                subsets[v].rank = 0;
            }

            while (e < verticesCount - 1) {
                Edge nextEdge = graph.Edges[i++];
                int x = Find(subsets, nextEdge.source);
                int y = Find(subsets, nextEdge.destination);

                if (x != y) {
                    result[e++] = nextEdge;
                    Union(subsets, x, y);
                }
            }

            return result;
        }
    }
}
