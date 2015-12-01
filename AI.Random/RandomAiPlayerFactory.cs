using Common;

namespace AI.Random
{
    public sealed class RandomAiPlayerFactory : IAiPlayerFactory
    {
        public IPlayer CreatePlayer(string name, Neuromon neuromon)
        {
            return new RandomAiPlayer(name, neuromon);
        }
    }
}