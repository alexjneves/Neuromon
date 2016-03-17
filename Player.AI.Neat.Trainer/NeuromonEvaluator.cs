﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly int _gameCombinationIterations;

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
            _gameCombinationIterations = experimentSettings.GameCombinationIterations;

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
            var accumulatedFitness = 0.0;

            var numberOfGames = _gameNeuromonCollectionCombinations.Count * _gameCombinationIterations;

            foreach (var neuromonCollectionCombination in _gameNeuromonCollectionCombinations)
            {
                // TODO: Should each possible game be played multiple times to counteract non-deterministic luck?

                for (var i = 0; i < _gameCombinationIterations; ++i)
                {
                    var game = CreateGame(
                        phenome,
                        new NeuromonCollection(neuromonCollectionCombination.Item1),
                        new NeuromonCollection(neuromonCollectionCombination.Item2)
                    );

                    var result = game.Run();
                    accumulatedFitness += _scoreCalculator.Calculate(TraineeName, result);
                }
            }

            var averageFitness = accumulatedFitness / numberOfGames;

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
            var opponent = new Player(opponentState, opponentController);

            var neatPlayerControllerFactory = new NeatAiPlayerControllerFactory(brain, _trainingGameSettings.NumberOfNeuromon);

            var traineeState = new PlayerState(TraineeName, traineeNeuromon);
            var traineeController = neatPlayerControllerFactory.CreatePlayer(traineeState);
            var trainee = new Player(traineeState, traineeController);

            // TODO: Should the trainee player first or second?
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