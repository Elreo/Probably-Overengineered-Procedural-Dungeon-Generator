using procedural_dungeon_generator.Common;
using procedural_dungeon_generator.Components;
using procedural_dungeon_generator.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace procedural_dungeon_generator.Generators {
    /// <summary>
    /// This class is used to turn those cells and tunnels into grids. WARNING: This particular
    /// implementation is extremely slow. It's best to use the async version of it.
    /// </summary>
    public class GridProcessor {
        public List<Cell> Cells { get; private set; }
        public List<Tunnel> Tunnels { get; private set; }
        public int Width { get; }
        public int Height { get; }
        private int NodeDivider { get; }

        /// <summary>
        /// It constructs the processor class.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="t"></param>
        /// <param name="width">Width of the grid.</param>
        /// <param name="height">Height of the grid.</param>
        public GridProcessor(List<Cell> c, List<Tunnel> t, int width, int height) {
            Cells = c;
            Tunnels = t;
            Width = width;
            Height = height;
            NodeDivider = 4;

            //throw new NotImplementedException();
        }

        public void ReplaceCells(List<Cell> cells) => Cells = cells;
        public void ReplaceTunnels(List<Tunnel> tunnels) => Tunnels = tunnels;

        /// <summary>
        /// This one creates a tunnel layer, with given set of grid.
        /// </summary>
        /// <param name="width">Height of the canvas.</param>
        /// <param name="height"></param>
        /// <returns></returns>
        public GridLayer CreateTunnelLayer() {
            GridLayer output = new GridLayer(Width, Height);

            // Adjust the coordinates first.
            //AdjustNegativePoint();

            // We can't do that since it will invalidate the hash in tunnels.
            // So we have to increase the thing manually.
            // TODO: There must be a better way to do this.
            Point requiredIncrement = (GetLowestPoint() * new Point(-1, -1)) + new Point(2, 2);

            // Get highest point of the block.
            //Point highestPoint = GetHighestPoint() + requiredIncrement;
            Point highestPoint = GetHighestPoint() + requiredIncrement;

            // Now we get the size of the blocks.
            Point blockSize = new Point(highestPoint.X / Width, highestPoint.Y / Height);

            // After we get the info, now we can process the tunnels.
            foreach (Tunnel tunnel in Tunnels) {
                // Get the points in the tunnel.
                //Point cellA = Cells.Single(obj => obj.GetHashCode() == tunnel.CellHashA).LocationCenter;
                //Point cellB = Cells.Single(obj => obj.GetHashCode() == tunnel.CellHashB).LocationCenter;
                Point cellA = Cells.Single(obj => obj.GetHashCode() == tunnel.CellHashA).LocationCenter + requiredIncrement;
                Point cellB = Cells.Single(obj => obj.GetHashCode() == tunnel.CellHashB).LocationCenter + requiredIncrement;

                // Turn the tunnels into points of path.
                List<Point> tunnelPaths = new List<Point>();
                if (tunnel.AnglePoint.Any()) {
                    // Insert the first one.
                    //tunnelPaths.AddRange(Utility.CreatePathPoints(cellA, tunnel.AnglePoint.First(), 
                    //    (int)cellA.Distance(tunnel.AnglePoint.First())));
                    tunnelPaths.AddRange(Utility.CreatePathPoints(cellA, tunnel.AnglePoint.First() + requiredIncrement,
                        (int) cellA.Distance(tunnel.AnglePoint.First() + requiredIncrement)));

                    // Insert the stuff in between.
                    if (tunnel.AnglePoint.Count > 1) {
                        for (int a = 1; a <= tunnel.AnglePoint.Count; a++) {
                            //tunnelPaths.AddRange(Utility.CreatePathPoints(tunnelPaths[a - 1], tunnelPaths[a], 
                            //    (int)tunnelPaths[a - 1].Distance(tunnelPaths[a])));
                            tunnelPaths.AddRange(Utility.CreatePathPoints(tunnelPaths[a - 1] + requiredIncrement, 
                                tunnelPaths[a] + requiredIncrement,
                                (int) (tunnelPaths[a - 1] + requiredIncrement)
                                .Distance(tunnelPaths[a] + requiredIncrement) / NodeDivider));
                        }
                    }

                    // Insert the last one.
                    //tunnelPaths.AddRange(Utility.CreatePathPoints(cellB, tunnel.AnglePoint.Last(),
                    //    (int) cellA.Distance(tunnel.AnglePoint.First())));
                    tunnelPaths.AddRange(Utility.CreatePathPoints(cellB, 
                        tunnel.AnglePoint.Last() + requiredIncrement,
                        (int)cellB.Distance(tunnel.AnglePoint.First() + requiredIncrement) / NodeDivider));

                } else {
                    tunnelPaths.AddRange(Utility.CreatePathPoints(cellA + requiredIncrement, 
                        cellB + requiredIncrement, 
                        (int) cellA.Distance(cellB) / NodeDivider));
                }

                // Now we simply need to process all the points. We do this by iterating through all the 
                // blocks and change the one that collides with every points.
                for (int x = 0, xCoord = 0; x < Width && xCoord <= highestPoint.X; x++, xCoord += blockSize.X) {
                    for (int y = 0, yCoord = 0; y < Height && yCoord <= highestPoint.Y; y++, yCoord += blockSize.Y) {
                        // Make a temp cell to check for collision.
                        Cell blockCell = new Cell(blockSize, new Point(xCoord, yCoord));

                        // Iterate through path points.
                        foreach (Point point in tunnelPaths) {
                            // Check for collision. If so, set the grid appropriately and break.
                            if (blockCell.CheckCollision(point)) {
                                output[x, y].Type = BlockType.Tunnel;
                                break;
                            }
                        }
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// This method is used to create the cell wall layer of a grid.
        /// </summary>
        /// <returns></returns>
        public GridLayer CreateCellWallLayer() {
            GridLayer output = new GridLayer(Width, Height);

            // Adjust negative points first.
            //AdjustNegativePoint();

            Point requiredIncrement = (GetLowestPoint() * new Point(-1, -1)) + new Point(2, 2);

            // Get highest point of the block.
            //Point highestPoint = GetHighestPoint();
            Point highestPoint = GetHighestPoint() + requiredIncrement;

            // Now we get the size of the blocks.
            Point blockSize = new Point(highestPoint.X / Width, highestPoint.Y / Height);

            // After getting the info, we can process the cells.
            foreach (Cell cell in Cells) {
                // Each cell has 4 points. We get all those 4 points.
                // TODO: X and Y might be flipped ... ?
                //Point topLeft = cell.Location;
                //Point topRight = new Point(cell.Location.X, cell.Location.Y + cell.Size.Y);
                //Point bottomLeft = new Point(cell.Location.X + cell.Size.X, cell.Location.Y);
                //Point bottomRight = cell.Location + cell.Size;
                Point topLeft = cell.Location + requiredIncrement;
                Point topRight = new Point(cell.Location.X, cell.Location.Y + cell.Size.Y) + requiredIncrement;
                Point bottomLeft = new Point(cell.Location.X + cell.Size.X, cell.Location.Y) + requiredIncrement;
                Point bottomRight = cell.Location + cell.Size + requiredIncrement;

                //Create the path points.
                List<Point> cellPaths = new List<Point>();

                cellPaths.AddRange(Utility.CreatePathPoints(topLeft, topRight, (int) topLeft.Distance(topRight) / NodeDivider));
                cellPaths.AddRange(Utility.CreatePathPoints(topRight, bottomRight, (int) topRight.Distance(bottomRight) / NodeDivider));
                cellPaths.AddRange(Utility.CreatePathPoints(bottomRight, bottomLeft, (int) bottomRight.Distance(bottomLeft) / NodeDivider));
                cellPaths.AddRange(Utility.CreatePathPoints(bottomLeft, topLeft, (int) bottomLeft.Distance(topLeft) / NodeDivider));


                //Now we simply need to process all the points.We do this by iterating through all the
                // blocks and change the one that collides with every points.
                for (int x = 0, xCoord = 0; x < Width && xCoord <= highestPoint.X; x++, xCoord += blockSize.X) {
                    for (int y = 0, yCoord = 0; y < Height && yCoord <= highestPoint.Y; y++, yCoord += blockSize.Y) {
                        // Make a temp cell to check for collision.
                        Cell blockCell = new Cell(blockSize, new Point(xCoord, yCoord));

                        // Iterate through path points.
                        foreach (Point point in cellPaths) {
                            // Check for collision. If so, set the grid appropriately and break.
                            if (blockCell.CheckCollision(point)) {
                                output[x, y].Type = BlockType.RoomWall;
                                break;
                            }
                        }
                    }
                }
            }

            return output;
        }

        public GridLayer CreateCellLayer() {
            GridLayer output = new GridLayer(Width, Height);

            Point requiredIncrement = (GetLowestPoint() * new Point(-1, -1)) + new Point(2, 2);

            // Get highest point of the block.
            Point highestPoint = GetHighestPoint() + requiredIncrement;

            // Now we get the size of the blocks.
            Point blockSize = new Point(highestPoint.X / Width, highestPoint.Y / Height);

            // After getting the info, we can process the cells.
            foreach (Cell cell in Cells) {
                // We need to find the top left and bottom right coordinates for the boxes.
                int xTL = 1, yTL = 1;
                int xBR = 1, yBR = 1;
                for (int x = 0, xCoord = 0; x < Width && xCoord <= highestPoint.X; x++, xCoord += blockSize.X) {
                    for (int y = 0, yCoord = 0; y < Height && yCoord <= highestPoint.Y; y++, yCoord += blockSize.Y) {
                        // Make temp cell to check for collision.
                        Cell blockCell = new Cell(blockSize, new Point(xCoord, yCoord));

                        // Check if cell point collides with it.
                        if (blockCell.CheckCollision(cell.Location + requiredIncrement)) {
                            xTL = x;
                            yTL = y;
                        }

                        if (blockCell.CheckCollision(cell.Location + cell.Size + requiredIncrement)) {
                            xBR = x;
                            yBR = y;
                        }
                    }
                }

                // After finding them, fill them in.
                for (int x = xTL; x < xBR; x++) {
                    for (int y = yTL; y < yBR; y++) {
                        output[x, y].Type = BlockType.Room;
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// This method is used to merge two grids together. It should be noted that the second grid
        /// will overlap stuff on the first grid.
        /// </summary>
        /// <returns></returns>
        public static GridLayer MergeGrid(GridLayer firstGrid, GridLayer secondGrid) {
            // Check for their size.
            if (firstGrid.Width != secondGrid.Width || firstGrid.Height != secondGrid.Height) {
                throw new InvalidLayerSizeException(secondGrid.Width, secondGrid.Height, 
                    firstGrid.Width, firstGrid.Height);
            }

            // Create new grid with first grid as reference.
            GridLayer output = firstGrid;

            // Replace everything that's not empty from second grid to first grid.
            for (int width = 0; width < output.Width; width++) {
                for (int height = 0; height < output.Height; height++) {
                    if (secondGrid[width, height].Type != BlockType.Empty) {
                        output[width, height].Type = secondGrid[width, height].Type;
                    }
                }
            }

            return output;
        }

        private Point GetHighestPoint() {
            // Get the maximum coordinates of the cells and tunnels.
            Point highestPoint = new Point(1, 1);

            Cells.ForEach(obj => highestPoint.X = highestPoint.X < (obj.Location + obj.Size).X ? (obj.Location + obj.Size).X : highestPoint.X);
            Cells.ForEach(obj => highestPoint.Y = highestPoint.Y < (obj.Location + obj.Size).Y ? (obj.Location + obj.Size).Y : highestPoint.Y);

            Tunnels.ForEach(obj => {
                obj.AnglePoint.ForEach(obj2 => highestPoint.X = highestPoint.X < obj2.X ? obj2.X : highestPoint.X);
                obj.AnglePoint.ForEach(obj2 => highestPoint.Y = highestPoint.Y < obj2.Y ? obj2.Y : highestPoint.Y);
            });

            // Add some space.
            // TODO: For some reason, certain parts on the very right bottom would get cut off.
            // Apparently it has something to do with space. So, this is here is a temp fix.
            //highestPoint += increment;
            highestPoint += new Point(100, 100);

            return highestPoint;
        }

        /// <summary>
        /// This function is used to adjust the tunnel and cell coordinates.
        /// </summary>
        private void AdjustNegativePoint() {
            // First, we need to shift the cells and tunnels to get rid of
            // any negative coordinates.
            Point lowestNegative = GetLowestPoint();

            // Invert them and add 2, and then add them back to the cells and tunnels.
            // Only add the ones that are in a negative.
            if (lowestNegative.X < 0) {
                lowestNegative.X *= -1;
                lowestNegative.X += 2;
                Cells.ForEach(obj => obj.Location.X += lowestNegative.X);
                Tunnels.ForEach(obj => obj.AnglePoint.ForEach(obj2 => obj2.X += lowestNegative.X));
            }
            if (lowestNegative.Y < 0) {
                lowestNegative.Y *= -1;
                lowestNegative.Y += 2;
                Cells.ForEach(obj => obj.Location.Y += lowestNegative.Y);
                Tunnels.ForEach(obj => obj.AnglePoint.ForEach(obj2 => obj2.Y += lowestNegative.Y));
            }
        }

        /// <summary>
        /// This is used to get the very top left corner of the grid.
        /// </summary>
        /// <returns></returns>
        private Point GetLowestPoint() {
            Point lowestNegative = new Point(1, 1);

            // Check the cells for lowest negative.
            Cells.ForEach(obj => lowestNegative.X = lowestNegative.X > obj.Location.X ? obj.Location.X : lowestNegative.X);
            Cells.ForEach(obj => lowestNegative.Y = lowestNegative.Y > obj.Location.Y ? obj.Location.Y : lowestNegative.Y);

            // Check the tunnels for lowest negative.
            Tunnels.ForEach(obj => {
                obj.AnglePoint.ForEach(obj2 => lowestNegative.X = lowestNegative.X > obj2.X ? obj2.X : lowestNegative.X);
                obj.AnglePoint.ForEach(obj2 => lowestNegative.Y = lowestNegative.Y > obj2.Y ? obj2.Y : lowestNegative.Y);
            });

            return lowestNegative;
        }
    }
}
