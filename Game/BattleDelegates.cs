using Common;
using Player;

namespace Game
{
    internal sealed class BattleDelegates
    {
        public delegate void AttackMadeDelegate(Neuromon attacker, Move move, Neuromon target, int damage);

        public delegate void NeuromonChangedDelegate(IPlayer player, Neuromon previousNeuromon, Neuromon newNeuromon);

        public delegate void GameOverDelegate(IPlayer winner, IPlayer loser);

        public delegate void GameStateChangedDelegate(GameState previousState, GameState newState);

        public delegate void NeuromonDefeatedDelegate(IPlayer attackingPlayer, Neuromon attacker, IPlayer defendingPlayer, Neuromon defeated);
    }
}