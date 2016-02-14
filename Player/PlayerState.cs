using System;
using System.Linq;
using Common;

namespace Player
{
    public sealed class PlayerState : IPlayerState
    {
        public string Name { get; }
        public NeuromonCollection NeuromonCollection { get; }
        public Neuromon ActiveNeuromon { get; private set; }

        public PlayerState(string name, NeuromonCollection neuromonCollection)
        {
            Name = name;
            NeuromonCollection = neuromonCollection;
            ActiveNeuromon = NeuromonCollection.First();
        }

        public PlayerState(IPlayerState toCopy)
        {
            Name = toCopy.Name;
            NeuromonCollection = new NeuromonCollection(toCopy.NeuromonCollection);
            ActiveNeuromon = new Neuromon(toCopy.ActiveNeuromon);
        }

        public void SwitchActiveNeuromon(Neuromon newActiveNeuromon)
        {
            if (!NeuromonCollection.Contains(newActiveNeuromon))
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