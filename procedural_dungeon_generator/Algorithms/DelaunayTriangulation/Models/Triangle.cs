using System.Collections.Generic;

using procedural_dungeon_generator.DelaunayTriangulation.Interfaces;

namespace procedural_dungeon_generator.DelaunayTriangulation.Models {
    public struct Triangle : ITriangle
    {
        public int Index { get; set; }

        public IEnumerable<IPoint> Points { get; set; }

        public Triangle(int t, IEnumerable<IPoint> points)
        {
            Points = points;
            Index = t;
        }
    }
}
