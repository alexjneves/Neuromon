namespace Player.AI.Intelligent
{
    public sealed class IntelligentAiPlayerControllerFactory : IPlayerControllerFactory
    {
        public IPlayerController CreatePlayer(IPlayerState initialState)
        {
            return new IntelligentAiPlayerController(initialState);
        }
    }
}
