using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

using procedural_dungeon_generator.Generators;
using procedural_dungeon_generator.Components;
using procedural_dungeon_generator.Adjusters;
using procedural_dungeon_generator.Exceptions;
using System.Linq;

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
             *  STEP 4.1
             * =========================================================================
             */
            TunnelGenerator tunnelGenerator = new TunnelGenerator(selectedCells);
            tunnelGenerator.DelaunayTriangulation();
            List<Cell> triangulatedCells = tunnelGenerator.ExportCells();

            // Draw step
            Console.WriteLine("Drawing fourth step phase one to image ...");

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

            foreach (Cell cell in triangulatedCells) {
                var foundCells = triangulatedCells.Where(o => cell != o && cell.ConnectedCell.Contains(o.GetHashCode()));
                foreach (Cell foundCell in foundCells) {
                    DrawLineFromCells(ref graph, ref pen, cell, foundCell, canvasSizeX, canvasSizeY);
                }
            }

            Console.WriteLine("Image drawn. Saving ...");

            if (File.Exists("step4.1.png")) {
                File.Delete("step4.1.png");
                Console.WriteLine("Previous save file has been deleted.");
            }

            image.Save("step4.1.png", System.Drawing.Imaging.ImageFormat.Png);

            Console.WriteLine("Image has been saved as \"step4.1.png\"");

            /**
             * =========================================================================
             *  STEP 4.2
             * =========================================================================
             */
            tunnelGenerator.RemoveOverlapping();
            triangulatedCells = tunnelGenerator.ExportCells();

            // Draw step
            Console.WriteLine("Drawing fourth step phase two to image ...");

            // Generate image with background
            image = new Bitmap(canvasSizeX, canvasSizeY);
            graph = Graphics.FromImage(image);
            graph.Clear(Color.White);

            pen = new Pen(Brushes.Black);

            // Draw the boxes.
            foreach (Cell cell in selectedCells) {
                DrawCube(ref graph, ref pen, cell, image.Width, image.Height);
            }

            // Change pen color to difference the lines.
            pen = new Pen(Brushes.Red);

            foreach (Cell cell in triangulatedCells) {
                var foundCells = triangulatedCells.Where(o => cell != o && cell.ConnectedCell.Contains(o.GetHashCode()));
                foreach (Cell foundCell in foundCells) {
                    DrawLineFromCells(ref graph, ref pen, cell, foundCell, canvasSizeX, canvasSizeY);
                }
            }

            Console.WriteLine("Image drawn. Saving ...");

            if (File.Exists("step4.2.png")) {
                File.Delete("step4.2.png");
                Console.WriteLine("Previous save file has been deleted.");
            }

            image.Save("step4.2.png", System.Drawing.Imaging.ImageFormat.Png);

            Console.WriteLine("Image has been saved as \"step4.2.png\"");

            /**
             * =========================================================================
             *  STEP 4.5
             * =========================================================================
             */
            //tunnelGenerator.RemoveOverlapping();
            //triangulatedCells = tunnelGenerator.ExportCells();
            List<Tunnel> tunnels = tunnelGenerator.GenerateTunnel();


            // Draw step
            Console.WriteLine("Drawing fourth step phase five to image ...");

            // Generate image with background
            image = new Bitmap(canvasSizeX, canvasSizeY);
            graph = Graphics.FromImage(image);
            graph.Clear(Color.White);

            pen = new Pen(Brushes.Black);

            // Draw the boxes.
            foreach (Cell cell in selectedCells) {
                DrawCube(ref graph, ref pen, cell, image.Width, image.Height);
            }

            // Change pen color to difference the lines.
            pen = new Pen(Brushes.Blue);

            // TODO: It sometimes break for some reason.
            foreach (Tunnel tunnel in tunnels) {
                var a = cells.Single(cell => cell.GetHashCode() == tunnel.CellHashA).LocationCenter;
                var b = cells.Single(cell => cell.GetHashCode() == tunnel.CellHashB).LocationCenter;
                DrawLineFromPoints(ref graph, ref pen, a, tunnel.AnglePoint.First(), canvasSizeX, canvasSizeY);
                DrawLineFromPoints(ref graph, ref pen, b, tunnel.AnglePoint.Last(), canvasSizeX, canvasSizeY);
            }

            Console.WriteLine("Image drawn. Saving ...");

            if (File.Exists("step4.5.png")) {
                File.Delete("step4.5.png");
                Console.WriteLine("Previous save file has been deleted.");
            }

            image.Save("step4.5.png", System.Drawing.Imaging.ImageFormat.Png);

            Console.WriteLine("Image has been saved as \"step4.5.png\"");

            Console.WriteLine("\n\nDebug Log:\n");
            triangulatedCells.ForEach(o => Console.WriteLine($"{o}"));
        }

        static void DrawLineFromCells(ref Graphics graph, ref Pen pen, Cell a, Cell b, int canvasSizeX, int canvasSizeY) {
            //graph.DrawLines(pen, new Point[] { 
            //    new Point(a.LocationCenter.X + (canvasSizeX / 2) - a.Center.X, a.LocationCenter.Y + (canvasSizeY / 2) - a.Center.Y), 
            //    new Point(b.LocationCenter.X + (canvasSizeX / 2) - b.Center.X, b.LocationCenter.Y + (canvasSizeY / 2) - b.Center.Y) 
            //});

            graph.DrawLines(pen, new Point[] {
                new Point(a.LocationCenter.X + (canvasSizeX / 2), a.LocationCenter.Y + (canvasSizeY / 2)),
                new Point(b.LocationCenter.X + (canvasSizeX / 2), b.LocationCenter.Y + (canvasSizeY / 2))
            });
        }

        static void DrawLineFromPoints(ref Graphics graph, ref Pen pen, procedural_dungeon_generator.Common.Point a, 
                procedural_dungeon_generator.Common.Point b, int canvasSizeX, int canvasSizeY) {
            graph.DrawLines(pen, new Point[] {
                new Point(a.X + (canvasSizeX / 2), a.Y + (canvasSizeX / 2)),
                new Point(b.X + (canvasSizeX / 2), b.Y + (canvasSizeX / 2))
            });
        }

        static Rectangle ToRectangle(Cell cell, int canvasSizeX, int canvasSizeY) {
            return new Rectangle(cell.Location.X + (canvasSizeX / 2), cell.Location.Y + (canvasSizeY / 2), cell.Size.X, cell.Size.Y);
        }

        static void DrawTextInRectangle(ref Graphics graph, string text, Rectangle rect) {
            graph.DrawString(text, new Font("Tahoma", 8), Brushes.Black, rect);
        }

        static void DrawCube(ref Graphics graph, ref Pen pen, Cell cell, int canvasSizeX, int canvasSizeY) {
            graph.DrawRectangle(pen, ToRectangle(cell, canvasSizeX, canvasSizeY));
            DrawTextInRectangle(ref graph, $"{{{ cell.Location.X }, { cell.Location.Y }}}\n" +
                $"{{{ cell.Location.X + cell.Size.Y }, {cell.Location.Y + cell.Size.Y}}}", 
                ToRectangle(cell, canvasSizeX, canvasSizeY));

            // There must be a better way to draw this.
            //graph.DrawLines(pen,
            //    new Point[] {
            //        new Point(cell.Location.X + (canvasSizeX / 2) - cell.Center.X, cell.Location.Y + (canvasSizeY / 2) - cell.Center.Y),
            //        new Point(cell.Location.X + (canvasSizeX / 2) - cell.Center.X, cell.Location.Y + (canvasSizeY / 2) - cell.Center.Y + cell.Size.Y)
            //    });
            //graph.DrawLines(pen,
            //    new Point[] {
            //        new Point(cell.Location.X + (canvasSizeX / 2) - cell.Center.X, cell.Location.Y + (canvasSizeY / 2) - cell.Center.Y),
            //        new Point(cell.Location.X + (canvasSizeX / 2) - cell.Center.X + cell.Size.X, cell.Location.Y + (canvasSizeY / 2) - cell.Center.Y)
            //    });
            //graph.DrawLines(pen,
            //    new Point[] {
            //        new Point(cell.Location.X + (canvasSizeX / 2) - cell.Center.X + cell.Size.X, cell.Location.Y + (canvasSizeY / 2) - cell.Center.Y),
            //        new Point(cell.Location.X + (canvasSizeX / 2) - cell.Center.X + cell.Size.X, cell.Location.Y + (canvasSizeY / 2) - cell.Center.Y + cell.Size.Y)
            //    });
            //graph.DrawLines(pen,
            //    new Point[] {
            //        new Point(cell.Location.X + (canvasSizeX / 2) - cell.Center.X, cell.Location.Y + (canvasSizeY / 2) - cell.Center.Y + cell.Size.Y),
            //        new Point(cell.Location.X + (canvasSizeX / 2) - cell.Center.X + cell.Size.X, cell.Location.Y + (canvasSizeY / 2) - cell.Center.Y + cell.Size.Y)
            //    });
        }
    }
}
