using procedural_dungeon_generator.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace procedural_dungeon_generator.Generators {
    /// <summary>
    /// This class is used to turn those cells and tunnels into grids.
    /// </summary>
    public class GridProcessor {
        List<Cell> cells;
        List<Tunnel> tunnels;

        public GridProcessor(List<Cell> c, List<Tunnel> t) {
            cells = c;
            tunnels = t;

            // TODO: Implement this.
            throw new NotImplementedException();
        }
    }
}
