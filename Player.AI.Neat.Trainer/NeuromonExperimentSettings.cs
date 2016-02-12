using SharpNeat.Core;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;

namespace Player.AI.Neat.Trainer
{
    internal sealed class NeuromonExperimentSettings
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
        public IPhenomeEvaluator<IBlackBox> PhenomeEvaluator { get; set; } 
        public NeatEvolutionAlgorithmParameters NeatEvolutionAlgorithmParameters { get; set; }
        public NeatGenomeParameters NeatGenomeParameters { get; set; }
    }
}