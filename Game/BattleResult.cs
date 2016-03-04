using Player;

namespace Game
{
    public sealed class BattleResult
    {
        public IPlayerState Winner { get; }
        public IPlayerState Loser { get; }

        public BattleResult(IPlayerState winner, IPlayerState loser)
        {
            Winner = winner;
            Loser = loser;
        }
    }
}