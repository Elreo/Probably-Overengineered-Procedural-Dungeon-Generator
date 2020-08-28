using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

using procedural_dungeon_generator.Common;
using procedural_dungeon_generator.Components;
using procedural_dungeon_generator.DelaunayTriangulation;
using procedural_dungeon_generator.DelaunayTriangulation.Interfaces;
using procedural_dungeon_generator.Algorithms.MinimumSpanningTree;
using procedural_dungeon_generator.Algorithms.MinimumSpanningTree.KruskalsAlgorithm;
using procedural_dungeon_generator.Algorithms.MinimumSpanningTree.PrimsAlgorithm;
using procedural_dungeon_generator.Tunneler;

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
                o => (IPoint) new DelaunayTriangulation.Models.Point(o.LocationCenter.X, o.LocationCenter.Y, o.GetHashCode())).ToArray());

            // Process the result.
            delaunator.ForEachTriangleEdge(edge => {
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
        /// This method is used to remove overlapping tunnels. This method should be used every
        /// tunnel creation method to ensure no overlapping tunnels.
        /// 
        /// It is done by tracing the line to see if every point collides with another cell.
        /// This method is the simplest way to do this. To hasten the process, only process
        /// boxes that is within the range.
        /// 
        /// </summary>
        public void RemoveOverlapping() {
            foreach (Cell cell in cells) {
                // Gather all the cells.
                Point pointA = cell.LocationCenter;
                // TODO: Sometimes, there are more than one elements.
                List<Cell> PointBs = cell.ConnectedCell
                    .Select(hash => cells.Single(c => c.GetHashCode() == hash))
                    .ToList();

                // Select the unrelated cells that might be overlapping with this one's tunnel.
                // This will be the list that's scanned for overlap.
                // TODO: Unless you're generating hundreds of cells, for now, it scans everything.

                // Check for overlapping cells.
                foreach (Cell unrelatedCell in cells) {
                    // Check for its own and other connected cells.
                    //if (unrelatedCell == cell || cell.ConnectedCell.Contains(unrelatedCell.GetHashCode())) continue;
                    if (unrelatedCell == cell) continue;

                    // Iterate through the connected cells.
                    foreach (Cell pointB in PointBs) {
                        // You can't check your own.
                        if (unrelatedCell == pointB) break;

                        // Create the path points.
                        List<Point> pathPoints = CreatePathPoints(pointA, pointB.LocationCenter, 
                            (int)pointA.Distance(pointB.LocationCenter));

                        // Iterate through them and find any collision.
                        // For some reason, sometimes it detects a collision, sometimes it doesn't.
                        // Sometimes it deletes the wrong connection altogether! It needs fixing.
                        // NOTE: It's fixed. See this commit's message.
                        foreach (Point point in pathPoints) {
                            // Check if it collides.
                            if (unrelatedCell.CheckCollision(point)) {
                                // If it does, remove the tunnel connection.
                                cell.ConnectedCell.Remove(pointB.GetHashCode());
                                cells[cells.IndexOf(pointB)].ConnectedCell.Remove(cell.GetHashCode());

                                // There's no point on continuing since the tunnel is disconnected.
                                break;
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// This methods crates a list of points that goes from point A to B.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="pointsAmount"></param>
        /// <returns></returns>
        private List<Point> CreatePathPoints(Point a, Point b, int pointsAmount) {
            // https://stackoverflow.com/questions/21249739/how-to-calculate-the-points-between-two-given-points-and-given-distance
            pointsAmount += 1;
            double d = Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y)) / pointsAmount;
            double fi = Math.Atan2(b.Y - a.Y, b.X - a.X);

            //var outputPoints = new List<Point>(pointsAmount + 1);
            var outputPoints = new List<Point>();
            for (int i = 0; i <= pointsAmount; ++i) {
                outputPoints.Add(new Point((int)(a.X + i * d * Math.Cos(fi)), (int)(a.Y + i * d * Math.Sin(fi))));
            }
            return outputPoints;
        }

        /// <summary>
        /// This method is used to generate the actual tunnels. This one relies to the cell relationships to
        /// generate the tunnels.
        /// </summary>
        /// <returns></returns>
        public List<Tunnel> GenerateTunnel() {
            // TODO: Finish this.
            return new CellWallDigger(cells).CreateSimpleTunnels();
        }

        /// <summary>
        /// This method is used to reduce the amount of tunnels that exist and ensured that
        /// every cell is connected with the tunnel. It uses Minimum Spanning Tree implementation.
        /// There are multiple algorithm methods that can be used.
        /// </summary>
        public void MinimumSpanningTree(MSTSelection algorithm = MSTSelection.Kruskal) {
            // TODO: Implement minimum spanning tree.
            throw new NotImplementedException();
            switch (algorithm) {
                case MSTSelection.Kruskal: cells = new KruskalsMST(cells, GenerateTunnel()).GetProcessedCells(); break;
                case MSTSelection.Prim: throw new NotImplementedException("Prim not yet implemented."); break;
                default: throw new InvalidOperationException("Unknown algorithm.");
            }
        }

        /// <summary>
        /// This one is the simplest way to reduce the amount of tunnels. It does this by randomly
        /// removing one of the tunnels in the cell. However, to ensure that everything is still 
        /// connected with each other, it will not remove the last two tunnel.
        /// </summary>
        public void TrimCellConnections() {
            // Transform all cells to trimcells and sort them.
            List<TrimCell> trimCells = cells
                .Select(x => MakeTrimCell(x))
                .OrderBy(x => x.cell.ConnectedCell.Count)
                .ToList();

            // Iterate through them all.
            foreach (TrimCell trimCell in trimCells) {
                // Check if it has more than two and not yet trimmed.
                if (trimCell.cell.ConnectedCell.Count > 2 && !trimCell.trimmed) {
                    int index = new Random().Next(0, trimCell.cell.ConnectedCell.Count);
                    int hashB = trimCell.cell.ConnectedCell.ElementAt(index);

                    // Check for the partner's trim condition.
                    bool cond = false;
                    foreach (TrimCell tc in trimCells) {
                        if (tc.cell.GetHashCode() == hashB && tc.trimmed) {
                            cond = true;
                        }
                    }
                    if (cond) continue;

                    // If it does, remove one connection at random.

                    // Remove from itself.
                    trimCell.cell.ConnectedCell.Remove(hashB);
                    trimCell.trimmed = true;

                    // Remove from the other.
                    trimCells.ForEach(x => {
                        if (x.cell.GetHashCode() == hashB) {
                            x.cell.ConnectedCell.Remove(trimCell.cell.GetHashCode());
                            x.trimmed = true;
                        }
                    });
                }
            }

            // Now transform them back to regular cell lists and assign them to cells.
            cells = trimCells.Select(x => x.cell).ToList();
        }

        /// <summary>
        /// Reserved for the function up there. It's used to mark something
        /// when something was removed there.
        /// </summary>
        private class TrimCell {
            public Cell cell;
            public bool trimmed;
        }

        private TrimCell MakeTrimCell(Cell cell) {
            return new TrimCell() { cell = cell, trimmed = false };
        }

        public List<Cell> ExportCells() {
            return cells;
        }
    }

    
}
