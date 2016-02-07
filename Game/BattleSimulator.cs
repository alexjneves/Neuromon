using System;
using System.Linq;
using System.Threading;
using Common.Turn;
using Game.Damage;
using Player;
using Player.Human;
using static Game.BattleDelegates;

namespace Game
{
    internal sealed class BattleSimulator
    {
        private const int ThinkingSeconds = 3;

        private readonly IDamageCalculator _damageCalculator;
        private readonly bool _simulateThinking;

        public GameState GameState { get; private set; }
        public IPlayer Player1 { get; }
        public IPlayer Player2 { get; }

        public event AttackMadeDelegate OnAttackMade;
        public event NeuromonChangedDelegate OnNeuromonChanged;
        public event GameOverDelegate OnGameOver;
        public event GameStateChangedDelegate OnGameStateChanged;
        public event NeuromonDefeatedDelegate OnNeuromonDefeated;

        public BattleSimulator(IPlayer player1, IPlayer player2, IDamageCalculator damageCalculator, bool simulateThinking)
        {
            GameState = GameState.NotStarted;

            Player1 = player1;
            Player2 = player2;
            _damageCalculator = damageCalculator;
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
                Attack(source, target, attack);
            }
            else if (sourceTurn is ChangeNeuromon)
            {
                var changeNeuromon = sourceTurn as ChangeNeuromon;
                ChangeNeuromon(source, changeNeuromon);
            }
        }

        private void Attack(IPlayer attackingPlayer, IPlayer targetPlayer, Attack attack)
        {
            var damage = _damageCalculator.CalculateDamage(attack.Move, targetPlayer.ActiveNeuromon);
            targetPlayer.ActiveNeuromon.TakeDamage(damage);

            OnAttackMade?.Invoke(attackingPlayer.ActiveNeuromon, attack.Move, targetPlayer.ActiveNeuromon, damage);
        }

        private void ChangeNeuromon(IPlayer player, ChangeNeuromon changeNeuromon)
        {
            var previousNeuromon = player.ActiveNeuromon;
            player.ActiveNeuromon = changeNeuromon.Neuromon;

            OnNeuromonChanged?.Invoke(player, previousNeuromon, changeNeuromon.Neuromon);
        }

        private void GameOver(IPlayer winner, IPlayer loser)
        {
            ChangeState(GameState.GameOver);

            OnGameOver?.Invoke(winner, loser);
        }
    }
}