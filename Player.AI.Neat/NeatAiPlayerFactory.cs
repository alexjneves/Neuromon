using Common;
using SharpNeat.Phenomes;

namespace Player.AI.Neat
{
    public sealed class NeatAiPlayerFactory : IPlayerFactory
    {
        private readonly IBlackBox _brain;

        public NeatAiPlayerFactory(IBlackBox brain)
        {
            _brain = brain;
        }

        public IPlayer CreatePlayer(string name, NeuromonCollection neuromon)
        {
            return new NeatAiPlayer(name, neuromon, _brain);
        }
    }
}