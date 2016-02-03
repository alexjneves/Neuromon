using Common;
using Player;
using static Game.BattleDelegates;

namespace Game
{
    internal sealed class BattleSimulator
    {
        public GameState GameState { get; private set; }

        public IPlayer Player1 { get; }
        public IPlayer Player2 { get; }

        public event TurnChosenDelegate OnTurnChosen;
        public event GameOverDelegate OnGameOver;
        public event GameStateChangedDelegate OnGameStateChanged;

        public BattleSimulator(IPlayer player1, IPlayer player2)
        {
            GameState = GameState.NotStarted;

            Player1 = player1;
            Player2 = player2;
        }

        public void Run()
        {
            while (true)
            {
                ChangeState(GameState.Player1Turn);
                TakeTurn(Player1, Player2);

                if (Player2.ActiveNeuromon.IsDead())
                {
                    GameOver(Player1, Player2);
                    break;
                }

                ChangeState(GameState.Player2Turn);
                TakeTurn(Player2, Player1);

                if (Player1.ActiveNeuromon.IsDead())
                {
                    GameOver(Player2, Player1);
                    break;
                }
            }
        }

        private void ChangeState(GameState newState)
        {
            var previousState = GameState;
            GameState = newState;

            OnGameStateChanged?.Invoke(previousState, GameState);
        }

        private void TakeTurn(IPlayer source, IPlayer target)
        {
            var sourceTurn = source.ChooseTurn();
            target.ActiveNeuromon.TakeDamage(sourceTurn.Move.Damage);

            OnTurnChosen?.Invoke(source, sourceTurn);
        }

        private void GameOver(IPlayer winner, IPlayer loser)
        {
            ChangeState(GameState.GameOver);

            OnGameOver?.Invoke(winner, loser);
        }
    }
}