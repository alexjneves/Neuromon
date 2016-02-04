using System;
using System.Linq;
using Common;
using Common.Turn;

namespace Player.Human
{
    internal sealed class HumanPlayer : IPlayer
    {
        private const ConsoleKey AttackKey = ConsoleKey.D1;
        private const ConsoleKey ChangeNeuromonKey = ConsoleKey.D2;

        public string Name { get; }
        public NeuromonCollection Neuromon { get; }
        public Neuromon ActiveNeuromon { get; set; }

        public HumanPlayer(string name, NeuromonCollection neuromon)
        {
            Name = name;
            Neuromon = neuromon;
            ActiveNeuromon = neuromon.First();
        }

        public ITurn ChooseTurn()
        {
            Console.WriteLine($"1: Attack\n2: Change Neuromon");

            var turnType = Console.ReadKey().Key;
            Console.WriteLine("\n");

            if (turnType == AttackKey)
            {
                return ChooseAttack();
            }

            if (turnType == ChangeNeuromonKey)
            {
                return ChooseActiveNeuromon();
            }

            throw new Exception("Invalid Move Selection");
        }

        private ITurn ChooseAttack()
        {
            Console.WriteLine("Choose Attack:");

            var attack = Console.ReadKey().Key;
            Console.WriteLine("\n");

            var move = DetermineMove(attack);
            return new Attack(move);
        }

        private Move DetermineMove(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.D1:
                    return ActiveNeuromon.MoveSet.MoveOne();
                case ConsoleKey.D2:
                    return ActiveNeuromon.MoveSet.MoveTwo();
                case ConsoleKey.D3:
                    return ActiveNeuromon.MoveSet.MoveThree();
                case ConsoleKey.D4:
                    return ActiveNeuromon.MoveSet.MoveFour();
                default:
                    throw new Exception("Invalid choice");
            }
        }

        private ITurn ChooseActiveNeuromon()
        {
            Console.WriteLine("Choose Neuromon:");

            var neuromonIndex = int.Parse(Console.ReadKey().KeyChar.ToString());

            Console.WriteLine();

            var otherNeuromon = Neuromon.Where(n => n != ActiveNeuromon).ToList();
            var newActiveNeuromon = otherNeuromon[neuromonIndex - 1];

            return new ChangeNeuromon(newActiveNeuromon);
        }
    }
}