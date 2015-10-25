namespace Neuromon
{
    internal sealed class Turn
    {
        public Move Move { get; }

        public Turn(Move move)
        {
            Move = move;
        }
    }
}