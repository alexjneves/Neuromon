using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Turn;

namespace Player.AI.Intelligent
{
    internal sealed class IntelligentAiPlayerController : IPlayerController
    {
        private const int EffectiveRank = 2;
        private const int NeutralRank = 1;
        private const int WeakRank = 0;

        private const double AttackProbability = 0.2;
        private const double SwitchActiveNeuromonProbability = 0.8;

        private readonly Random _rand;
        private readonly Dictionary<Neuromon, RouletteWheel<Move>> _neuromonRouletteWheels;
        private readonly RouletteWheel<TurnType> _turnTypeRouletteWheel;
        private readonly TurnType[] _supportedTurnTypes;

        public IntelligentAiPlayerController(IPlayerState initialState)
        {
            _rand = new Random();
            _neuromonRouletteWheels = new Dictionary<Neuromon, RouletteWheel<Move>>();

            foreach (var neuromon in initialState.AllNeuromon)
            {
                var rouletteWheel = CreateMoveRouletteWheel(neuromon);
                _neuromonRouletteWheels.Add(neuromon, rouletteWheel);
            }

            _turnTypeRouletteWheel = CreateTurnTypeRouletteWheel();

            _supportedTurnTypes = new[] { TurnType.Attack, TurnType.SwitchActiveNeuromon };
        }

        public ITurn ChooseTurn(IPlayerState playerState, IPlayerState opponentState)
        {
            var validTurns = DetermineValidTurnTypes(playerState);

            var turnType = TurnType.Attack;

            var allTurnsAreCurrentlyValid = _supportedTurnTypes.All(t => validTurns.Contains(t));

            var activeNeuromonRank = RankNeuromonChoice(playerState.ActiveNeuromon, opponentState.ActiveNeuromon);
            var inactiveNeuromonRanks = RankNeuromon(playerState.InactiveNeuromon.Where(n => !n.IsDead), opponentState.ActiveNeuromon);

            var betterActiveNeuromon =
                inactiveNeuromonRanks.Where(choice => choice.Value > activeNeuromonRank)
                    .OrderByDescending(choice => choice.Value)
                    .ToList();

            if (allTurnsAreCurrentlyValid && betterActiveNeuromon.Any())
            {
                turnType = _turnTypeRouletteWheel.Spin();
            }

            switch (turnType)
            {
                case TurnType.Attack:
                    return ChooseAttack(playerState);
                case TurnType.SwitchActiveNeuromon:
                    return new SwitchActiveNeuromon(betterActiveNeuromon.First().Key);
                default:
                    throw new Exception($"Unsupported turn type: {turnType}");
            }
        }

        private Attack ChooseAttack(IPlayerState playerState)
        {
            RouletteWheel<Move> rouletteWheel;
            if (!_neuromonRouletteWheels.TryGetValue(playerState.ActiveNeuromon, out rouletteWheel))
            {
                throw new Exception($"Roulette Wheel does not exist for Neuromon {playerState.ActiveNeuromon.Name}");
            }

            var move = rouletteWheel.Spin();

            return new Attack(move);
        }

        private static Neuromon ChooseNeuromon(IPlayerState playerState, IPlayerState opponentState)
        {
            var aliveNeuromon = playerState.InactiveNeuromon.Where(n => !n.IsDead).ToList();

            if (aliveNeuromon.Count == 1)
            {
                return aliveNeuromon.Single();
            }

            var rankedOrderedChoices = RankNeuromon(aliveNeuromon, opponentState.ActiveNeuromon)
                .OrderByDescending(choice => choice.Value);

            return rankedOrderedChoices.First().Key;
        }

        private static Dictionary<Neuromon, int> RankNeuromon(IEnumerable<Neuromon> neuromon, Neuromon opponentNeuromon)
        {
            return neuromon.ToDictionary(
                n => n,
                n => RankNeuromonChoice(n, opponentNeuromon)
            );
        }

        private static int RankNeuromonChoice(Neuromon neuromon, Neuromon opponentNeuromon)
        {
            if (neuromon.Type.IsEffectiveAgainst(opponentNeuromon.Type))
            {
                return EffectiveRank;
            }

            if (!opponentNeuromon.Type.IsEffectiveAgainst(neuromon.Type))
            {
                return NeutralRank;
            }

            return WeakRank;
        } 

        public Neuromon SelectActiveNeuromon(IPlayerState playerState, IPlayerState opponentState)
        {
            return ChooseNeuromon(playerState, opponentState);
        }

        private static RouletteWheel<Move> CreateMoveRouletteWheel(Neuromon neuromon)
        {
            var fitnessProportionateProbabilityCalculator = new FitnessProportionateProbabilityCalculator<Move>(neuromon.MoveSet.Moves);

            var selectionProbabilities = fitnessProportionateProbabilityCalculator.Calculate(m => m.Damage * 1.0);

            return new RouletteWheel<Move>(selectionProbabilities);
        }

        private static RouletteWheel<TurnType> CreateTurnTypeRouletteWheel()
        {
            var turnTypeProbabilities = new FitnessProportionateProbabilityObjectCollection<TurnType>(new List<FitnessProportionateProbabilityObject<TurnType>>
            {
                new FitnessProportionateProbabilityObject<TurnType>(TurnType.Attack, AttackProbability),
                new FitnessProportionateProbabilityObject<TurnType>(TurnType.SwitchActiveNeuromon, SwitchActiveNeuromonProbability)
            });

            return new RouletteWheel<TurnType>(turnTypeProbabilities);
        }

        private static IEnumerable<TurnType> DetermineValidTurnTypes(IPlayerState playerState)
        {
            if (playerState.ActiveNeuromon.IsDead)
            {
                return new[] { TurnType.SwitchActiveNeuromon };
            }

            var validMoves = new List<TurnType> { TurnType.Attack };

            if (playerState.InactiveNeuromon.Any(n => !n.IsDead))
            {
                validMoves.Add(TurnType.SwitchActiveNeuromon);
            }

            return validMoves.ToArray();
        }
    }
}
