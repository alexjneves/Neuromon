namespace Neuromon
{
    internal sealed class BattleDelegates
    {
        public delegate void TurnChosenDelegate(IPlayer player, Turn turn);

        public delegate void GameOverDelegate(IPlayer winner, IPlayer loser);

        public delegate void GameStateChangedDelegate(GameState previousState, GameState newState);
    }
}