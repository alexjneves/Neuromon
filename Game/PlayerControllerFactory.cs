using System;
using Common;
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
                case PlayerTypes.Human:
                    return _humanPlayerControllerFactory.CreatePlayer(initialState);
                case PlayerTypes.IntelligentAi:
                    return _intelligentAiPlayerControllerFactory.CreatePlayer(initialState);
                case PlayerTypes.RandomAi:
                    return _randomAiPlayerControllerFactory.CreatePlayer(initialState);
                case PlayerTypes.NeatAi:
                    return new NeatAiPlayerControllerFactory(brainFileName, _numberOfNeuromon, _inputCount, _outputCount).CreatePlayer(initialState);
                default:
                    throw new Exception($"{playerType} is an unsupported player type");
            }
        }
    }
}