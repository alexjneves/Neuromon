namespace Common.Turn
{
    public sealed class Attack : ITurn
    {
        public Move Move { get; }

        public Attack(Move move)
        {
            Move = move;
        }
    }
}