using System;
using System.Collections.Generic;
using System.Text;

namespace procedural_dungeon_generator.Exceptions {
    public class InvalidLayerSizeException : Exception {
        public int GivenWidth { get; }
        public int GivenHeight { get; }
        public int ExpectedWidth { get; }
        public int ExpectedHeight { get; }

        public InvalidLayerSizeException(int givenw, int givenh, int expw, int exph) : 
            base($"Grid layer size mismatch. The given layer is {givenw} / {givenh}, but the " +
                $"expected layer is {expw} / {exph}") {
            GivenWidth = givenw;
            GivenHeight = givenh;
            ExpectedWidth = expw;
            ExpectedHeight = exph;
        }

    }
}
