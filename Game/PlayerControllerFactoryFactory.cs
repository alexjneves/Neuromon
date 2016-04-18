using System;
using Common;
using Player;
using Player.AI.Intelligent;
using Player.AI.Neat;
using Player.AI.Random;
using Player.Human;

namespace Game
{
    public sealed class PlayerControllerFactoryFactory
    {
        public static IPlayerControllerFactory Create(string playerType, int numberOfNeuromon, int inputCount, int outputCount, string brainFileName)
        {
            switch (playerType.ToLower())
            {
                case PlayerTypes.Human:
                    return new HumanPlayerControllerFactory();
                case PlayerTypes.IntelligentAi:
                    return new IntelligentAiPlayerControllerFactory();
                case PlayerTypes.RandomAi:
                    return new RandomAiPlayerControllerFactory();
                case PlayerTypes.NeatAi:
                    return new NeatAiPlayerControllerFactory(brainFileName, numberOfNeuromon, inputCount, outputCount);
                default:
                    throw new Exception($"{playerType} is an unsupported player type");
            }
        }
    }
}