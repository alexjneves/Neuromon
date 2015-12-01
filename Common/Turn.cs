namespace Common
{
    public sealed class Turn
    {
        public Move Move { get; }

        public Turn(Move move)
        {
            Move = move;
        }
    }
}