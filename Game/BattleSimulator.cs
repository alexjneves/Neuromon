using System.Linq;
using Common.Turn;
using Game.Damage;
using Player;
using static Game.BattleDelegates;

namespace Game
{
    public sealed class BattleSimulator
    {
        private readonly IDamageCalculator _damageCalculator;

        private BattleResult _battleResult;

        public GameState GameState { get; private set; }
        public IPlayer Player1 { get; }
        public IPlayer Player2 { get; }

        public event AttackMadeDelegate OnAttackMade;
        public event NeuromonChangedDelegate OnNeuromonChanged;
        public event GameOverDelegate OnGameOver;
        public event GameStateChangedDelegate OnGameStateChanged;
        public event NeuromonDefeatedDelegate OnNeuromonDefeated;

        public BattleSimulator(IPlayer player1, IPlayer player2, IDamageCalculator damageCalculator)
        {
            GameState = GameState.NotStarted;

            Player1 = player1;
            Player2 = player2;
            _damageCalculator = damageCalculator;
        }

        public BattleResult Run()
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

            return _battleResult;
        }

        private void SimulateTurn(IPlayer sourcePlayer, IPlayer opponentPlayer)
        {
            ChooseTurn(sourcePlayer, opponentPlayer.State);

            if (opponentPlayer.State.AllNeuromon.All(n => n.IsDead))
            {
                GameOver(new BattleResult(sourcePlayer.State, opponentPlayer.State));
            }
            else if (opponentPlayer.State.ActiveNeuromon.IsDead)
            {
                OnNeuromonDefeated?.Invoke(sourcePlayer.State, sourcePlayer.State.ActiveNeuromon, opponentPlayer.State, opponentPlayer.State.ActiveNeuromon);

                var deadNeuromon = opponentPlayer.State.ActiveNeuromon;
                var newActiveNeuromon = opponentPlayer.Controller.SelectActiveNeuromon(opponentPlayer.State, sourcePlayer.State);

                opponentPlayer.State.SwitchActiveNeuromon(newActiveNeuromon);

                OnNeuromonChanged?.Invoke(opponentPlayer.State, deadNeuromon, opponentPlayer.State.ActiveNeuromon);
            }
        }

        private void ChangeState(GameState newState)
        {
            var previousState = GameState;
            GameState = newState;

            OnGameStateChanged?.Invoke(previousState, GameState);
        }

        private void ChooseTurn(IPlayer source, IPlayerState opponentState)
        {
            var sourceTurn = source.Controller.ChooseTurn(source.State, opponentState);

            if (sourceTurn is Attack)
            {
                var attack = sourceTurn as Attack;
                Attack(source.State, opponentState, attack);
            }
            else if (sourceTurn is ChangeNeuromon)
            {
                var changeNeuromon = sourceTurn as ChangeNeuromon;
                ChangeNeuromon(source.State, changeNeuromon);
            }
        }

        private void Attack(IPlayerState attackingPlayerState, IPlayerState targetPlayerState, Attack attack)
        {
            var damage = _damageCalculator.CalculateDamage(attack.Move, targetPlayerState.ActiveNeuromon);
            targetPlayerState.ActiveNeuromon.TakeDamage(damage);

            OnAttackMade?.Invoke(attackingPlayerState.ActiveNeuromon, attack.Move, targetPlayerState.ActiveNeuromon, damage);
        }

        private void ChangeNeuromon(IPlayerState playerState, ChangeNeuromon changeNeuromon)
        {
            var previousNeuromon = playerState.ActiveNeuromon;
            playerState.SwitchActiveNeuromon(changeNeuromon.Neuromon);

            OnNeuromonChanged?.Invoke(playerState, previousNeuromon, changeNeuromon.Neuromon);
        }

        private void GameOver(BattleResult battleResult)
        {
            _battleResult = battleResult;
            ChangeState(GameState.GameOver);

            OnGameOver?.Invoke(_battleResult);
        }
    }
}