using System.Linq;
using Common;
using Player;

namespace AI.Random
{
    internal sealed class RandomAiPlayer : IPlayer
    {
        public string Name { get; }
        public NeuromonCollection Neuromon { get; }
        public Neuromon ActiveNeuromon { get; }

        private readonly System.Random _rand;

        public RandomAiPlayer(string name, NeuromonCollection neuromon)
        {
            Name = name;
            Neuromon = neuromon;
            ActiveNeuromon = neuromon.First();

            _rand = new System.Random();
        }

        public Turn ChooseTurn()
        {
            var move = _rand.Next(1, 5);
            var selectedMove = ActiveNeuromon.MoveSet[move];

            return new Turn(selectedMove);
        }
    }
}
