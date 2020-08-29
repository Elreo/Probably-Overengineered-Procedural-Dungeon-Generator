using procedural_dungeon_generator.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace procedural_dungeon_generator.Components {
    /// <summary>
    /// A thread-safe version of GridLayer class.
    /// by the grid.
    /// </summary>
    public class GridLayerAsync {
        //Block[][] blocks;
        public Block[,] Blocks { get; }
        public int Width { get; }
        public int Height { get; }

        // Mutex to lock and unlock the blocks.
        private Mutex mut = new Mutex();

        public GridLayerAsync(int width, int height) {
            Width = width;
            Height = height;


            Blocks = new Block[width, height];
            for (int w = 0; w < width; w++) {
                for (int h = 0; h < height; h++) {
                    Blocks[w, h] = new Block(w, h, BlockType.Empty);
                }
            }

        }

        public Block this[int width, int height] {
            get {
                Block output;
                mut.WaitOne();
                output = Blocks[width, height];
                mut.ReleaseMutex();
                return output;
            }
            set {
                mut.WaitOne();
                Blocks[width, height] = value;
                mut.ReleaseMutex();
            }
        }
    }
}
