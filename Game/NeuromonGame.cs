using System;
using System.Collections.Generic;
using System.IO;
using Common;
using Data;
using Newtonsoft.Json;

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

            var playerOne = playerFactory.Create(
                gameSettings.PlayerOneType,
                gameSettings.PlayerOneName,
                randomNeuromonGenerator.GenerateNeuromonCollection());

            var playerTwo = playerFactory.Create(
                gameSettings.PlayerTwoType,
                gameSettings.PlayerTwoName,
                randomNeuromonGenerator.GenerateNeuromonCollection());

            var battleSimulator = new BattleSimulator(playerOne, playerTwo, gameSettings.SimulateThinking);

            Renderer renderer = null;

            if (gameSettings.ShouldRender)
            {
                renderer = new Renderer(battleSimulator);
            }

            battleSimulator.Run();
        }
    }
}
