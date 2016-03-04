using Common;

namespace Player.Human
{
    public sealed class HumanPlayerControllerFactory : IPlayerControllerFactory
    {
        public IPlayerController CreatePlayer(IPlayerState initialState)
        {
            return new HumanPlayerController();
        }
    }
}