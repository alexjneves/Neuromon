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
        private const string HumanPlayer = "human";
        private const string IntelligentAiPlayer = "intelligent";
        private const string RandomAiPlayer = "random";
        private const string NeatAiPlayer = "neat";

        private readonly IPlayerControllerFactory _humanPlayerControllerFactory;
        private readonly IPlayerControllerFactory _intelligentAiPlayerControllerFactory;
        private readonly IPlayerControllerFactory _randomAiPlayerControllerFactory;
        private readonly IPlayerControllerFactory _neatAiPlayerControllerFactory;

        public PlayerControllerFactory(string brainFileName)
        {
            _humanPlayerControllerFactory = new HumanPlayerControllerFactory();
            _intelligentAiPlayerControllerFactory = new IntelligentAiPlayerControllerFactory();
            _randomAiPlayerControllerFactory = new RandomAiPlayerControllerFactory();
            _neatAiPlayerControllerFactory = new NeatAiPlayerControllerFactory(brainFileName);
        }

        public IPlayerController Create(string playerType, IPlayerState initialState)
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
                    return _neatAiPlayerControllerFactory.CreatePlayer(initialState);
                default:
                    throw new Exception($"{playerType} is an unsupported player type");
            }
        }
    }
}