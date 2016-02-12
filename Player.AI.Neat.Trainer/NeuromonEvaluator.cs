using System;
using SharpNeat.Core;
using SharpNeat.Phenomes;

namespace Player.AI.Neat.Trainer
{
    internal sealed class NeuromonEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        public ulong EvaluationCount { get; private set; }
        public bool StopConditionSatisfied { get; private set; }

        public NeuromonEvaluator()
        {
            EvaluationCount = 0;
            StopConditionSatisfied = false;
        }

        public FitnessInfo Evaluate(IBlackBox phenome)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}