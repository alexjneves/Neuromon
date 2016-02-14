using Common;

namespace Player.AI.Random
{
    public sealed class RandomAiPlayerControllerFactory : IPlayerControllerFactory
    {
        public IPlayerController CreatePlayer(IPlayerState initialState)
        {
            return new RandomAiPlayerController();
        }
    }
}