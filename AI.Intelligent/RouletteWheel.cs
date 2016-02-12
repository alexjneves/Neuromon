using System;
using System.Collections.Generic;
using System.Linq;

namespace Player.AI.Intelligent
{
    internal sealed class RouletteWheel<T>
    {
        private const int NumberOfElements = 100;

        private readonly List<T> _wheel;
        private readonly Random _rand;

        public RouletteWheel(FitnessProportionateProbabilityObjectCollection<T> probabilityObjectCollection)
        {
            _wheel = new List<T>(NumberOfElements);
            _rand = new Random();

            PopulateWheel(probabilityObjectCollection);
        }

        public T Spin()
        {
            var randomPosition = _rand.Next(0, NumberOfElements);

            return _wheel[randomPosition];
        }

        private void PopulateWheel(FitnessProportionateProbabilityObjectCollection<T> probabilityObjectCollection)
        {
            foreach (var probabilityObject in probabilityObjectCollection.ProbabilityObjects)
            {
                var numberOfWheelPositions = CalculateNumberOfWheelPositions(probabilityObject.Probability);

                _wheel.AddRange(Enumerable.Repeat(probabilityObject.Item, numberOfWheelPositions));
            }

            if (_wheel.Count != NumberOfElements)
            {
                throw new Exception($"Roulette Wheel contents should equal {NumberOfElements}");
            }
        }

        private static int CalculateNumberOfWheelPositions(double probability)
        {
            return (int) Math.Round(probability * 100, MidpointRounding.AwayFromZero);
        }
    }
}
