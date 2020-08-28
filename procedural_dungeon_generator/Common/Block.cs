﻿using System;
using System.Collections.Generic;
using System.Text;

namespace procedural_dungeon_generator.Common {
    /// <summary>
    /// This is used to identify what kind of block it is.
    /// </summary>
    public enum BlockType {
        Room, Tunnel, Empty
    }

    /// <summary>
    /// A block is a spot located in a grid.
    /// </summary>
    public class Block {
        public BlockType Type { get; }
        public int X { get; }
        public int Y { get; }

        public Block(int x, int y, BlockType type) {
            Type = type;
            X = x;
            Y = y;
        }

        public override int GetHashCode() {
            return X ^ Y ^ Type.GetHashCode();
        }
    }
}
