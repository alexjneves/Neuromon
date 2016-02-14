using Common;

namespace Player
{
    public interface IPlayerState
    {
        string Name { get; }
        NeuromonCollection NeuromonCollection { get; }
        Neuromon ActiveNeuromon { get; }
        void SwitchActiveNeuromon(Neuromon newActiveNeuromon);
    }
}