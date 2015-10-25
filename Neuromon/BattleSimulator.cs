using System;
using static Neuromon.BattleDelegates;

namespace Neuromon
{
    internal sealed class BattleSimulator
    {
        public GameState GameState { get; private set; }

        public IPlayer Player1 { get; }
        public IPlayer Player2 { get; }

        public event TurnChosenDelegate OnTurnChosen;
        public event GameOverDelegate OnGameOver;
        public event GameStateChangedDelegate OnGameStateChanged;

        public BattleSimulator()
        {
            GameState = GameState.NotStarted;

            Player1 = new HumanPlayer("Player 1");
            Player2 = new HumanPlayer("Player 2");
        }

        public void Run()
        {
            while (true)
            {
                ChangeState(GameState.Player1Turn);
                TakeTurn(Player1, Player2);

                if (Player2.Neuromon.IsDead())
                {
                    GameOver(Player1, Player2);
                    break;
                }

                ChangeState(GameState.Player2Turn);
                TakeTurn(Player2, Player1);

                if (Player1.Neuromon.IsDead())
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
            target.Neuromon.TakeDamage(sourceTurn.Move.Damage);

            OnTurnChosen?.Invoke(source, sourceTurn);
        }

        private void GameOver(IPlayer winner, IPlayer loser)
        {
            ChangeState(GameState.GameOver);

            OnGameOver?.Invoke(winner, loser);
        }
    }
}