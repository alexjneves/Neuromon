using System.IO;
using Data;
using Game.Damage;
using Newtonsoft.Json;
using Player;

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

            var playerControllerFactory = new PlayerControllerFactory(
                gameSettings.NumberOfNeuromon,
                gameSettings.InputNeuronCount,
                gameSettings.OutputNeuronCount
            );

            var randomNeuromonGenerator = new NeuromonCollectionGenerator(gameDatabase.Neuromon, gameSettings.NumberOfNeuromon);

            var playerOneState = new PlayerState(gameSettings.PlayerOneName, randomNeuromonGenerator.GenerateNeuromonCollection());
            var playerOneController = playerControllerFactory.Create(gameSettings.PlayerOneType, playerOneState, gameSettings.PlayerOneBrain);
            var playerOne = new Player.Player(playerOneState, playerOneController);

            var playerTwoState = new PlayerState(gameSettings.PlayerTwoName, randomNeuromonGenerator.GenerateNeuromonCollection());
            var playerTwoController = playerControllerFactory.Create(gameSettings.PlayerTwoType, playerTwoState, gameSettings.PlayerTwoBrain);
            var playerTwo = new Player.Player(playerTwoState, playerTwoController);

            var damageCalculatorFactory = new DamageCalculatorFactory(
                gameSettings.EffectiveMultiplier, gameSettings.WeakMultiplier,
                gameSettings.MinimumRandomMultiplier, gameSettings.MaximumRandomMultiplier,
                gameSettings.NonDeterministic
            );

            var battleSimulator = new BattleSimulator(playerOne, playerTwo, damageCalculatorFactory.Create());

            Renderer renderer = null;

            if (gameSettings.ShouldRender)
            {
                renderer = new Renderer(battleSimulator, gameSettings.SimulateThinking);
            }

            battleSimulator.Run();
        }
    }
}
