using procedural_dungeon_generator.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace procedural_dungeon_generator.DelaunayTriangulation {
    /// <summary>
    /// This method is inline and private for the library to use. This is the implementation
    /// of Delaunay Triangulation (https://en.wikipedia.org/wiki/Delaunay_triangulation).
    /// 
    /// Majority of the code piece in this namespace is made by Rafael Kuebler (https://github.com/RafaelKuebler/DelaunayVoronoi).
    /// To abide by the license, this particular library is also licensed with MIT license. The code pieces has been rewritten to 
    /// fit into this particular library. This library contains the Bowyer Watson implementation of this particular piece.
    /// </summary>
    class Triangulator {
        // TODO: Finish this.
        private int minX;
        private int minY;
        private int maxX;
        private int maxY;
        private IEnumerable<Triangle> border;
        private List<DTPoint> points;

        /// <summary>
        /// It takes an input of outer points.
        /// </summary>
        /// <param name="points"></param>
        public Triangulator(List<DTPoint> inputPoints) {
            // TODO: There's absolutely no reason to use `Point` here.

            minX = int.MaxValue; minY = int.MaxValue; maxX = 0; maxY = 0;
            
            // Gather data about min and max in the points.
            foreach (DTPoint point in inputPoints) {
                if (minX > point.X) minX = point.X;
                if (minY > point.Y) minY = point.Y;
                if (maxX < point.X) maxX = point.X;
                if (maxY < point.Y) maxY = point.Y;
            }

            // Generate border triangles.
            //DTPoint point0 = new DTPoint(minX, minY);
            //DTPoint point1 = new DTPoint(minX, maxY);
            //DTPoint point2 = new DTPoint(maxX, maxY);
            //DTPoint point3 = new DTPoint(minX, maxY);
            DTPoint point0 = new DTPoint(minX, minY);
            DTPoint point1 = new DTPoint(minX, maxY);
            DTPoint point2 = new DTPoint(maxX, maxY);
            DTPoint point3 = new DTPoint(maxX, minY);
            Triangle tri1 = new Triangle(point0, point1, point2);
            Triangle tri2 = new Triangle(point0, point2, point3);
            border = new List<Triangle>() { tri1, tri2 };

            // Put them into the points.
            points = new List<DTPoint>() { point0, point1, point2, point3 };
            //points = new List<DTPoint>();

            // Transform all input into DTPoints and put them in there.
            foreach (DTPoint point in inputPoints) {
                points.Add(point);
            }
        }

        public IEnumerable<Triangle> BoyerWatson() {
            HashSet<Triangle> triangulation = new HashSet<Triangle>(border);
            //HashSet<Triangle> triangulation = new HashSet<Triangle>();

            foreach (DTPoint point in points) {
                ISet<Triangle> badTriangles = FindBadTriangles(point, triangulation);
                List<Edge> polygon = FindHoleBoundaries(badTriangles);

                foreach (Triangle triangle in badTriangles) {
                    foreach (DTPoint vertex in triangle.Vertices) {
                        vertex.AdjacentTriangles.Remove(triangle);
                    }
                }
                triangulation.RemoveWhere(o => badTriangles.Contains(o));

                foreach (Edge edge in polygon.Where(possibleEdge => possibleEdge.PointA != point && possibleEdge.PointB != point)) {
                    var triangle = new Triangle(point, edge.PointA, edge.PointB);
                    //if (!triangulation.Contains(triangle)) triangulation.Add(triangle);
                    triangulation.Add(triangle);
                }
            }

            return triangulation;
        }

        private ISet<Triangle> FindBadTriangles(DTPoint point, HashSet<Triangle> triangles) {
            IEnumerable<Triangle> badTriangles = triangles.Where(o => o.IsPointInsideCircumcircle(point));
            return new HashSet<Triangle>(badTriangles);
        }

        private List<Edge> FindHoleBoundaries(ISet<Triangle> badTriangles) {
            List<Edge> edges = new List<Edge>();
            foreach (Triangle triangle in badTriangles) {
                edges.Add(new Edge(triangle.Vertices[0], triangle.Vertices[1]));
                edges.Add(new Edge(triangle.Vertices[1], triangle.Vertices[2]));
                edges.Add(new Edge(triangle.Vertices[2], triangle.Vertices[0]));
            }
            IEnumerable<IGrouping<Edge, Edge>> grouped = edges.GroupBy(o => o);
            IEnumerable<Edge> boundaryEdges = edges.GroupBy(o => o).Where(o => o.Count() == 1).Select(o => o.First());
            return boundaryEdges.ToList();
        }
    }
}
