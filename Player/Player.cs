namespace Player
{
    public class Player : IPlayer
    {
        public IPlayerState State { get; }
        public IPlayerController Controller { get; }

        public Player(IPlayerState state, IPlayerController controller)
        {
            State = state;
            Controller = controller;
        }
    }
}