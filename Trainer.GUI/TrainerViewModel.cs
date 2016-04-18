using SharpNeat.EvolutionAlgorithms;

namespace Player.AI.Neat.Trainer.Gui
{
    public class TrainerViewModel
    {
        public TrainingGameSettings TrainingGameSettings { get; set; }
        public ExperimentSettings ExperimentSettings { get; set; }
        public NeatEvolutionAlgorithmParameters NeatEvolutionAlgorithmParameters { get; set; }
    }
}