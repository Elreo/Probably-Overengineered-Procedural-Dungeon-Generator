using System;
using System.Collections.Generic;
using System.Text;

namespace procedural_dungeon_generator.DelaunayTriangulation {
    class Voronoi {
        public IEnumerable<Edge> GenerateEdgesFromDelaunay(IEnumerable<Triangle> triangulation) {
            HashSet<Edge> voronoiEdges = new HashSet<Edge>();
            foreach (Triangle triangle in triangulation) {
                foreach (Triangle neighbor in triangle.TrianglesWithSharedEdge) {
                    Edge edge = new Edge(triangle.Circumcenter, neighbor.Circumcenter);
                    voronoiEdges.Add(edge);
                }
            }
            return voronoiEdges;
        }
    }
}
