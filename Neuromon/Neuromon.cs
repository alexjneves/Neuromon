using System;
using System.Collections.Generic;

namespace Neuromon
{
    internal sealed class Neuromon
    {
        public string Name { get; }
        public int Health { get; private set; }
        public MoveSet MoveSet { get; }

        public Neuromon(string name)
        {
            Name = name;
            Health = 10;
            MoveSet = GenerateRandomMoveSet();
        }

        private static MoveSet GenerateRandomMoveSet()
        {
            var moves = new List<Move>(4);
            var rand = new Random();

            for (var i = 0; i < 4; ++i)
            {
                var moveName = $"Move{i + 1}";
                var damage = rand.Next(10);

                moves.Add(new Move(moveName, damage));
            }

            return new MoveSet(moves);
        }

        public bool IsDead() => Health <= 0;

        public void TakeDamage(int damage)
        {
            Health -= damage;

            if (Health < 0)
            {
                Health = 0;
            }
        }
    }
}