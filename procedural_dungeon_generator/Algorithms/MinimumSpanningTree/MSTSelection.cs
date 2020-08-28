using System;
using System.Collections.Generic;
using System.Text;

namespace procedural_dungeon_generator.Algorithms.MinimumSpanningTree {
    /// <summary>
    /// This enum is used by TunnelGenerator to select which
    /// minimum spanning tree algorithm should be used.
    /// </summary>
    public enum MSTSelection {
        Kruskal,
        Prim
    }
}
