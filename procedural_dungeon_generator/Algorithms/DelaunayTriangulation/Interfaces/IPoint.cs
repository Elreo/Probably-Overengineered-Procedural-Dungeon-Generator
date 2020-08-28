namespace procedural_dungeon_generator.DelaunayTriangulation.Interfaces {
    public interface IPoint
    {
        double X { get; set; }
        double Y { get; set; }
        int CellReference { get; }
    }
}
