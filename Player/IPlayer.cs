using Common;

namespace Player
{
    public interface IPlayer
    {
        string Name { get; }
        NeuromonCollection Neuromon { get; }
        Neuromon ActiveNeuromon { get; }

        Turn ChooseTurn();
    }
}