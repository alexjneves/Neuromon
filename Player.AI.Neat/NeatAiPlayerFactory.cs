using Common;

namespace Player.AI.Neat
{
    public sealed class NeatAiPlayerFactory : IPlayerFactory
    {
        public IPlayer CreatePlayer(string name, NeuromonCollection neuromon)
        {
            return new NeatAiPlayer(name, neuromon);
        }
    }
}