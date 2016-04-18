using System;
using System.Collections.Generic;
using Common;
using Data;
using Game;
using Game.Damage;
using Player;
using Player.AI.Neat;
using SharpNeat.Core;
using SharpNeat.Phenomes;

namespace Trainer
{
    internal sealed class NeuromonEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        private const string TraineeName = "Trainee";
        private const string OpponentName = "Opponent";

        private readonly TrainingGameSettings _trainingGameSettings;
        private readonly double _desiredFitness;
        private readonly int _initialGameStateIterations;

        private readonly IPlayerControllerFactory _opponentPlayerControllerFactory;
        private readonly IDamageCalculator _damageCalculator;
        private readonly IList<Tuple<NeuromonCollection, NeuromonCollection>> _gameNeuromonCollectionCombinations;
        private readonly ScoreCalculator _scoreCalculator;

        private Renderer _renderer;

        public ulong EvaluationCount { get; private set; }
        public bool StopConditionSatisfied { get; private set; }

        public NeuromonEvaluator(TrainingGameSettings trainingGameSettings, ExperimentSettings experimentSettings)
        {
            _trainingGameSettings = trainingGameSettings;
            _desiredFitness = experimentSettings.DesiredFitness;
            _initialGameStateIterations = experimentSettings.InitialGameStateIterations;

            _opponentPlayerControllerFactory = PlayerControllerFactoryFactory.Create(
                 _trainingGameSettings.OpponentType,
                _trainingGameSettings.NumberOfNeuromon,
                experimentSettings.InputCount,
                experimentSettings.OutputCount,
                _trainingGameSettings.OpponentBrainFileName
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

            _scoreCalculator = new ScoreCalculator();
            _renderer = null;

            EvaluationCount = 0;
            StopConditionSatisfied = false;
        }

        public FitnessInfo Evaluate(IBlackBox phenome)
        {
            var accumulatedFitnessTotal = 0.0;

            foreach (var neuromonCollectionCombination in _gameNeuromonCollectionCombinations)
            {
                var localFitnessTotal = 0.0;

                for (var i = 0; i < _initialGameStateIterations; ++i)
                {
                    var game = CreateGame(
                        phenome,
                        new NeuromonCollection(neuromonCollectionCombination.Item1),
                        new NeuromonCollection(neuromonCollectionCombination.Item2)
                    );

                    var result = game.Run();
                    localFitnessTotal += _scoreCalculator.Calculate(TraineeName, result);
                }

                var localFitnessAverage = localFitnessTotal / _initialGameStateIterations;
                accumulatedFitnessTotal += localFitnessAverage;
            }

            var averageFitness = accumulatedFitnessTotal / _gameNeuromonCollectionCombinations.Count;

            EvaluationCount++;

            if (!StopConditionSatisfied)
            {
                StopConditionSatisfied = averageFitness >= _desiredFitness;
            }

            return new FitnessInfo(averageFitness, averageFitness);
        }

        private BattleSimulator CreateGame(IBlackBox brain, NeuromonCollection traineeNeuromon, NeuromonCollection opponentNeuromon)
        {
            var opponentState = new PlayerState(OpponentName, opponentNeuromon);
            var opponentController = _opponentPlayerControllerFactory.CreatePlayer(opponentState);
            var opponent = new Player.Player(opponentState, opponentController);

            var neatPlayerControllerFactory = new NeatAiPlayerControllerFactory(brain, _trainingGameSettings.NumberOfNeuromon);

            var traineeState = new PlayerState(TraineeName, traineeNeuromon);
            var traineeController = neatPlayerControllerFactory.CreatePlayer(traineeState);
            var trainee = new Player.Player(traineeState, traineeController);

            var battleSimulator = new BattleSimulator(opponent, trainee, _damageCalculator);

            if (_trainingGameSettings.ShouldRender)
            {
                _renderer = new Renderer(battleSimulator, false);
            }

            return battleSimulator;
        }

        public void Reset()
        {
        }
    }
}