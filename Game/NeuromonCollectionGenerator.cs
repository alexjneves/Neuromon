using System;
using System.Collections.Generic;
using Common;

namespace Game
{
    internal sealed class NeuromonCollectionGenerator
    {
        private readonly IList<Neuromon> _allNeuromon;
        private readonly int _numberOfNeuromon;
        private readonly Random _rand;

        public NeuromonCollectionGenerator(IList<Neuromon> allNeuromon, int numberOfNeuromon)
        {
            if (allNeuromon.Count < numberOfNeuromon)
            {
                throw new Exception($"Number of Neuromon ({numberOfNeuromon}) is larger than the number of " + $"available neuromon ({allNeuromon.Count})");
            }

            _allNeuromon = allNeuromon;
            _numberOfNeuromon = numberOfNeuromon;
            _rand = new Random();
        }

        public NeuromonCollection GenerateNeuromonCollection()
        {
            var neuromonHashSet = new HashSet<Neuromon>();

            while (neuromonHashSet.Count < _numberOfNeuromon)
            {
                var nextIndex = _rand.Next(_allNeuromon.Count);
                neuromonHashSet.Add(new Neuromon(_allNeuromon[nextIndex]));
            }

            return new NeuromonCollection(neuromonHashSet);
        }
    }
}