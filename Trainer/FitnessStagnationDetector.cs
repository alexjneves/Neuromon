using System;
using System.Linq;

namespace Player.AI.Neat.Trainer
{
    internal sealed class FitnessStagnationDetector
    {
        private const int MinimumTriggerValue = 2;
        private const double FitnessComparisonTolerance = 0.0001;

        private readonly int _triggerValue;
        private readonly CircularBuffer<double> _fitnessBuffer;

        private int _addCounter;

        public FitnessStagnationDetector(int triggerValue)
        {
            if (triggerValue < MinimumTriggerValue)
            {
                throw new Exception($"Trigger value must be >= {MinimumTriggerValue}");
            }

            _triggerValue = triggerValue;
            _fitnessBuffer = new CircularBuffer<double>(_triggerValue);

            _addCounter = 0;
        }

        public bool HasFitnessStagnated()
        {
            if (!HasBufferBeenFilled())
            {
                return false;
            }

            return _fitnessBuffer.All(fitness => Math.Abs(fitness - _fitnessBuffer.First()) < FitnessComparisonTolerance);
        }

        public void Add(double fitnessValue)
        {
            if (!HasBufferBeenFilled())
            {
                _addCounter++;
            }

            _fitnessBuffer.Add(fitnessValue);
        }

        private bool HasBufferBeenFilled() => _addCounter >= _triggerValue;
    }
}