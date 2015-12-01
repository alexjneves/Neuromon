using Common;

namespace AI.Intelligent
{
    public sealed class IntelligentAiPlayerFactory : IAiPlayerFactory
    {
        public IPlayer CreatePlayer(string name, Neuromon neuromon)
        {
            return new IntelligentAiPlayer(name, neuromon);
        }
    }
}
