using System.Collections.Generic;

namespace procedural_dungeon_generator.DelaunayTriangulation.Interfaces {
    public interface IVoronoiCell
    {
        IPoint[] Points { get; }
        int Index { get; }
    }
}
