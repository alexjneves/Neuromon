using System.Collections.Generic;
using Common;

namespace Player
{
    public interface IPlayerState
    {
        string Name { get; }
        NeuromonCollection AllNeuromon { get; }
        NeuromonCollection InactiveNeuromon { get; }
        Neuromon ActiveNeuromon { get; }

        void SwitchActiveNeuromon(Neuromon newActiveNeuromon);
    }
}