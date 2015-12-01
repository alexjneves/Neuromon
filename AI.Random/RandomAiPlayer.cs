using Common;

namespace AI.Random
{
    internal sealed class RandomAiPlayer : IPlayer
    {
        public string Name { get; }
        public Neuromon Neuromon { get; }

        private readonly System.Random _rand;

        public RandomAiPlayer(string name, Neuromon neuromon)
        {
            Name = name;
            Neuromon = neuromon;

            _rand = new System.Random();
        }

        public Turn ChooseTurn()
        {
            var move = _rand.Next(1, 5);
            var selectedMove = Neuromon.MoveSet.GetMove(move);

            return new Turn(selectedMove);
        }
    }
}
