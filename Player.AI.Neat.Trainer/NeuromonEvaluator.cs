using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Common;
using Data;
using Game;
using Game.Damage;
using SharpNeat.Core;
using SharpNeat.Phenomes;

namespace Player.AI.Neat.Trainer
{
    internal sealed class NeuromonEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        private const string TraineeName = "Trainee";
        private const string OpponentName = "Opponent";

        private readonly TrainingGameSettings _trainingGameSettings;
        private readonly double _desiredFitness;

        private readonly PlayerControllerFactory _opponentPlayerControllerFactory;
        private readonly IDamageCalculator _damageCalculator;

        private readonly IList<Tuple<NeuromonCollection, NeuromonCollection>> _gameNeuromonCollectionCombinations;

        private Renderer _renderer;

        public ulong EvaluationCount { get; private set; }
        public bool StopConditionSatisfied { get; private set; }

        public NeuromonEvaluator(TrainingGameSettings trainingGameSettings, ExperimentSettings experimentSettings)
        {
            _trainingGameSettings = trainingGameSettings;
            _desiredFitness = experimentSettings.DesiredFitness;

            _opponentPlayerControllerFactory = new PlayerControllerFactory(
                _trainingGameSettings.OpponentBrainFileName, 
                _trainingGameSettings.NumberOfNeuromon,
                experimentSettings.InputCount,
                experimentSettings.OutputCount
            );

            _damageCalculator = new DamageCalculatorFactory(
                _trainingGameSettings.EffectiveMultiplier, _trainingGameSettings.WeakMultiplier,
                _trainingGameSettings.MinimumRandomMultiplier, _trainingGameSettings.MaximumRandomMultiplier,
                _trainingGameSettings.NonDeterministic
            ).Create();

            var gameDatabase = GameDatabase.CreateFromFiles(
                _trainingGameSettings.TypesFileName,
                _trainingGameSettings.MovesFileName,
                _trainingGameSettings.NeuromonFileName
                );

            _gameNeuromonCollectionCombinations = new GameNeuromonCombinationsGenerator(
                gameDatabase.Neuromon,
                _trainingGameSettings.NumberOfNeuromon
            ).CreateGameNeuromonCollectionCombinations();

            _renderer = null;

            EvaluationCount = 0;
            StopConditionSatisfied = false;
        }

        public FitnessInfo Evaluate(IBlackBox phenome)
        {
            var accumulatedFitness = 0.0;

            foreach (var neuromonCollectionCombination in _gameNeuromonCollectionCombinations)
            {
                // TODO: Should each possible game be played multiple times to counteract non-deterministic luck?

                var game = CreateGame(
                    phenome,
                    new NeuromonCollection(neuromonCollectionCombination.Item1),
                    new NeuromonCollection(neuromonCollectionCombination.Item2)
                );

                var result = game.Run();
                accumulatedFitness += CalculateFitness(result);
            }

            var averageFitness = accumulatedFitness / _gameNeuromonCollectionCombinations.Count;

            EvaluationCount++;
            StopConditionSatisfied = averageFitness >= _desiredFitness;

            if (StopConditionSatisfied)
            {
                Console.WriteLine("Desired Fitness Achieved! Stopping training...");
            }

            return new FitnessInfo(averageFitness, averageFitness);
        }

        private BattleSimulator CreateGame(IBlackBox brain, NeuromonCollection traineeNeuromon, NeuromonCollection opponentNeuromon)
        {
            var opponentState = new PlayerState(OpponentName, opponentNeuromon);
            var opponentController = _opponentPlayerControllerFactory.Create(_trainingGameSettings.OpponentType, opponentState);
            var opponent = new Player(opponentState, opponentController);

            var neatPlayerControllerFactory = new NeatAiPlayerControllerFactory(brain, _trainingGameSettings.NumberOfNeuromon);

            var traineeState = new PlayerState(TraineeName, traineeNeuromon);
            var traineeController = neatPlayerControllerFactory.CreatePlayer(traineeState);
            var trainee = new Player(traineeState, traineeController);

            // TODO: Should the trainee player first or second?
            var battleSimulator = new BattleSimulator(opponent, trainee, _damageCalculator, false);

            if (_trainingGameSettings.ShouldRender)
            {
                _renderer = new Renderer(battleSimulator);
            }

            return battleSimulator;
        }

        private static double CalculateFitness(BattleResult result)
        {
            var fitness = 0.0;

            if (result.Winner.Name == TraineeName)
            {
                fitness += 100;
            }

            // TODO: Design fitness function

            return fitness;
        }

        public void Reset()
        {
        }
    }
}