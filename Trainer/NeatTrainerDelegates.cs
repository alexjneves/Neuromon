namespace Trainer
{
    public sealed class NeatTrainerDelegates
    {
        public delegate void StatusUpdateDelegate(uint generation, double highestFitness, double averageFitness);
        public delegate void TrainingPausedDelegate();
        public delegate void StagnationDetectedDelegate();
        public delegate void HighestFitnessAchievedDelegate(double fitness);
        public delegate void DesiredFitnessAchievedDelegate();
    }
}