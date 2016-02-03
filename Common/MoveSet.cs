using System;
using System.Collections.Generic;

namespace Common
{
    public sealed class MoveSet
    {
        private const int NumberOfMovesInMoveSet = 4;

        public IList<Move> Moves { get; }

        public MoveSet(IList<Move> moves)
        {
            if (moves.Count != NumberOfMovesInMoveSet)
            {
                throw new Exception();
            }

            Moves = moves;
        }

        public Move this[int key] => Moves[key];

        public Move MoveOne() => Moves[0];
        public Move MoveTwo() => Moves[1];
        public Move MoveThree() => Moves[2];
        public Move MoveFour() => Moves[3];
    }
}