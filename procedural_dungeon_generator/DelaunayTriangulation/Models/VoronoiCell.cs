using procedural_dungeon_generator.DelaunayTriangulation.Interfaces;

namespace procedural_dungeon_generator.DelaunayTriangulation.Models {
    public struct VoronoiCell : IVoronoiCell
    {
        public IPoint[] Points { get; set; }
        public int Index { get; set; }
        public VoronoiCell(int triangleIndex, IPoint[] points)
        {
            Points = points;
            Index = triangleIndex;
        }
    }
}
