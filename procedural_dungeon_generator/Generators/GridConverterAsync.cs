using procedural_dungeon_generator.Common;
using procedural_dungeon_generator.Components;
using procedural_dungeon_generator.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace procedural_dungeon_generator.Generators {
    /// <summary>
    /// This class is used to turn those cells and tunnels into grids. This is the async version
    /// of the grid converter.
    /// </summary>
    public class GridConverterAsync {
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
        public GridConverterAsync(List<Cell> c, List<Tunnel> t, int width, int height) {
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
        public async Task<GridLayerAsync> CreateTunnelLayer() {
            GridLayerAsync output = new GridLayerAsync(Width, Height);

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
            // Now async!
            List<Task> tasks = new List<Task>();
            foreach (Tunnel tunnel in Tunnels) {
                Tunnel tempTunnel = tunnel;
                tasks.Add(Task.Run(async () => {
                    // Get the points in the tunnel.
                    Point cellA = Cells.Single(obj => obj.GetHashCode() == tempTunnel.CellHashA).LocationCenter + requiredIncrement;
                    Point cellB = Cells.Single(obj => obj.GetHashCode() == tempTunnel.CellHashB).LocationCenter + requiredIncrement;

                    // Turn the tunnels into points of path.
                    List<Point> tunnelPaths = new List<Point>();
                    if (tempTunnel.AnglePoint.Any()) {
                        // Insert the first one.
                        tunnelPaths.AddRange(Utility.CreatePathPoints(cellA, tempTunnel.AnglePoint.First() + requiredIncrement,
                            (int) cellA.Distance(tempTunnel.AnglePoint.First() + requiredIncrement)));

                        // Insert the stuff in between.
                        if (tempTunnel.AnglePoint.Count > 1) {
                            for (int a = 1; a <= tempTunnel.AnglePoint.Count; a++) {
                                tunnelPaths.AddRange(Utility.CreatePathPoints(tunnelPaths[a - 1] + requiredIncrement,
                                    tunnelPaths[a] + requiredIncrement,
                                    (int) (tunnelPaths[a - 1] + requiredIncrement)
                                    .Distance(tunnelPaths[a] + requiredIncrement) / NodeDivider));
                            }
                        }

                        // Insert the last one.
                        tunnelPaths.AddRange(Utility.CreatePathPoints(cellB,
                            tempTunnel.AnglePoint.Last() + requiredIncrement,
                            (int) cellB.Distance(tempTunnel.AnglePoint.First() + requiredIncrement) / NodeDivider));

                    } else {
                        tunnelPaths.AddRange(Utility.CreatePathPoints(cellA + requiredIncrement,
                            cellB + requiredIncrement,
                            (int) cellA.Distance(cellB) / NodeDivider));
                    }

                    // Now we simply need to process all the points. We do this by iterating through all the 
                    // blocks and change the one that collides with every points.
                    // Now async!
                    List<Task> tasks1 = new List<Task>();
                    for (int x = 0, xCoord = 0; x < Width && xCoord <= highestPoint.X; x++, xCoord += blockSize.X) {
                        for (int y = 0, yCoord = 0; y < Height && yCoord <= highestPoint.Y; y++, yCoord += blockSize.Y) {
                            int tx = x;
                            int ty = y;
                            int cx = xCoord;
                            int cy = yCoord;
                            tasks1.Add(Task.Run(() => {
                                // Make a temp cell to check for collision.
                                Cell blockCell = new Cell(blockSize, new Point(cx, cy));

                                // Iterate through path points.
                                foreach (Point point in tunnelPaths) {
                                    // Check for collision. If so, set the grid appropriately and break.
                                    if (blockCell.CheckCollision(point)) {
                                        output[tx, ty].Type = BlockType.Tunnel;
                                        break;
                                    }
                                }
                            }));
                        }
                    }
                    await Task.WhenAll(tasks1);
                }));
            }

            await Task.WhenAll(tasks);

            return output;
        }

        /// <summary>
        /// This method is used to create the cell wall layer of a grid.
        /// </summary>
        /// <returns></returns>
        public async Task<GridLayerAsync> CreateCellWallLayer() {
            GridLayerAsync output = new GridLayerAsync(Width, Height);

            Point requiredIncrement = (GetLowestPoint() * new Point(-1, -1)) + new Point(2, 2);

            // Get highest point of the block.
            Point highestPoint = GetHighestPoint() + requiredIncrement;

            // Now we get the size of the blocks.
            Point blockSize = new Point(highestPoint.X / Width, highestPoint.Y / Height);

            // After getting the info, we can process the cells.
            // Now async!
            List<Task> tasks = new List<Task>();
            foreach (Cell cell in Cells) {
                Cell tempCell = cell;
                tasks.Add(Task.Run(async () => {
                    // Each cell has 4 points. We get all those 4 points.
                    Point topLeft = tempCell.Location + requiredIncrement;
                    Point topRight = new Point(tempCell.Location.X, tempCell.Location.Y + tempCell.Size.Y) + requiredIncrement;
                    Point bottomLeft = new Point(tempCell.Location.X + tempCell.Size.X, tempCell.Location.Y) + requiredIncrement;
                    Point bottomRight = tempCell.Location + tempCell.Size + requiredIncrement;

                    //Create the path points.
                    List<Point> cellPaths = new List<Point>();

                    cellPaths.AddRange(Utility.CreatePathPoints(topLeft, topRight, (int) topLeft.Distance(topRight) / NodeDivider));
                    cellPaths.AddRange(Utility.CreatePathPoints(topRight, bottomRight, (int) topRight.Distance(bottomRight) / NodeDivider));
                    cellPaths.AddRange(Utility.CreatePathPoints(bottomRight, bottomLeft, (int) bottomRight.Distance(bottomLeft) / NodeDivider));
                    cellPaths.AddRange(Utility.CreatePathPoints(bottomLeft, topLeft, (int) bottomLeft.Distance(topLeft) / NodeDivider));


                    //Now we simply need to process all the points.We do this by iterating through all the
                    // blocks and change the one that collides with every points.
                    // Now async!
                    List<Task> tasks1 = new List<Task>();
                    for (int x = 0, xCoord = 0; x < Width && xCoord <= highestPoint.X; x++, xCoord += blockSize.X) {
                        for (int y = 0, yCoord = 0; y < Height && yCoord <= highestPoint.Y; y++, yCoord += blockSize.Y) {
                            int tx = x;
                            int ty = y;
                            int cx = xCoord;
                            int cy = yCoord;
                            tasks1.Add(Task.Run(() => {
                                // Make a temp cell to check for collision.
                                Cell blockCell = new Cell(blockSize, new Point(cx, cy));

                                // Iterate through path points.
                                foreach (Point point in cellPaths) {
                                    // Check for collision. If so, set the grid appropriately and break.
                                    if (blockCell.CheckCollision(point)) {
                                        output[tx, ty].Type = BlockType.RoomWall;
                                        break;
                                    }
                                }
                            }));
                        }
                    }
                    await Task.WhenAll(tasks1);
                }));
            }

            await Task.WhenAll(tasks);

            return output;
        }

        public async Task<GridLayerAsync> CreateCellLayer() {
            GridLayerAsync output = new GridLayerAsync(Width, Height);

            Point requiredIncrement = (GetLowestPoint() * new Point(-1, -1)) + new Point(2, 2);

            // Get highest point of the block.
            Point highestPoint = GetHighestPoint() + requiredIncrement;

            // Now we get the size of the blocks.
            Point blockSize = new Point(highestPoint.X / Width, highestPoint.Y / Height);

            // After getting the info, we can process the cells.
            // Now async!
            List<Task> tasks = new List<Task>();
            foreach (Cell cell in Cells) {
                Cell tempCell = cell;
                tasks.Add(Task.Run(() => {
                    // We need to find the top left and bottom right coordinates for the boxes.
                    int xTL = 1, yTL = 1;
                    int xBR = 1, yBR = 1;
                    for (int x = 0, xCoord = 0; x < Width && xCoord <= highestPoint.X; x++, xCoord += blockSize.X) {
                        for (int y = 0, yCoord = 0; y < Height && yCoord <= highestPoint.Y; y++, yCoord += blockSize.Y) {
                            // Make temp cell to check for collision.
                            Cell blockCell = new Cell(blockSize, new Point(xCoord, yCoord));

                            // Check if cell point collides with it.
                            if (blockCell.CheckCollision(tempCell.Location + requiredIncrement)) {
                                xTL = x;
                                yTL = y;
                            }

                            if (blockCell.CheckCollision(tempCell.Location + tempCell.Size + requiredIncrement)) {
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
                }));
            }

            await Task.WhenAll(tasks);

            return output;
        }


        /// <summary>
        /// This static method is used to generate doors - things that connects the tunnels
        /// and rooms.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static GridLayerAsync GenerateConnections(GridLayerAsync grid) {
            // This is used to blacklist blocks from checks. This is used
            // to prevent creating a large amount of doors in very close
            // proximity.
            List<(int, int)> noScan = new List<(int, int)>();
            GridLayerAsync output = grid;

            // We iterate through the grids.
            for (int x = 0; x < grid.Width; x++) {
                for (int y = 0; y < grid.Height; y++) {
                    // Check if it's blacklisted.
                    if (noScan.Contains((x, y))) continue;

                    // Now check for surrounding. Here's the condition of getting marked
                    // as a room connection blacklist;
                    // - Block has to have a tunnel on its side.
                    // - If the block has been marked, it will check for another in its sides,
                    //   touching room. If it has, blacklist them.

                    // Check its top block.
                    if (y > 0 && output[x, y].Type == BlockType.RoomWall && output[x, y - 1].Type == BlockType.Tunnel) {
                        // Make sure left and right side are not empty. If so, don't make the connection.
                        if (output[x - 1, y].Type != BlockType.Empty && output[x + 1, y].Type != BlockType.Empty) {
                            output[x, y].Type = BlockType.RoomConnector;
                        }
                    }

                    // Check its left block.
                    if (x > 0 && output[x, y].Type == BlockType.RoomWall && output[x - 1, y].Type == BlockType.Tunnel) {
                        // Make sure top and bottom side are not empty. If so, don't make the connection.
                        if (output[x, y - 1].Type != BlockType.Empty && output[x, y + 1].Type != BlockType.Empty) {
                            output[x, y].Type = BlockType.RoomConnector;
                        }
                    }

                    // Check for its right block.
                    if (x < grid.Width - 1 && output[x, y].Type == BlockType.RoomWall && output[x + 1, y].Type == BlockType.Tunnel) {
                        if (output[x, y - 1].Type != BlockType.Empty && output[x, y + 1].Type != BlockType.Empty) {
                            output[x, y].Type = BlockType.RoomConnector;
                        }
                    }

                    // Check its bottom block.
                    if (y < grid.Height - 1 && output[x, y].Type == BlockType.RoomWall && output[x, y + 1].Type == BlockType.Tunnel) {
                        // Make sure left and right side are not empty. If so, don't make the connection.
                        if (output[x - 1, y].Type != BlockType.Empty && output[x + 1, y].Type != BlockType.Empty) {
                            output[x, y].Type = BlockType.RoomConnector;
                        }
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
        public static GridLayerAsync MergeGrid(GridLayerAsync firstGrid, GridLayerAsync secondGrid) {
            // Check for their size.
            if (firstGrid.Width != secondGrid.Width || firstGrid.Height != secondGrid.Height) {
                throw new InvalidLayerSizeException(secondGrid.Width, secondGrid.Height, 
                    firstGrid.Width, firstGrid.Height);
            }

            // Create new grid with first grid as reference.
            GridLayerAsync output = firstGrid;

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

            Tunnels.ForEach(obj => {
                obj.AnglePoint.ForEach(obj2 => highestPoint.X = highestPoint.X < obj2.X ? obj2.X : highestPoint.X);
                obj.AnglePoint.ForEach(obj2 => highestPoint.Y = highestPoint.Y < obj2.Y ? obj2.Y : highestPoint.Y);
            });

            Cells.ForEach(obj => highestPoint.X = highestPoint.X < (obj.Location + obj.Size).X ? (obj.Location + obj.Size).X : highestPoint.X);
            Cells.ForEach(obj => highestPoint.Y = highestPoint.Y < (obj.Location + obj.Size).Y ? (obj.Location + obj.Size).Y : highestPoint.Y);

            // Add some space.
            // TODO: For some reason, certain parts on the very right bottom would get cut off.
            // Apparently it has something to do with space. So, this is here is a temp fix.
            //highestPoint += increment;
            highestPoint += new Point(200, 200);

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
