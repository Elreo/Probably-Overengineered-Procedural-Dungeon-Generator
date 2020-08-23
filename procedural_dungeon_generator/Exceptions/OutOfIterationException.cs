using System;
using System.Collections.Generic;
using System.Text;

namespace procedural_dungeon_generator.Exceptions {
    public class OutOfIterationException : Exception {
        public int IterationNumber { get; }

        public OutOfIterationException(int iterationNumber) : 
            base("Iteration exceeded limit. Minimum iteration is " + iterationNumber + 
                ". Please retry with the different dataset.") { IterationNumber = iterationNumber; }
    }
}
