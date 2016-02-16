namespace Player.AI.Neat.Trainer
{
    internal sealed class ExperimentSettings
    {
        public string ExperimentName { get; set; }
        public string Description { get; set; }
        public int InputCount { get; set; }
        public int OutputCount { get; set; }
        public int PopulationSize { get; set; }
        public bool LoadExistingPopulation { get; set; }
        public string ExistingPopulationFilePath { get; set; }
        public string OutputPopulationFilePath { get; set; }
        public string OutputChampionFilePath { get; set; }
        public string ComplexityRegulationStrategy { get; set; }
        public int ComplexityThreshold { get; set; }
        public double DesiredFitness { get; set; }
        public int GameCombinationIterations { get; set; }
    }
}