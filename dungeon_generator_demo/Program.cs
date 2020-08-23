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
        static void Main(string[] args) {
            Console.WriteLine("Initializing ...");

            CellGenerator cellGenerator = new CellGenerator();

            Console.WriteLine("Generating dungeons with 150 rooms ...");

            // Step 1
            List<Cell> cells = cellGenerator.GenerateCellList(150, 50, 250, 750);

            // Draw first step
            Console.WriteLine("Drawing first step to image ...");

            // Generate image with background
            Image image = new Bitmap(5000, 5000);
            Graphics graph = Graphics.FromImage(image);
            graph.Clear(Color.White);

            // Ready pen and draw the cells
            Pen pen = new Pen(Brushes.Black);

            //foreach (Cell cell in cells) {
            foreach (Cell cell in cells) {
                DrawCube(ref graph, ref pen, cell);
            }

            Console.WriteLine("Image drawn. Saving ...");

            if (File.Exists("step1.png")) {
                File.Delete("step1.png");
                Console.WriteLine("Previous save file has been deleted.");
            }

            image.Save("step1.png", System.Drawing.Imaging.ImageFormat.Png);

            Console.WriteLine("Image has been saved as \"step1.png\"");

            // Step 2
            //List<Cell> rearrangedCells = CellDistributor.FlockingSeparation(cells, cells.Count * 2);
            List<Cell> rearrangedCells;

            while (true) {
                try {
                    rearrangedCells = CellDistributor.FlockingSeparation(cells, cells.Count * 2);
                    break;
                } catch (OutOfIterationException exception) {
                    Console.WriteLine("WARNING: Separation iteration has been exhausted. " +
                        "Iteration limit is " + exception.IterationNumber + ". Retrying with new dataset ...\n");
                    cells = cellGenerator.GenerateCellList(150, 50, 250, 750);
                }
            }

            // Draw second step
            Console.WriteLine("Drawing second step to image ...");

            // Generate image with background
            image = new Bitmap(5000, 5000);
            graph = Graphics.FromImage(image);
            graph.Clear(Color.White);

            //foreach (Cell cell in cells) {
            foreach (Cell cell in rearrangedCells) {
                DrawCube(ref graph, ref pen, cell);
            }

            Console.WriteLine("Image drawn. Saving ...");

            if (File.Exists("step2.png")) {
                File.Delete("step2.png");
                Console.WriteLine("Previous save file has been deleted.");
            }

            image.Save("step2.png", System.Drawing.Imaging.ImageFormat.Png);

            Console.WriteLine("Image has been saved as \"step2.png\"");
        }

        static void DrawCube(ref Graphics graph, ref Pen pen, Cell cell) {
            // TODO: There must be a better way to draw this.
            graph.DrawLines(pen,
                new Point[] {
                    new Point(cell.Location.X + 1250 - cell.Center.X, cell.Location.Y + 1250 - cell.Center.Y),
                    new Point(cell.Location.X + 1250 - cell.Center.X, cell.Location.Y + 1250 - cell.Center.Y + cell.Size.Y)
                });
            graph.DrawLines(pen,
                new Point[] {
                    new Point(cell.Location.X + 1250 - cell.Center.X, cell.Location.Y + 1250 - cell.Center.Y),
                    new Point(cell.Location.X + 1250 - cell.Center.X + cell.Size.X, cell.Location.Y + 1250 - cell.Center.Y)
                });
            graph.DrawLines(pen,
                new Point[] {
                    new Point(cell.Location.X + 1250 - cell.Center.X + cell.Size.X, cell.Location.Y + 1250 - cell.Center.Y),
                    new Point(cell.Location.X + 1250 - cell.Center.X + cell.Size.X, cell.Location.Y + 1250 - cell.Center.Y + cell.Size.Y)
                });
            graph.DrawLines(pen,
                new Point[] {
                    new Point(cell.Location.X + 1250 - cell.Center.X, cell.Location.Y + 1250 - cell.Center.Y + cell.Size.Y),
                    new Point(cell.Location.X + 1250 - cell.Center.X + cell.Size.X, cell.Location.Y + 1250 - cell.Center.Y + cell.Size.Y)
                });
        }
    }
}
