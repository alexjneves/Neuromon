using System;
using System.Collections.Generic;
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
        private readonly GameDatabase _gameDatabase;
        private readonly IDamageCalculator _damageCalculator;

        private Renderer renderer;

        public ulong EvaluationCount { get; private set; }
        public bool StopConditionSatisfied { get; private set; }

        public NeuromonEvaluator(TrainingGameSettings trainingGameSettings, double desiredFitness)
        {
            _trainingGameSettings = trainingGameSettings;
            _desiredFitness = desiredFitness;

            _opponentPlayerControllerFactory = new PlayerControllerFactory(_trainingGameSettings.OpponentBrainFileName);

            _gameDatabase = GameDatabase.CreateFromFiles(
                _trainingGameSettings.TypesFileName,
                _trainingGameSettings.MovesFileName,
                _trainingGameSettings.NeuromonFileName
            );

            _damageCalculator = new DamageCalculatorFactory(
                _trainingGameSettings.EffectiveMultiplier, _trainingGameSettings.WeakMultiplier,
                _trainingGameSettings.MinimumRandomMultiplier, _trainingGameSettings.MaximumRandomMultiplier,
                _trainingGameSettings.NonDeterministic
            ).Create();

            renderer = null;

            EvaluationCount = 0;
            StopConditionSatisfied = false;
        }

        private BattleSimulator CreateGame(IBlackBox brain, NeuromonCollection traineeNeuromon, NeuromonCollection opponentNeuromon)
        {
            var opponentState = new PlayerState(OpponentName, opponentNeuromon);
            var opponentController = _opponentPlayerControllerFactory.Create(_trainingGameSettings.OpponentType, opponentState);
            var opponent = new Player(opponentState, opponentController);

            var neatPlayerControllerFactory = new NeatAiPlayerControllerFactory(brain);

            var traineeState = new PlayerState(TraineeName, traineeNeuromon);
            var traineeController = neatPlayerControllerFactory.CreatePlayer(traineeState);
            var trainee = new Player(traineeState, traineeController);

            // TODO: Should the trainee player first or second?
            var battleSimulator = new BattleSimulator(opponent, trainee, _damageCalculator, false);

            if (_trainingGameSettings.ShouldRender)
            {
                renderer = new Renderer(battleSimulator);
            }

            return battleSimulator;
        }

        /**
        * TODO: Create combinations of possible games
        * TODO: Is the search space going to be too large?
        * Combinations of all possible neuromon collections for two players, e.g.
        * 5 possible Neuromon in the Database: A, B, C, D, E
        * Each player has 3 Neuromon
        * Player 1 may have 5 Choose 3 Neuromon = 10
        * For each of those, Player 2 may have each of the possible Neuromon collections, therefore 10 * 10 = 100 combinations
        * 
        */
        private static IList<Tuple<NeuromonCollection, NeuromonCollection>> CreateAllPossibleNeuromonCollectionCombinations()
        {
            throw new NotImplementedException();
        }

        public FitnessInfo Evaluate(IBlackBox phenome)
        {
            var allPossibleNeuromonCollectionCombinations = CreateAllPossibleNeuromonCollectionCombinations();

            var accumulatedFitness = 0.0;

            foreach (var possibleNeuromonCollectionCombination in allPossibleNeuromonCollectionCombinations)
            {
                // TODO: Should each possible game be played multiple times to counteract non-deterministic luck?

                var game = CreateGame(phenome, possibleNeuromonCollectionCombination.Item1,
                    possibleNeuromonCollectionCombination.Item2);

                var result = game.Run();
                accumulatedFitness += CalculateFitness(result);
            }

            // TODO: Should an average be taken, or should we just return the accumulated fitness?

            var averageFitness = accumulatedFitness / allPossibleNeuromonCollectionCombinations.Count;

            EvaluationCount++;
            StopConditionSatisfied = averageFitness >= _desiredFitness;

            return new FitnessInfo(averageFitness, averageFitness);
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