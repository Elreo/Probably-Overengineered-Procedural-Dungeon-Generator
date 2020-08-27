using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using procedural_dungeon_generator.Components;
using procedural_dungeon_generator.DelaunayTriangulation;

namespace procedural_dungeon_generator.Generators {
    /// <summary>
    /// This class is used to generate tunnels from existing cells.
    /// </summary>
    public class TunnelGenerator {
        private Triangulator triangulator;
        private Voronoi voronoi;
        private int pointCount;
        private List<Cell> cells;

        /// <summary>
        /// Put the cells here and it will process everything.
        /// </summary>
        /// <param name="cells"></param>
        public TunnelGenerator(List<Cell> cells) {
            List<DTPoint> points = new List<DTPoint>();
            //cells.ForEach(o => points.Add(new DTPoint(o.LocationCenter, o.GetHashCode())));
            foreach (Cell cell in cells) {
                points.Add(new DTPoint(cell.LocationCenter, cell.GetHashCode()));
            }
            triangulator = new Triangulator(points);
            voronoi = new Voronoi();
            pointCount = points.Count;
            this.cells = cells;
        }

        /// <summary>
        /// This function will triangulate paths for the cells. It uses Delaunay Voronoi method,
        /// with Bowyer Watson implementation method.
        /// </summary>
        public void TriangulatePaths() {
            // New cell list.
            List<Cell> connectedCells = new List<Cell>();

            // Triangulate.
            List<Triangle> triangulation = triangulator.BoyerWatson().ToList();

            // What can we do with this?
            //IEnumerable<Edge> edges = voronoi.GenerateEdgesFromDelaunay(triangulation);

            // Get rid of any triangulation that is not connected to a cell.
            //List<int> mark = new List<int>();
            //for (int a = 0; a < triangulation.Count; a++) {
            //    if (triangulation[a].Vertices[0].ReferenceCellInstance == -1 ||
            //        triangulation[a].Vertices[1].ReferenceCellInstance == -1 ||
            //        triangulation[a].Vertices[2].ReferenceCellInstance == -1) { mark.Add(a); }
            //}

            //mark.Sort();
            //foreach (int i in mark) {
            //    triangulation.RemoveAt(i);
            //}

            List<Triangle> mark = new List<Triangle>();
            foreach (Triangle triangle in triangulation) {
                if (triangle.Vertices[0].ReferenceCellInstance == -1 ||
                    triangle.Vertices[1].ReferenceCellInstance == -1 ||
                    triangle.Vertices[2].ReferenceCellInstance == -1) {
                    //triangulation.Remove(triangle);
                    mark.Add(triangle);
                }
            }

            foreach (Triangle tri in mark) {
                triangulation.Remove(tri);
            }

            triangulation = triangulation.Distinct().ToList();

            // Remove duplicates.
            //bool duplicate = false;
            //do {
            //    Triangle dupe = null;
            //    foreach (Triangle triangle in triangulation) {
            //        foreach (Triangle tri in triangulation) {
            //            if (triangle.Vertices[0] == tri.Vertices[0] &&
            //                triangle.Vertices[1] == tri.Vertices[1] &&
            //                triangle.Vertices[2] == tri.Vertices[2] ||
            //                triangle.Vertices[0] == tri.Vertices[1] &&
            //                triangle.Vertices[1] == tri.Vertices[2] &&
            //                triangle.Vertices[2] == tri.Vertices[0] ||
            //                triangle.Vertices[0] == tri.Vertices[2] &&
            //                triangle.Vertices[2] == tri.Vertices[1] &&
            //                triangle.Vertices[1] == tri.Vertices[2]) {
            //                dupe = tri;
            //                duplicate = true;
            //            }
            //        }
            //        if (dupe != null) break;
            //    }
            //    if (dupe != null) triangulation.Remove(dupe);
            //    else duplicate = false;
            //} while (duplicate);

            // Attach the produced data to the cells.
            foreach (Triangle triangle in triangulation) {
                Cell c0 = cells.Where(o => triangle.Vertices[0].ReferenceCellInstance == o.GetHashCode()).First();
                Cell c1 = cells.Where(o => triangle.Vertices[1].ReferenceCellInstance == o.GetHashCode()).First();
                Cell c2 = cells.Where(o => triangle.Vertices[2].ReferenceCellInstance == o.GetHashCode()).First();

                // First cell.
                c0.ConnectedCell.Add(c1.GetHashCode());
                c0.ConnectedCell.Add(c2.GetHashCode());
                connectedCells.Add(c0);

                // Second cell.
                c1.ConnectedCell.Add(c0.GetHashCode());
                c1.ConnectedCell.Add(c2.GetHashCode());
                connectedCells.Add(c1);

                // Third cells.
                c2.ConnectedCell.Add(c0.GetHashCode());
                c2.ConnectedCell.Add(c1.GetHashCode());
                connectedCells.Add(c2);
            }

            cells = connectedCells;
        }

        // This is used to clear all the tunnels in the cells.
        public void ClearTunnels() {
            cells.ForEach(cell => cell.ConnectedCell = new List<int>());
        }

        /// <summary>
        /// Use this to export the cells.
        /// </summary>
        /// <returns></returns>
        public List<Cell> ExportCells() {
            return cells;
        }
    }
}
