using procedural_dungeon_generator.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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
        public List<Tunnel> SimpleTunnels() {
            List<Tunnel> output = new List<Tunnel>();

            // Iterate through the cells.
            foreach (Cell cell in Cells) {
                // Get the cell relationship data.
                var otherCells = cell.ConnectedCell
                    .Select(hash => Cells.Single(c => c.GetHashCode() == hash))
                    .ToList();
            }

            // TODO: Finish this.

            return output;
        }
    }
}
