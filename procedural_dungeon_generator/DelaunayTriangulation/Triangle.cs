using procedural_dungeon_generator.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;

namespace procedural_dungeon_generator.DelaunayTriangulation {
    /// <summary>
    /// This class represents a triangle. A triangle is a shape that is comprised
    /// of three points in space. A triangle has three points and edges.
    /// </summary>
    class Triangle {
        public DTPoint[] Vertices { get; }
        public DTPoint Circumcenter { get; private set; }
        public double RadiusSquared;

        public IEnumerable<Triangle> TrianglesWithSharedEdge {
            get {
                HashSet<Triangle> neighbors = new HashSet<Triangle>();
                foreach (DTPoint vertex in Vertices) {
                    IEnumerable<Triangle> trianglesWithSharedEdge = vertex.AdjactmentTriangles.Where(
                        o => o != this && SharesEdgeWith(o));
                    neighbors.UnionWith(trianglesWithSharedEdge);
                }
                return neighbors;
            }
        }

        public Triangle(DTPoint point1, DTPoint point2, DTPoint point3) {
            // This is done to ensure that the given points are not equal. While it
            // may not happen in this use case, it's best to keep this here.
            if (point1 == point2 || point1 == point3 || point2 == point3) {
                throw new ArgumentException("One or more points are equal. All three points must be distinct points.");
            }

            if (IsCounterClockwise(point1, point2, point3)) {
                Vertices[0] = point1;
                Vertices[1] = point2;
                Vertices[2] = point3;
            } else {
                Vertices[0] = point1;
                Vertices[1] = point3;
                Vertices[2] = point2;
            }

            Vertices[0].AdjactmentTriangles.Add(this);
            Vertices[1].AdjactmentTriangles.Add(this);
            Vertices[2].AdjactmentTriangles.Add(this);

            UpdateCircumcircle();
        }

        private void UpdateCircumcircle() {
            // https://codefound.wordpress.com/2013/02/21/how-to-compute-a-circumcircle/#more-58
            // https://en.wikipedia.org/wiki/Circumscribed_circle

            DTPoint p0 = Vertices[0];
            DTPoint p1 = Vertices[1];
            DTPoint p2 = Vertices[2];

            int dA = p0.X * p0.X + p0.Y * p0.Y;
            int dB = p1.X * p1.X + p1.Y * p1.Y;
            int dC = p2.X * p2.X + p2.Y * p2.Y;

            int aux1 = dA * (p2.Y - p1.Y) + dB * (p0.Y - p2.Y) + dC * (p1.Y - p0.Y);
            int aux2 = -(dA * (p2.X - p1.X) + dB * (p0.X - p2.X) + dC * (p1.X - p0.X));
            int div = 2 * (p0.X * (p2.Y - p1.Y) + p1.X * (p0.Y - p2.Y) + p2.X * (p1.Y - p0.Y));

            if (div == 0) throw new DivideByZeroException();

            DTPoint center = new DTPoint(aux1 / div, aux2 / div);
            Circumcenter = center;
            RadiusSquared = (center.X - p0.X) * (center.X - p0.X) + (center.Y - p0.Y) * (center.Y - p0.Y);
        }

        private bool IsCounterClockwise(DTPoint point1, DTPoint point2, DTPoint point3) {
            return ((point2.X - point1.X) * (point3.Y - point1.Y) - (point3.X - point1.X) * (point2.Y - point1.Y)) > 0;
        }

        private bool SharesEdgeWith(Triangle triangle) {
            return Vertices.Where(o => triangle.Vertices.Contains(o)).Count() == 2;
        }

        public bool IsPointInsideCircumcircle(DTPoint point) {
            return ((point.X - Circumcenter.X) * (point.X - Circumcenter.X) + 
                (point.Y - Circumcenter.Y) * (point.Y - Circumcenter.Y)) < RadiusSquared;
        }
    }
}
