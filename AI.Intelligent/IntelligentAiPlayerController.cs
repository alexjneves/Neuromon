using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Turn;

namespace Player.AI.Intelligent
{
    internal sealed class IntelligentAiPlayerController : IPlayerController
    {
        private readonly Random _rand;
        private readonly Dictionary<Neuromon, RouletteWheel<Move>> _neuromonRouletteWheels;

        public IntelligentAiPlayerController(IPlayerState initialState)
        {
            _rand = new Random();
            _neuromonRouletteWheels = new Dictionary<Neuromon, RouletteWheel<Move>>();

            foreach (var neuromon in initialState.AllNeuromon)
            {
                var rouletteWheel = CreateRouletteWheel(neuromon);
                _neuromonRouletteWheels.Add(neuromon, rouletteWheel);
            }
        }

        public ITurn ChooseTurn(IPlayerState playerState, IPlayerState opponentState)
        {
            RouletteWheel<Move> rouletteWheel;
            if (!_neuromonRouletteWheels.TryGetValue(playerState.ActiveNeuromon, out rouletteWheel))
            {
                throw new Exception($"Roulette Wheel does not exist for Neuromon {playerState.ActiveNeuromon.Name}");
            }

            var move = rouletteWheel.Spin();

            return new Attack(move);
        }

        public Neuromon SelectActiveNeuromon(IPlayerState playerState, IPlayerState opponentState)
        {
            var aliveNeuromon = playerState.AllNeuromon.Where(n => !n.IsDead).ToList();

            var neuromonIndex = _rand.Next(0, aliveNeuromon.Count);
            return aliveNeuromon[neuromonIndex];
        }

        private static RouletteWheel<Move> CreateRouletteWheel(Neuromon neuromon)
        {
            var fitnessProportionateProbabilityCalculator = new FitnessProportionateProbabilityCalculator<Move>(neuromon.MoveSet.Moves);

            var selectionProbabilities = fitnessProportionateProbabilityCalculator.Calculate(m => m.Damage * 1.0);

            return new RouletteWheel<Move>(selectionProbabilities);
        }
    }
}
