using procedural_dungeon_generator.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace procedural_dungeon_generator.Components {
    /// <summary>
    /// This class is used to contain data that is produced
    /// by the grid.
    /// </summary>
    public class GridLayer {
        //Block[][] blocks;
        public Block[,] Blocks { get; }
        public int Width { get; }
        public int Height { get; }

        public GridLayer(int width, int height) {
            Width = width;
            Height = height;

            Blocks = new Block[width, height];
            for (int w = 0; w < width; w++) {
                for (int h = 0; h < height; h++) {
                    Blocks[w, h] = new Block(w, h, BlockType.Empty);
                }
            }

        }

    }
}
