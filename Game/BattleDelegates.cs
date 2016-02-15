using Common;
using Player;

namespace Game
{
    public sealed class BattleDelegates
    {
        public delegate void AttackMadeDelegate(Neuromon attacker, Move move, Neuromon target, int damage);

        public delegate void NeuromonChangedDelegate(IPlayerState playerState, Neuromon previousNeuromon, Neuromon newNeuromon);

        public delegate void GameOverDelegate(BattleResult battleResult);

        public delegate void GameStateChangedDelegate(GameState previousState, GameState newState);

        public delegate void NeuromonDefeatedDelegate(IPlayerState attackingPlayerState, Neuromon attacker, IPlayerState defendingPlayerState, Neuromon defeated);
    }
}