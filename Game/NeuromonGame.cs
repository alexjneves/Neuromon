using System;
using System.Collections.Generic;
using AI.Random;
using Common;

namespace Game
{
    internal sealed class NeuromonGame
    {
        private static void Main(string[] args)
        {
            const string player1Name = "Human Player 1";
            const string player2Name = "AI Player 2";

            var aiPlayerFactory = new RandomAiPlayerFactory();

            var player1 = new HumanPlayer(player1Name, CreateRandomNeuromon($"{player1Name}'s Neuromon"));
            var player2 = aiPlayerFactory.CreatePlayer(player2Name,
                CreateRandomNeuromon($"{player2Name}'s Neuromon"));

            var battleSimulator = new BattleSimulator(player1, player2);
            var renderer = new Renderer(battleSimulator);

            battleSimulator.Run();
        }

        private static Neuromon CreateRandomNeuromon(string name)
        {
            var moveSet = GenerateRandomMoveSet();
            return new Neuromon(name, moveSet);
        }

        private static MoveSet GenerateRandomMoveSet()
        {
            var moves = new List<Move>(4);
            var rand = new Random();

            for (var i = 0; i < 4; ++i)
            {
                var moveName = $"Move{i + 1}";
                var damage = rand.Next(1, 10);

                moves.Add(new Move(moveName, damage));
            }

            return new MoveSet(moves);
        }
    }
}
