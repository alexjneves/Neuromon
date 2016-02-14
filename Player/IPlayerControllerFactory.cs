namespace Player
{
    public interface IPlayerControllerFactory
    {
        IPlayerController CreatePlayer(IPlayerState initialState);
    }
}
