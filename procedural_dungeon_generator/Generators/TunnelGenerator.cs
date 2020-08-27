using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using procedural_dungeon_generator.Components;
using procedural_dungeon_generator.DelaunayTriangulation;
using procedural_dungeon_generator.DelaunayTriangulation.Models;
using procedural_dungeon_generator.DelaunayTriangulation.Interfaces;

namespace procedural_dungeon_generator.Generators {
    /// <summary>
    /// This class is used to generate tunnels from existing cells.
    /// </summary>
    public class TunnelGenerator {
        private List<Cell> cells;
        private Delaunator delaunator;

        public TunnelGenerator(List<Cell> cells) {
            this.cells = cells;
        }

        /// <summary>
        /// This method utilizes the Delaunay Triangulation.
        /// </summary>
        public void DelaunayTriangulation() {
            // This is here because we're going to refresh the delaunator every time
            // it finishes doing the calculation.
            delaunator = new Delaunator(cells.Select(
                o => (IPoint) new Point(o.LocationCenter.X, o.LocationCenter.Y, o.GetHashCode())).ToArray());

            // Process the result.
            delaunator.ForEachTriangleEdge(edge => {
                //if (edge.P.CellReference == -1 || edge.Q.CellReference == -1) return;
                foreach (Cell cell in cells) {
                    //if (edge.P.CellReference == cell.GetHashCode()) {
                    //    cell.ConnectedCell.Add(cells.Single(o => o.GetHashCode() == edge.Q.CellReference).GetHashCode());
                    //}
                    //break;

                    // You probably don't want to do this. This is a temporary fix.
                    if (cell.LocationCenter.X == (int)edge.Q.X && cell.LocationCenter.Y == (int)edge.Q.Y) {
                        cell.ConnectedCell.Add(edge.P.CellReference);
                    }
                    if (cell.LocationCenter.X == (int)edge.P.X && cell.LocationCenter.Y == (int)edge.P.Y) {
                        cell.ConnectedCell.Add(edge.Q.CellReference);
                    }
                }
            });

        }

        /// <summary>
        /// This function is used to reduce the amount of tunnels that exist and ensured that
        /// every cell is connected with the tunnel. It uses Minimum Spanning Tree implementation.
        /// </summary>
        public void MinimumSpanningTree() {
            throw new NotImplementedException();
        }

        public List<Cell> ExportCells() {
            return cells;
        }
    }
}
