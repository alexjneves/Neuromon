using System;
using Common;
using Common.Turn;
using SharpNeat.Phenomes;

namespace Player.AI.Neat
{
    internal sealed class NeatAiPlayer : IPlayer
    {
        private readonly IBlackBox _brain;

        public string Name { get; }
        public NeuromonCollection Neuromon { get; }
        public Neuromon ActiveNeuromon { get; set; }

        public NeatAiPlayer(string name, NeuromonCollection neuromon, IBlackBox brain)
        {
            Name = name;
            Neuromon = neuromon;
            _brain = brain;
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
