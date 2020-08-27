using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

using procedural_dungeon_generator.Generators;
using procedural_dungeon_generator.Components;
using procedural_dungeon_generator.Adjusters;
using procedural_dungeon_generator.Exceptions;

namespace dungeon_generator_demo {
    class Program {
        private static int canvasSizeX = 5000;
        private static int canvasSizeY = 5000;

        static void Main(string[] args) {
            Console.WriteLine("Initializing ...");

            CellGenerator cellGenerator = new CellGenerator();

            Console.WriteLine("Generating dungeons with 150 rooms ...");

            /**
             * =========================================================================
             *  STEP 1
             * =========================================================================
             */
            List<Cell> cells = cellGenerator.GenerateCellList(150, 50, 250, 750, 1, 0.2);

            // Draw first step
            Console.WriteLine("Drawing first step to image ...");

            // Generate image with background
            Image image = new Bitmap(canvasSizeX, canvasSizeY);
            Graphics graph = Graphics.FromImage(image);
            graph.Clear(Color.White);

            // Ready pen and draw the cells
            Pen pen = new Pen(Brushes.Black);

            //foreach (Cell cell in cells) {
            foreach (Cell cell in cells) {
                DrawCube(ref graph, ref pen, cell, image.Width, image.Height);
            }

            Console.WriteLine("Image drawn. Saving ...");

            if (File.Exists("step1.png")) {
                File.Delete("step1.png");
                Console.WriteLine("Previous save file has been deleted.");
            }

            image.Save("step1.png", System.Drawing.Imaging.ImageFormat.Png);

            Console.WriteLine("Image has been saved as \"step1.png\"");

            /**
             * =========================================================================
             *  STEP 2
             * =========================================================================
             */
            //List<Cell> rearrangedCells = CellDistributor.FlockingSeparation(cells, cells.Count * 2);
            List<Cell> rearrangedCells;

            while (true) {
                try {
                    rearrangedCells = CellDistributor.FlockingSeparation(cells, cells.Count * 2);
                    break;
                } catch (OutOfIterationException exception) {
                    Console.WriteLine("WARNING: Separation iteration has been exhausted. " +
                        "Iteration limit is " + exception.IterationNumber + ". Retrying with new dataset ...\n");
                    //cells = cellGenerator.GenerateCellList(150, 50, 250, 750);
                    cells = cellGenerator.GenerateCellList(150, 50, 250, 750, 1, 0.2);
                }
            }

            // Draw second step
            Console.WriteLine("Drawing second step to image ...");

            // Generate image with background
            image = new Bitmap(canvasSizeX, canvasSizeY);
            graph = Graphics.FromImage(image);
            graph.Clear(Color.White);

            //foreach (Cell cell in cells) {
            foreach (Cell cell in rearrangedCells) {
                DrawCube(ref graph, ref pen, cell, image.Width, image.Height);
            }

            Console.WriteLine("Image drawn. Saving ...");

            if (File.Exists("step2.png")) {
                File.Delete("step2.png");
                Console.WriteLine("Previous save file has been deleted.");
            }

            image.Save("step2.png", System.Drawing.Imaging.ImageFormat.Png);

            Console.WriteLine("Image has been saved as \"step2.png\"");

            /**
             * =========================================================================
             *  STEP 3
             * =========================================================================
             */
            List<Cell> selectedCells = CellDistributor.TrimCells(rearrangedCells, 25);

            // Draw second step
            Console.WriteLine("Drawing third step to image ...");

            // Generate image with background
            image = new Bitmap(canvasSizeX, canvasSizeY);
            graph = Graphics.FromImage(image);
            graph.Clear(Color.White);

            //foreach (Cell cell in cells) {
            foreach (Cell cell in selectedCells) {
                DrawCube(ref graph, ref pen, cell, image.Width, image.Height);
            }

            Console.WriteLine("Image drawn. Saving ...");

            if (File.Exists("step3.png")) {
                File.Delete("step3.png");
                Console.WriteLine("Previous save file has been deleted.");
            }

            image.Save("step3.png", System.Drawing.Imaging.ImageFormat.Png);

            Console.WriteLine("Image has been saved as \"step3.png\"");

            /**
             * =========================================================================
             *  STEP 4
             * =========================================================================
             */
            //List<Cell> rearrangedCells = CellDistributor.FlockingSeparation(cells, cells.Count * 2);
            //List<Cell> selectedCells = CellDistributor.TrimCells(rearrangedCells, 25);
            TunnelGenerator tunnelGenerator = new TunnelGenerator(selectedCells);
            tunnelGenerator.TriangulatePaths();
            List<Cell> triangulatedCells = tunnelGenerator.ExportCells();

            // Draw fourth step
            Console.WriteLine("Drawing fourth step to image ...");

            // Generate image with background
            image = new Bitmap(canvasSizeX, canvasSizeY);
            graph = Graphics.FromImage(image);
            graph.Clear(Color.White);

            // Draw the boxes.
            foreach (Cell cell in selectedCells) {
                DrawCube(ref graph, ref pen, cell, image.Width, image.Height);
            }

            // Change pen color to difference the lines.
            pen = new Pen(Brushes.Red);

            // TODO: There has to be a better way to do this.
            foreach (Cell cell in triangulatedCells) {
                // Iterate the hash code.
                foreach (int hashcode in cell.ConnectedCell) {
                    // Get the cells that's connected to it.
                    foreach (Cell innerCell in triangulatedCells) {
                        // If found, draw it and break out of the loop.
                        if (hashcode == innerCell.GetHashCode()) {
                            DrawLineFromCells(ref graph, ref pen, cell, innerCell, canvasSizeX, canvasSizeY);
                            break;
                        }
                    }
                }
            }

            Console.WriteLine("Image drawn. Saving ...");

            if (File.Exists("step4.png")) {
                File.Delete("step4.png");
                Console.WriteLine("Previous save file has been deleted.");
            }

            image.Save("step4.png", System.Drawing.Imaging.ImageFormat.Png);

            Console.WriteLine("Image has been saved as \"step4.png\"");
        }

        static void DrawLineFromCells(ref Graphics graph, ref Pen pen, Cell cellA, Cell cellB, int canvasSizeX, int canvasSizeY) {
            graph.DrawLines(pen,
                new Point[] {
                    new Point(cellA.LocationCenter.X, cellA.LocationCenter.Y),
                    new Point(cellB.LocationCenter.X, cellB.LocationCenter.Y)
                });
        }

        static void DrawCube(ref Graphics graph, ref Pen pen, Cell cell, int canvasSizeX, int canvasSizeY) {
            // TODO: There must be a better way to draw this.
            graph.DrawLines(pen,
                new Point[] {
                    new Point(cell.Location.X + (canvasSizeX / 2) - cell.Center.X, cell.Location.Y + (canvasSizeY / 2) - cell.Center.Y),
                    new Point(cell.Location.X + (canvasSizeX / 2) - cell.Center.X, cell.Location.Y + (canvasSizeY / 2) - cell.Center.Y + cell.Size.Y)
                });
            graph.DrawLines(pen,
                new Point[] {
                    new Point(cell.Location.X + (canvasSizeX / 2) - cell.Center.X, cell.Location.Y + (canvasSizeY / 2) - cell.Center.Y),
                    new Point(cell.Location.X + (canvasSizeX / 2) - cell.Center.X + cell.Size.X, cell.Location.Y + (canvasSizeY / 2) - cell.Center.Y)
                });
            graph.DrawLines(pen,
                new Point[] {
                    new Point(cell.Location.X + (canvasSizeX / 2) - cell.Center.X + cell.Size.X, cell.Location.Y + (canvasSizeY / 2) - cell.Center.Y),
                    new Point(cell.Location.X + (canvasSizeX / 2) - cell.Center.X + cell.Size.X, cell.Location.Y + (canvasSizeY / 2) - cell.Center.Y + cell.Size.Y)
                });
            graph.DrawLines(pen,
                new Point[] {
                    new Point(cell.Location.X + (canvasSizeX / 2) - cell.Center.X, cell.Location.Y + (canvasSizeY / 2) - cell.Center.Y + cell.Size.Y),
                    new Point(cell.Location.X + (canvasSizeX / 2) - cell.Center.X + cell.Size.X, cell.Location.Y + (canvasSizeY / 2) - cell.Center.Y + cell.Size.Y)
                });
        }
    }
}
