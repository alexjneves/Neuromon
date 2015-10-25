using System;
using System.Collections.Generic;
using System.Linq;

namespace Neuromon
{
    internal sealed class MoveSet
    {
        private const int NumberOfMovesInMoveSet = 4;

        private readonly IList<Move> _moves; 

        public MoveSet(IList<Move> moves)
        {
            if (moves.Count() != NumberOfMovesInMoveSet)
            {
                throw new Exception();
            }

            _moves = moves;
        }

        public Move MoveOne() => _moves[0];
        public Move MoveTwo() => _moves[1];
        public Move MoveThree() => _moves[2];
        public Move MoveFour() => _moves[3];
    }
}