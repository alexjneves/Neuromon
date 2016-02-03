using Common;

namespace Player.Human
{
    public sealed class HumanPlayerFactory : IPlayerFactory
    {
        public IPlayer CreatePlayer(string name, NeuromonCollection neuromon)
        {
            return new HumanPlayer(name, neuromon);
        }
    }
}