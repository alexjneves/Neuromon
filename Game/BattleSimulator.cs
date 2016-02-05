using System;
using System.Linq;
using System.Threading;
using Common;
using Common.Turn;
using Player;
using Player.Human;
using static Game.BattleDelegates;

namespace Game
{
    internal sealed class BattleSimulator
    {
        private const int ThinkingSeconds = 3;

        private readonly bool _simulateThinking;

        public GameState GameState { get; private set; }
        public IPlayer Player1 { get; }
        public IPlayer Player2 { get; }

        public event AttackMadeDelegate OnAttackMade;
        public event NeuromonChangedDelegate OnNeuromonChanged;
        public event GameOverDelegate OnGameOver;
        public event GameStateChangedDelegate OnGameStateChanged;
        public event NeuromonDefeatedDelegate OnNeuromonDefeated;

        public BattleSimulator(IPlayer player1, IPlayer player2, bool simulateThinking)
        {
            GameState = GameState.NotStarted;

            Player1 = player1;
            Player2 = player2;
            _simulateThinking = simulateThinking;
        }

        public void Run()
        {
            while (GameState != GameState.GameOver)
            {
                ChangeState(GameState.Player1Turn);
                SimulateTurn(Player1, Player2);

                if (GameState == GameState.GameOver)
                {
                    break;
                }

                ChangeState(GameState.Player2Turn);
                SimulateTurn(Player2, Player1);
            }
        }

        private void SimulateTurn(IPlayer sourcePlayer, IPlayer targetPlayer)
        {
            if (_simulateThinking && !(sourcePlayer is HumanPlayer))
            {
                Thread.Sleep(TimeSpan.FromSeconds(ThinkingSeconds));
            }

            ChooseTurn(sourcePlayer, targetPlayer);

            if (targetPlayer.Neuromon.All(n => n.IsDead))
            {
                GameOver(sourcePlayer, targetPlayer);
            }
            else if (targetPlayer.ActiveNeuromon.IsDead)
            {
                OnNeuromonDefeated?.Invoke(sourcePlayer, sourcePlayer.ActiveNeuromon, targetPlayer, targetPlayer.ActiveNeuromon);

                var deadNeuromon = targetPlayer.ActiveNeuromon;
                targetPlayer.ActiveNeuromon = targetPlayer.SelectActiveNeuromon();

                if (targetPlayer.ActiveNeuromon.IsDead)
                {
                    throw new Exception("Cannot choose a dead Neuromon to be the active Neuromon");
                }

                OnNeuromonChanged?.Invoke(targetPlayer, deadNeuromon, targetPlayer.ActiveNeuromon);
            }
        }


        private void ChangeState(GameState newState)
        {
            var previousState = GameState;
            GameState = newState;

            OnGameStateChanged?.Invoke(previousState, GameState);
        }

        private void ChooseTurn(IPlayer source, IPlayer target)
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