using System.Collections.Generic;
using Common;

namespace Player
{
    public interface IPlayerState
    {
        string Name { get; }
        NeuromonCollection AllNeuromon { get; }
        Neuromon ActiveNeuromon { get; }
        IEnumerable<Neuromon> InactiveNeuromon { get; } 
        void SwitchActiveNeuromon(Neuromon newActiveNeuromon);
    }
}