using System;
using Common;
using Common.Turn;
using SharpNeat.Phenomes;

namespace Player.AI.Neat
{
    internal sealed class NeatAiPlayer : IPlayer
    {
        public string Name { get; }
        public NeuromonCollection Neuromon { get; }
        public Neuromon ActiveNeuromon { get; set; }

        public NeatAiPlayer(string name, NeuromonCollection neuromon)
        {
            Name = name;
            Neuromon = neuromon;
        }

        public ITurn ChooseTurn()
        {
            throw new NotImplementedException();
        }

        public Neuromon SelectActiveNeuromon()
        {
            throw new NotImplementedException();
        }
    }
}
