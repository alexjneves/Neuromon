using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Common.Turn;

namespace Player.Human
{
    public sealed class HumanPlayerController : IPlayerController
    {
        private const int AttackTurnType = 1;
        private const int ChangeNeuromonTurnType = 2;

        public ITurn ChooseTurn(IPlayerState playerState, IPlayerState opponentState)
        {
            var canSwitchNeuromon = playerState.NeuromonCollection.Count(n => !n.IsDead) > 1;

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
                selectedTurn = ChooseAttack(playerState.ActiveNeuromon);
            }
            else if (turnType == ChangeNeuromonTurnType && canSwitchNeuromon)
            {
                selectedTurn = new ChangeNeuromon(SelectActiveNeuromon(playerState, opponentState));
            }

            return selectedTurn;
        }

        public Neuromon SelectActiveNeuromon(IPlayerState playerState, IPlayerState opponentState)
        {
            Neuromon newActiveNeuromon;
            bool validSelection;

            var otherNeuromon = playerState.NeuromonCollection.Where(n => n != playerState.ActiveNeuromon).ToList();

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

        private static ITurn ChooseAttack(Neuromon activeNeuromon)
        {
            Console.WriteLine("Choose Attack:");

            var moveIndex = ReadInputUntilValid(input => input <= 4 && input > 0, "Invalid Attack!");

            var move = activeNeuromon.MoveSet[moveIndex - 1];
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