﻿using System.Linq;
using Common;
using Common.Turn;
using Player;

namespace AI.Random
{
    internal sealed class RandomAiPlayer : IPlayer
    {
        public string Name { get; }
        public NeuromonCollection Neuromon { get; }
        public Neuromon ActiveNeuromon { get; set; }

        private readonly System.Random _rand;

        public RandomAiPlayer(string name, NeuromonCollection neuromon)
        {
            Name = name;
            Neuromon = neuromon;
            ActiveNeuromon = neuromon.First();

            _rand = new System.Random();
        }

        public ITurn ChooseTurn()
        {
            var move = _rand.Next(1, 5);
            var selectedMove = ActiveNeuromon.MoveSet[move];

            return new Attack(selectedMove);
        }
    }
}
