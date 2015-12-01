using System;
using Common;

namespace AI.Intelligent
{
    internal sealed class IntelligentAiPlayer : IPlayer
    {
        public string Name { get; }
        public Neuromon Neuromon { get; }

        private readonly Random _rand;
        private readonly RouletteWheel<Move> _rouletteWheel;

        public IntelligentAiPlayer(string name, Neuromon neuromon)
        {
            Name = name;
            Neuromon = neuromon;
            _rand = new Random();

            var fitnessProportionateProbabilityCalculator = new FitnessProportionateProbabilityCalculator<Move>(neuromon.MoveSet.Moves);

            var selectionProbabilities = fitnessProportionateProbabilityCalculator.Calculate(m => m.Damage * 1.0);
            
            _rouletteWheel = new RouletteWheel<Move>(selectionProbabilities);
        }

        public Turn ChooseTurn()
        {
            var move = _rouletteWheel.Spin();

            return new Turn(move);
        }
    }
}
