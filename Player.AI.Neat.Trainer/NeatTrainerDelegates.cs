namespace Player.AI.Neat.Trainer
{
    public sealed class NeatTrainerDelegates
    {
        public delegate void StatusUpdateDelegate(uint generation, double highestFitness);
        public delegate void TrainingPausedDelegate();
        public delegate void StagnationDetectedDelegate();
    }
}