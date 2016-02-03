using System;
using System.Collections.Generic;
using System.IO;
using AI.Intelligent;
using Common;
using Data;
using Newtonsoft.Json;
using Player.Human;

namespace Game
{
    internal sealed class NeuromonGame
    {
        private const string GameSettingsFileName = "GameSettings.json";

        private static void Main(string[] args)
        {
            var gameSettingsJson = File.ReadAllText(GameSettingsFileName);
            var gameSettings = JsonConvert.DeserializeObject<GameSettings>(gameSettingsJson);

            var gameDatabase = GameDatabase.CreateFromFiles(
                gameSettings.TypesFileName, 
                gameSettings.MovesFileName, 
                gameSettings.NeuromonFileName
            );

            var playerFactory = new PlayerFactory();

            var randomNeuromonGenerator = new NeuromonCollectionGenerator(gameDatabase.Neuromon, gameSettings.NumberOfNeuromon);

            var player1Neuromon = randomNeuromonGenerator.GenerateNeuromonCollection();
            var player2Neuromon = randomNeuromonGenerator.GenerateNeuromonCollection();

            var playerOne = playerFactory.Create(
                gameSettings.PlayerOneType,
                gameSettings.PlayerOneName,
                randomNeuromonGenerator.GenerateNeuromonCollection());

            var playerTwo = playerFactory.Create(
                gameSettings.PlayerTwoType,
                gameSettings.PlayerTwoName,
                randomNeuromonGenerator.GenerateNeuromonCollection());

            //const string player1Name = "Alex";
            //const string player2Name = "Intelligent AI";

            //var aiPlayerFactory = new IntelligentAiPlayerFactory();

            //var player1 = new HumanPlayer(player1Name, CreateRandomNeuromon($"{player1Name}'s Neuromon"));
            //var player2 = aiPlayerFactory.CreatePlayer(player2Name,
            //    CreateAiNeuromon($"{player2Name}'s Neuromon"));

            var battleSimulator = new BattleSimulator(playerOne, playerTwo);
            var renderer = new Renderer(battleSimulator);

            battleSimulator.Run();
        }

        private static Neuromon CreateRandomNeuromon(string name)
        {
            var moveSet = GenerateRandomMoveSet();
            return new Neuromon(name, 60, new GameType("", null, null),  moveSet);
        }

        private static MoveSet GenerateRandomMoveSet()
        {
            var moves = new List<Move>(4);
            var rand = new Random();

            for (var i = 0; i < 4; ++i)
            {
                var moveName = $"Move{i + 1}";
                var damage = rand.Next(1, 10);

                moves.Add(new Move(moveName, new GameType("", null, null), 10, damage));
            }

            return new MoveSet(moves);
        }

        private static Neuromon CreateAiNeuromon(string name)
        {
            var moves = new List<Move>(4);

            //for (var i = 0; i < 4; ++i)
            //{
            //    var moveName = $"Move{i + 1}";
            //    var damage = i + 1;

            //    moves.Add(new Move(moveName, damage));
            //}

            moves.Add(new Move("Move1", new GameType("", null, null), 10, 1));
            moves.Add(new Move("Move2", new GameType("", null, null), 10, 3));
            moves.Add(new Move("Move3", new GameType("", null, null), 10, 5));
            moves.Add(new Move("Move4", new GameType("", null, null), 10, 6));

            return new Neuromon(name, 60, new GameType("", null, null),  new MoveSet(moves));
        }
    }
}
