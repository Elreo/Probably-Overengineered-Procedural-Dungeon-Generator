using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

using procedural_dungeon_generator.Generators;
using procedural_dungeon_generator.Components;

namespace dungeon_generator_demo {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Initializing ...");

            CellGenerator cellGenerator = new CellGenerator();

            Console.WriteLine("Generating dungeons with 150 rooms ...");

            List<Cell> cells = cellGenerator.GenerateCellList(150, 50, 250, 750);

            Console.WriteLine("Drawing to image ...");

            // Generate image with background
            Image image = new Bitmap(2500, 2500);
            Graphics graph = Graphics.FromImage(image);
            graph.Clear(Color.Azure);

            // Ready pen and draw the cells
            Pen pen = new Pen(Brushes.Blue);

            foreach (Cell cell in cells) {
                DrawCube(ref graph, ref pen, cell);
            }

            Console.WriteLine("Image drawn. Saving ...");

            if (File.Exists("output.png")) {
                File.Delete("output.png");
                Console.WriteLine("Previous save file has been deleted.");
            }

            image.Save("output.png", System.Drawing.Imaging.ImageFormat.Png);

            Console.WriteLine("Image has been saved as \"output.png\"");
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
                    new Point(cell.Location.X + 1250 - cell.Center.X, cell.Location.Y + 1250 - cell.Center.X),
                    new Point(cell.Location.X + 1250 - cell.Center.X + cell.Size.X, cell.Location.Y + 1250 - cell.Center.X)
                });
            graph.DrawLines(pen,
                new Point[] {
                    new Point(cell.Location.X + 1250 - cell.Center.X + cell.Size.X, cell.Location.Y + 1250 - cell.Center.X),
                    new Point(cell.Location.X + 1250 - cell.Center.X + cell.Size.X, cell.Location.Y + 1250 - cell.Center.X + cell.Size.Y)
                });
            graph.DrawLines(pen,
                new Point[] {
                    new Point(cell.Location.X + 1250 - cell.Center.X, cell.Location.Y + 1250 - cell.Center.X + cell.Size.Y),
                    new Point(cell.Location.X + 1250 - cell.Center.X + cell.Size.X, cell.Location.Y + 1250 - cell.Center.X + cell.Size.Y)
                });
        }
    }
}
