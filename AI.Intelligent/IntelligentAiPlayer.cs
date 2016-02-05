using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Turn;
using Player;

namespace AI.Intelligent
{
    internal sealed class IntelligentAiPlayer : IPlayer
    {
        public string Name { get; }
        public NeuromonCollection Neuromon { get; }
        public Neuromon ActiveNeuromon { get; set; }

        private readonly Random _rand;
        private readonly Dictionary<Neuromon, RouletteWheel<Move>> _neuromonRouletteWheels;

        public IntelligentAiPlayer(string name, NeuromonCollection neuromonCollection)
        {
            Name = name;
            Neuromon = neuromonCollection;
            ActiveNeuromon = neuromonCollection.First();

            _rand = new Random();
            _neuromonRouletteWheels = new Dictionary<Neuromon, RouletteWheel<Move>>();

            foreach (var neuromon in neuromonCollection)
            {
                var rouletteWheel = CreateRouletteWheel(neuromon);
                _neuromonRouletteWheels.Add(neuromon, rouletteWheel);
            }
        }

        public ITurn ChooseTurn()
        {
            RouletteWheel<Move> rouletteWheel;
            if (!_neuromonRouletteWheels.TryGetValue(ActiveNeuromon, out rouletteWheel))
            {
                throw new Exception($"Roulette Wheel does not exist for Neuromon {ActiveNeuromon.Name}");
            }

            var move = rouletteWheel.Spin();

            return new Attack(move);
        }

        public Neuromon SelectActiveNeuromon()
        {
            var aliveNeuromon = Neuromon.Where(n => !n.IsDead).ToList();

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
