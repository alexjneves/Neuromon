using System;
using System.Linq;
using Common;

namespace Player
{
    public sealed class PlayerState : IPlayerState
    {
        public string Name { get; }
        public NeuromonCollection AllNeuromon { get; }
        public Neuromon ActiveNeuromon { get; private set; }

        public NeuromonCollection InactiveNeuromon => new NeuromonCollection(AllNeuromon.Where(n => n != ActiveNeuromon));

        public PlayerState(string name, NeuromonCollection allNeuromon)
        {
            Name = name;
            AllNeuromon = allNeuromon;
            ActiveNeuromon = AllNeuromon.First();
        }

        public PlayerState(IPlayerState toCopy)
        {
            Name = toCopy.Name;
            AllNeuromon = new NeuromonCollection(toCopy.AllNeuromon);
            ActiveNeuromon = AllNeuromon.First();
        }

        public void SwitchActiveNeuromon(Neuromon newActiveNeuromon)
        {
            if (!InactiveNeuromon.Contains(newActiveNeuromon))
            {
                throw new Exception("Must switch to a Neuromon in the Neuromon Collection");
            }

            if (newActiveNeuromon.IsDead)
            {
                throw new Exception("Cannot choose a dead Neuromon to be the active Neuromon");
            }

            ActiveNeuromon = newActiveNeuromon;
        }
    }
}