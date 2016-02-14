namespace Player
{
    public interface IPlayer
    {
        IPlayerState State { get; }
        IPlayerController Controller { get; }
    }
}