using Common;
using Common.Turn;
using Player;
using static Game.BattleDelegates;

namespace Game
{
    internal sealed class BattleSimulator
    {
        public GameState GameState { get; private set; }

        public IPlayer Player1 { get; }
        public IPlayer Player2 { get; }

        public event AttackMadeDelegate OnAttackMade;
        public event NeuromonChangedDelegate OnNeuromonChanged;
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

            if (sourceTurn is Attack)
            {
                var attack = sourceTurn as Attack;
                Attack(attack.Move, target.ActiveNeuromon);
                OnAttackMade?.Invoke(source.ActiveNeuromon, attack.Move, target.ActiveNeuromon, attack.Move.Damage);
            }
            else if (sourceTurn is ChangeNeuromon)
            {
                var changeNeuromon = sourceTurn as ChangeNeuromon;
                var previousNeuromon = source.ActiveNeuromon;

                ChangeNeuromon(source, changeNeuromon.Neuromon);
                OnNeuromonChanged?.Invoke(source, previousNeuromon, changeNeuromon.Neuromon);
            }
        }

        private static void Attack(Move move, Neuromon target)
        {
            target.TakeDamage(move.Damage);
        }

        private static void ChangeNeuromon(IPlayer player, Neuromon neuromon)
        {
            player.ActiveNeuromon = neuromon;
        }

        private void GameOver(IPlayer winner, IPlayer loser)
        {
            ChangeState(GameState.GameOver);

            OnGameOver?.Invoke(winner, loser);
        }
    }
}