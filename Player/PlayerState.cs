using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Player
{
    public sealed class PlayerState : IPlayerState
    {
        public string Name { get; }
        public NeuromonCollection AllNeuromon { get; }
        public Neuromon ActiveNeuromon { get; private set; }

        public IEnumerable<Neuromon> InactiveNeuromon
        {
            get { return AllNeuromon.Where(n => n != ActiveNeuromon); }
        }

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
            ActiveNeuromon = new Neuromon(toCopy.ActiveNeuromon);
        }

        public void SwitchActiveNeuromon(Neuromon newActiveNeuromon)
        {
            if (!AllNeuromon.Contains(newActiveNeuromon))
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