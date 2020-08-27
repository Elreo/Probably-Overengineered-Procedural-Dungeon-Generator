using System.Collections.Generic;

namespace procedural_dungeon_generator.DelaunayTriangulation.Interfaces {
    public interface ITriangle
    {
        IEnumerable<IPoint> Points { get; }
        int Index { get; }
    }
}
