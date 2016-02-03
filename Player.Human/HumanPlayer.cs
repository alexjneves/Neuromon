using System;
using System.Linq;
using Common;

namespace Player.Human
{
    internal sealed class HumanPlayer : IPlayer
    {
        public string Name { get; }
        public NeuromonCollection Neuromon { get; }
        public Neuromon ActiveNeuromon { get; }

        public HumanPlayer(string name, NeuromonCollection neuromon)
        {
            Name = name;
            Neuromon = neuromon;
            ActiveNeuromon = neuromon.First();
        }

        public Turn ChooseTurn()
        {
            var choice = Console.ReadKey();
            Console.WriteLine("\n");

            var move = DetermineMove(choice.Key);
            return new Turn(move);
        }

        private Move DetermineMove(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.D1:
                    return Neuromon[0].MoveSet.MoveOne();
                case ConsoleKey.D2:
                    return Neuromon[0].MoveSet.MoveTwo();
                case ConsoleKey.D3:
                    return Neuromon[0].MoveSet.MoveThree();
                case ConsoleKey.D4:
                    return Neuromon[0].MoveSet.MoveFour();
                default:
                    throw new Exception("Invalid choice");
            }
        }
    }
}