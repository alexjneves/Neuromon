using System;
using Common;
using Player;
using Player.AI.Intelligent;
using Player.AI.Random;
using Player.Human;

namespace Game
{
    internal sealed class PlayerFactory
    {
        private const string HumanPlayer = "human";
        private const string IntelligentAiPlayer = "intelligent";
        private const string RandomAiPlayer = "random";

        private readonly HumanPlayerFactory _humanPlayerFactory;
        private readonly IntelligentAiPlayerFactory _intelligentAiPlayerFactory;
        private readonly RandomAiPlayerFactory _randomAiPlayerFactory;

        public PlayerFactory()
        {
            _humanPlayerFactory = new HumanPlayerFactory();
            _intelligentAiPlayerFactory = new IntelligentAiPlayerFactory();
            _randomAiPlayerFactory = new RandomAiPlayerFactory();
        }

        public IPlayer Create(string playerType, string playerName, NeuromonCollection playerNeuromon)
        {
            switch (playerType)
            {
                case HumanPlayer:
                    return _humanPlayerFactory.CreatePlayer(playerName, playerNeuromon);
                case IntelligentAiPlayer:
                    return _intelligentAiPlayerFactory.CreatePlayer(playerName, playerNeuromon);
                case RandomAiPlayer:
                    return _randomAiPlayerFactory.CreatePlayer(playerName, playerNeuromon);
                default:
                    throw new Exception($"{playerType} is an unsupported player type");
            }
        }
    }
}