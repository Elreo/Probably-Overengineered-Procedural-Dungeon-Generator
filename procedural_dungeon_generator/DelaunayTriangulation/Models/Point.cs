using procedural_dungeon_generator.DelaunayTriangulation.Interfaces;

namespace procedural_dungeon_generator.DelaunayTriangulation.Models {
    public struct Point : IPoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        /// <summary>
        /// This is used to determine which cell this belongs to.
        /// This contains the hashcode of that particular cell.
        /// </summary>
        public int CellReference { get; }

        public Point(double x, double y, int cellReference)
        {
            X = x;
            Y = y;
            CellReference = cellReference;
        }
        public override string ToString() => $"{X},{Y}";
    }

}
