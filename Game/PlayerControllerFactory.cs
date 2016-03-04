using System;
using Player;
using Player.AI.Intelligent;
using Player.AI.Neat;
using Player.AI.Random;
using Player.Human;

namespace Game
{
    public sealed class PlayerControllerFactory
    {
        private readonly int _numberOfNeuromon;
        private readonly int _inputCount;
        private readonly int _outputCount;
        private const string HumanPlayer = "human";
        private const string IntelligentAiPlayer = "intelligent";
        private const string RandomAiPlayer = "random";
        private const string NeatAiPlayer = "neat";

        private readonly IPlayerControllerFactory _humanPlayerControllerFactory;
        private readonly IPlayerControllerFactory _intelligentAiPlayerControllerFactory;
        private readonly IPlayerControllerFactory _randomAiPlayerControllerFactory;

        public PlayerControllerFactory(int numberOfNeuromon, int inputCount, int outputCount)
        {
            _numberOfNeuromon = numberOfNeuromon;
            _inputCount = inputCount;
            _outputCount = outputCount;
            _humanPlayerControllerFactory = new HumanPlayerControllerFactory();
            _intelligentAiPlayerControllerFactory = new IntelligentAiPlayerControllerFactory();
            _randomAiPlayerControllerFactory = new RandomAiPlayerControllerFactory();
        }

        public IPlayerController Create(string playerType, IPlayerState initialState, string brainFileName)
        {
            switch (playerType)
            {
                case HumanPlayer:
                    return _humanPlayerControllerFactory.CreatePlayer(initialState);
                case IntelligentAiPlayer:
                    return _intelligentAiPlayerControllerFactory.CreatePlayer(initialState);
                case RandomAiPlayer:
                    return _randomAiPlayerControllerFactory.CreatePlayer(initialState);
                case NeatAiPlayer:
                    return new NeatAiPlayerControllerFactory(brainFileName, _numberOfNeuromon, _inputCount, _outputCount).CreatePlayer(initialState);
                default:
                    throw new Exception($"{playerType} is an unsupported player type");
            }
        }
    }
}