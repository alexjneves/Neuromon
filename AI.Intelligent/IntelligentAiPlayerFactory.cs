using Common;

namespace Player.AI.Intelligent
{
    public sealed class IntelligentAiPlayerFactory : IPlayerFactory
    {
        public IPlayer CreatePlayer(string name, NeuromonCollection neuromon)
        {
            return new IntelligentAiPlayer(name, neuromon);
        }
    }
}
