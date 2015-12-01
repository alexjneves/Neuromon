using Common;

namespace AI
{
    public interface IAiPlayerFactory
    {
        IPlayer CreatePlayer(string name, Neuromon neuromon);
    }
}
