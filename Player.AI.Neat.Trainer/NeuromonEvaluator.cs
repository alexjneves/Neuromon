using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Data;
using Game;
using SharpNeat.Core;
using SharpNeat.Phenomes;

namespace Player.AI.Neat.Trainer
{
    internal sealed class NeuromonEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        private readonly TrainingGameSettings _trainingGameSettings;
        private readonly double _desiredFitness;

        public ulong EvaluationCount { get; private set; }
        public bool StopConditionSatisfied { get; private set; }

        public NeuromonEvaluator(TrainingGameSettings trainingGameSettings, double desiredFitness)
        {
            _trainingGameSettings = trainingGameSettings;
            _desiredFitness = desiredFitness;

            EvaluationCount = 0;
            StopConditionSatisfied = false;
        }

        private void CreateGame(IBlackBox brain)
        {
            var gameDatabase = GameDatabase.CreateFromFiles(
                _trainingGameSettings.TypesFileName,
                _trainingGameSettings.MovesFileName,
                _trainingGameSettings.NeuromonFileName
            );

            var playerControllerFactory = new PlayerControllerFactory(_trainingGameSettings.OpponentBrainFileName);

            var opponentState = new PlayerState("Opponent", CreateNeuromonCollection(gameDatabase.Neuromon));
            var opponentController = playerControllerFactory.Create(_trainingGameSettings.OpponentType, opponentState);
            var opponent = new Player(opponentState, opponentController);

            var neatPlayerFactory = new NeatAiPlayerControllerFactory(brain);

            // TODO: Finish Implementing
            // TODO: Create combinations of possible games
        }

        private static NeuromonCollection CreateNeuromonCollection(IList<Neuromon> availableNeuromon)
        {
            var distinct = availableNeuromon.Distinct();
            throw new NotImplementedException();
        }

        public FitnessInfo Evaluate(IBlackBox phenome)
        {
            throw new NotImplementedException();
        }

        private double CalculateFitness(IBlackBox phenome)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}