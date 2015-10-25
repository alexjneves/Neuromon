using System;

namespace Neuromon
{
    internal sealed class HumanPlayer : IPlayer
    {
        public string Name { get; }
        public Neuromon Neuromon { get; }

        public HumanPlayer(string name)
        {
            Name = name;
            Neuromon = new Neuromon(name + "'s Neuromon");
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
                    return Neuromon.MoveSet.MoveOne();
                case ConsoleKey.D2:
                    return Neuromon.MoveSet.MoveTwo();
                case ConsoleKey.D3:
                    return Neuromon.MoveSet.MoveThree();
                case ConsoleKey.D4:
                    return Neuromon.MoveSet.MoveFour();
                default:
                    throw new Exception("Invalid choice");
            }
        }
    }
}