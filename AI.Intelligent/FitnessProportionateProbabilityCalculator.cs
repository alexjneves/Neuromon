using System;
using System.Collections.Generic;
using System.Linq;

namespace Player.AI.Intelligent
{
    internal sealed class FitnessProportionateProbabilityCalculator<T>
    {
        private readonly ICollection<T> _items;

        public FitnessProportionateProbabilityCalculator(ICollection<T> items)
        {
            _items = items;
        }

        public FitnessProportionateProbabilityObjectCollection<T> Calculate(Func<T, double> fitnessFunction)
        {
            var totalFitness = _items.Sum(fitnessFunction);

            var fitnessProportionateProbabilities = new List<FitnessProportionateProbabilityObject<T>>(_items.Count);

            foreach (var item in _items)
            {
                var fitness = fitnessFunction(item);

                var fitnessProportionateProbability = fitness / totalFitness;

                var probabilityObject = new FitnessProportionateProbabilityObject<T>(item, fitnessProportionateProbability);

                fitnessProportionateProbabilities.Add(probabilityObject);
            }

            return new FitnessProportionateProbabilityObjectCollection<T>(fitnessProportionateProbabilities);
        } 
    }
}
