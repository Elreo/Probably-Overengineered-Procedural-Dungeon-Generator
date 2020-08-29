using procedural_dungeon_generator.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Xml;
using procedural_dungeon_generator.Common;

namespace procedural_dungeon_generator.Tunneler {
    /// <summary>
    /// This is the actual tunnel creation class. This namespace may only
    /// be used by TunnelGenerator class.
    /// </summary>
    class CellWallDigger {
        private List<Cell> Cells { get; set; }

        public CellWallDigger(List<Cell> cells) {
            Cells = cells;
        }

        /// <summary>
        /// This one is the simplest tunnel implementation. Just make sure
        /// that everything has a shape of L and it's all good.
        /// </summary>
        /// <returns></returns>
        public List<Tunnel> CreateSimpleTunnels() {
            List<Tunnel> output = new List<Tunnel>();

            // Create a duplicate cells. This operation will break
            // the existing cells.
            List<Cell> dupeCells = Cells.Select(o => o).ToList();
            

            // Iterate through the cells.
            foreach (Cell cell in dupeCells) {
                // Get the cell relationship data.
                var otherCells = cell.ConnectedCell
                    .Select(hash => dupeCells.Single(c => c.GetHashCode() == hash))
                    .ToList();

                // Now that we know who connects to who, we make the tunnels.
                foreach (Cell relationCell in otherCells) {
                    Tunnel tunnel = new Tunnel(cell, relationCell);

                    // Check if the tunnel for this match exist or not. We don't want
                    // to create two tunnels going to the same destination.
                    bool outerExit = false;
                    foreach (Tunnel checkTunnel in output) {
                        if (checkTunnel.CellHashA == cell.GetHashCode() && checkTunnel.CellHashB == relationCell.GetHashCode() ||
                            checkTunnel.CellHashB == cell.GetHashCode() && checkTunnel.CellHashA == relationCell.GetHashCode())
                            outerExit = true;
                        break;
                    }
                    if (outerExit) continue;

                    // The easiest way to do this is to create a single curve every time
                    // there was a relationship. The curve depends on the position of both
                    // cells.

                    // We check the angle first. And then, add them from there.
                    double angle = cell.LocationCenter.GetAngle(relationCell.LocationCenter);
                    if (45 < angle && angle < 135 || 225 < angle && angle < 315) {
                        // From X axis.
                        tunnel.InsertAnglePoint(new Point(cell.LocationCenter.X, relationCell.LocationCenter.Y));
                    } else {
                        // From Y axis.
                        tunnel.InsertAnglePoint(new Point(relationCell.LocationCenter.X, cell.LocationCenter.Y));
                    }

                    output.Add(tunnel);
                }

            }
            return output;
        }

        /// <summary>
        /// This method is used to adjust the tunnels and ensure that every tunnel does not collide with each other.
        /// Notably, it follows the collision rules of its own cell.
        /// </summary>
        /// <param name="inputTunnels"></param>
        /// <param name="inputCells"></param>
        /// <returns></returns>
        public List<Tunnel> UncollideTunnnels(List<Tunnel> inputTunnels) {
            // TODO: Implement this.
            throw new NotImplementedException();
            List<Tunnel> output = new List<Tunnel>();

            // Iterate through the input.
            foreach (Tunnel tunnel in inputTunnels) {
                // The first thing we need is to get the cells.
                var cellA = Cells.Single(x => tunnel.CellHashA == x.GetHashCode());
                var cellB = Cells.Single(x => tunnel.CellHashB == x.GetHashCode());

                // Next, we make the points.
                var tunnelPath = Utility.CreatePathPoints(cellA.LocationCenter, 
                    cellB.LocationCenter, (int)cellA.RadiusDistanceFrom(cellB));
                
                // We remove the points that collides with its own connections
            }

            return output;
        }

    }
}
