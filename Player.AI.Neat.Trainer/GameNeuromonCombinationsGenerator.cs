using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Player.AI.Neat.Trainer
{
    /**
    * TODO: Is the search space going to be too large?
    * TODO: What is the efficiency of this class?
    * Combinations of all possible neuromon collections for two players, e.g.
    * 5 possible Neuromon in the Database: A, B, C, D, E
    * Each player has 3 Neuromon
    * Player 1 may have 5 Choose 3 Neuromon = 10
    * First Neuromon matters as that will be the Active, therefore each combination has 3 potential orders
    * 10 * 30 = 30
    * For each of those, Player 2 may have each of the possible Neuromon collections, therefore 30 * 30 = 900 combinations
    */
    internal sealed class GameNeuromonCombinationsGenerator
    {
        private readonly IList<Neuromon> _allNeuromon;
        private readonly int _numberToChoose;

        public GameNeuromonCombinationsGenerator(IList<Neuromon> allNeuromon, int numberToChoose)
        {
            _allNeuromon = allNeuromon;
            _numberToChoose = numberToChoose;
        }

        public IList<Tuple<NeuromonCollection, NeuromonCollection>> CreateGameNeuromonCollectionCombinations()
        {
            var neuromonCombinations = CombineWithOrder(Combinations(_allNeuromon, _numberToChoose)).ToList();

            var gameNeuromonCollectionCombinations = new List<Tuple<NeuromonCollection, NeuromonCollection>>(neuromonCombinations.Count * neuromonCombinations.Count); 

            foreach (var neuromonCombination in neuromonCombinations)
            {
                foreach (var otherNeuromonCombination in neuromonCombinations)
                {
                    gameNeuromonCollectionCombinations.Add(new Tuple<NeuromonCollection, NeuromonCollection>(
                            new NeuromonCollection(neuromonCombination),
                            new NeuromonCollection(otherNeuromonCombination))
                    );
                }
            }

            return gameNeuromonCollectionCombinations;
        }

        private static IEnumerable<IEnumerable<T>> Combinations<T>(IEnumerable<T> elements, int n)
        {
            if (n == 0)
            {
                return new[] { new T[0] };
            }

            return elements.SelectMany((e, i) =>
            {
                return Combinations(elements.Skip(i + 1), n - 1).Select(c => (new[] { e }).Concat(c));
            });
        }

        private static IEnumerable<IEnumerable<T>> CombineWithOrder<T>(IEnumerable<IEnumerable<T>> numberCombinations)
        {
            var numberCombinationsArray = numberCombinations as IEnumerable<T>[] ?? numberCombinations.ToArray();

            var combined = new List<List<T>>(numberCombinationsArray.Length * numberCombinationsArray.Length);

            foreach (var numberCombinationArray in numberCombinationsArray.Select(numberCombination => numberCombination.ToArray()))
            {
                for (var i = 0; i < numberCombinationArray.Length; ++i)
                {
                    var l = new List<T>(numberCombinationArray.Length)
                    {
                        numberCombinationArray.ElementAt(i)
                    };

                    l.AddRange(numberCombinationArray.Where(element => !element.Equals(numberCombinationArray.ElementAt(i))));

                    combined.Add(l);
                }
            }

            return combined;
        }
    }
}