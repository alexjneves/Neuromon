using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Common;
using Common.Turn;

namespace Player.AI.Intelligent
{
    internal sealed class IntelligentAiPlayerController : IPlayerController
    {
        private const double AttackProbability = 0.8;
        private const double SwitchActiveNeuromonProbability = 0.2;

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

            var allMovesAreCurrentlyValid = _supportedTurnTypes.All(t => validTurns.Contains(t));

            if (allMovesAreCurrentlyValid)
            {
                turnType = _turnTypeRouletteWheel.Spin();
            }

            switch (turnType)
            {
                case TurnType.Attack:
                    return ChooseAttack(playerState);
                case TurnType.SwitchActiveNeuromon:
                    return new SwitchActiveNeuromon(ChooseNeuromon(playerState, opponentState));
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

            var rankedOrderedChoices = aliveNeuromon.ToDictionary(
                neuromon => neuromon, 
                neuromon => RankNeuromonChoice(neuromon, opponentState.ActiveNeuromon)
            ).OrderByDescending(choice => choice.Value);

            return rankedOrderedChoices.First().Key;
        }

        private static int RankNeuromonChoice(Neuromon neuromon, Neuromon opponentNeuromon)
        {
            if (neuromon.Type.IsEffectiveAgainst(opponentNeuromon.Type))
            {
                return 2;
            }

            if (!opponentNeuromon.Type.IsEffectiveAgainst(neuromon.Type))
            {
                return 1;
            }

            return 0;
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
