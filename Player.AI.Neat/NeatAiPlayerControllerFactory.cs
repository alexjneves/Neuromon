using Common;
using SharpNeat.Phenomes;

namespace Player.AI.Neat
{
    public sealed class NeatAiPlayerControllerFactory : IPlayerControllerFactory
    {
        private readonly IBlackBox _brain;

        public NeatAiPlayerControllerFactory(string brainFileName)
        {
            // TODO: Load brain from file
            _brain = null;
        }

        public NeatAiPlayerControllerFactory(IBlackBox brain)
        {
            _brain = brain;
        }

        public IPlayerController CreatePlayer(IPlayerState initiaPlayerState)
        {
            return new NeatAiPlayerController(_brain);
        }
    }
}