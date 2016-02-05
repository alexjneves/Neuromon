using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Common.Turn;

namespace Player.Human
{
    public sealed class HumanPlayer : IPlayer
    {
        private const int AttackTurnType = 1;
        private const int ChangeNeuromonTurnType = 2;

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
            var canSwitchNeuromon = Neuromon.Count(n => !n.IsDead) > 1;

            var validTurnTypes = new List<int>()
            {
                AttackTurnType
            };

            var sb = new StringBuilder();
            sb.AppendLine("1: Attack");

            if (canSwitchNeuromon)
            {
                sb.AppendLine("2: Change Neuromon");
                validTurnTypes.Add(ChangeNeuromonTurnType);
            }

            Console.WriteLine(sb.ToString());

            ITurn selectedTurn = null;
            var turnType = ReadInputUntilValid(input => validTurnTypes.Contains(input));

            if (turnType == AttackTurnType)
            {
                selectedTurn = ChooseAttack();
            }
            else if (turnType == ChangeNeuromonTurnType && canSwitchNeuromon)
            {
                selectedTurn = new ChangeNeuromon(SelectActiveNeuromon());
            }

            return selectedTurn;
        }

        public Neuromon SelectActiveNeuromon()
        {
            Neuromon newActiveNeuromon;
            bool validSelection;

            var otherNeuromon = Neuromon.Where(n => n != ActiveNeuromon).ToList();

            do
            {
                Console.WriteLine("Choose Neuromon:");

                var neuromonIndex = ReadInputUntilValid(input => input <= otherNeuromon.Count && input > 0, "Invalid Neuromon Selection!");

                newActiveNeuromon = otherNeuromon[neuromonIndex - 1];

                validSelection = !newActiveNeuromon.IsDead;

                if (!validSelection)
                {
                    Console.WriteLine($"Cannot choose {newActiveNeuromon.Name} as they are dead!");
                }
            } while (!validSelection);

            return newActiveNeuromon;
        }

        private ITurn ChooseAttack()
        {
            Console.WriteLine("Choose Attack:");

            var moveIndex = ReadInputUntilValid(input => input <= 4 && input > 0, "Invalid Attack!");

            var move = ActiveNeuromon.MoveSet[moveIndex - 1];
            return new Attack(move);
        }

        private static int ReadInputUntilValid(Predicate<int> condition, string errorMessage = "Invalid Selection!")
        {
            var conditionMet = false;
            var input = -1;

            while (!conditionMet)
            {
                input = ReadInputUntilValid();
                conditionMet = condition(input);

                if (!conditionMet)
                {
                    Console.WriteLine(errorMessage);
                }
            }

            return input;
        }

        private static int ReadInputUntilValid()
        {
            int neuromonIndex;

            while (!int.TryParse(ReadInput(), out neuromonIndex))
            {
                Console.WriteLine("Invalid Selection!");
            }

            return neuromonIndex;
        }

        private static string ReadInput()
        {
            var input = Console.ReadKey().KeyChar.ToString();
            Console.WriteLine("\n");

            return input;
        } 
    }
}