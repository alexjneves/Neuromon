using Common;
using Common.Turn;

namespace Player
{
    public interface IPlayer
    {
        string Name { get; }
        NeuromonCollection Neuromon { get; }
        Neuromon ActiveNeuromon { get; set; }

        ITurn ChooseTurn();
    }
}