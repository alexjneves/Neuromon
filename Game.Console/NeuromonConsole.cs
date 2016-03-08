using System.IO;
using Data;
using Game.Damage;
using Newtonsoft.Json;
using Player;

namespace Game.Console
{
    internal sealed class NeuromonConsole
    {
        private const string GameSettingsFileName = "Config/GameSettings.json";

        private static void Main(string[] args)
        {
            var gameSettingsJson = File.ReadAllText(GameSettingsFileName);
            var gameSettings = JsonConvert.DeserializeObject<GameSettings>(gameSettingsJson);

            var gameDatabase = GameDatabase.CreateFromFiles(
                gameSettings.TypesFileName,
                gameSettings.MovesFileName,
                gameSettings.NeuromonFileName
            );

            var playerOneControllerFactory = PlayerControllerFactoryFactory.Create(
                gameSettings.PlayerOneType,
                gameSettings.NumberOfNeuromon,
                gameSettings.InputNeuronCount,
                gameSettings.OutputNeuronCount,
                gameSettings.PlayerOneBrain
            );

            var playerTwoControllerFactory = PlayerControllerFactoryFactory.Create(
                gameSettings.PlayerTwoType,
                gameSettings.NumberOfNeuromon,
                gameSettings.InputNeuronCount,
                gameSettings.OutputNeuronCount,
                gameSettings.PlayerTwoBrain
            );

            var randomNeuromonGenerator = new NeuromonCollectionGenerator(gameDatabase.Neuromon, gameSettings.NumberOfNeuromon);

            var playerOneState = new PlayerState(gameSettings.PlayerOneName, randomNeuromonGenerator.GenerateNeuromonCollection());
            var playerOneController = playerOneControllerFactory.CreatePlayer(playerOneState);
            var playerOne = new Player.Player(playerOneState, playerOneController);

            var playerTwoState = new PlayerState(gameSettings.PlayerTwoName, randomNeuromonGenerator.GenerateNeuromonCollection());
            var playerTwoController = playerTwoControllerFactory.CreatePlayer(playerTwoState);
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
