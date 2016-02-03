using Common;
using Player;

namespace AI.Random
{
    public sealed class RandomAiPlayerFactory : IPlayerFactory
    {
        public IPlayer CreatePlayer(string name, NeuromonCollection neuromon)
        {
            return new RandomAiPlayer(name, neuromon);
        }
    }
}