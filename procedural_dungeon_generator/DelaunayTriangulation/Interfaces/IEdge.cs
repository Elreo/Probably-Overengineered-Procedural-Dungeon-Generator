namespace procedural_dungeon_generator.DelaunayTriangulation.Interfaces {
    public interface IEdge
    {
        IPoint P { get; }
        IPoint Q { get; }
        int Index { get; }
    }
}
