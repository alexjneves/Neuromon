using System;
using System.Collections.Generic;
using System.Linq;

namespace Player.AI.Intelligent
{
    internal sealed class FitnessProportionateProbabilityObjectCollection<T>
    {
        public IEnumerable<FitnessProportionateProbabilityObject<T>> ProbabilityObjects { get; }

        public FitnessProportionateProbabilityObjectCollection(IList<FitnessProportionateProbabilityObject<T>> fitnessProportionateProbabilityObjects)
        {
            var sum = fitnessProportionateProbabilityObjects.Select(po => po.Probability).Sum();

            if (sum != 1.0)
            {
                throw new Exception($"Fitness Probability values should total 1, however they totalled {sum}");
            }

            ProbabilityObjects = fitnessProportionateProbabilityObjects;
        }
    }
}
