using SharpNeat.EvolutionAlgorithms;

namespace Trainer.GUI
{
    public class TrainerViewModel
    {
        public TrainingGameSettings TrainingGameSettings { get; set; }
        public ExperimentSettings ExperimentSettings { get; set; }
        public NeatEvolutionAlgorithmParameters NeatEvolutionAlgorithmParameters { get; set; }
    }
}