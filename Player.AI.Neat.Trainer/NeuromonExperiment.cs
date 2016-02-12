using System;
using System.Collections.Generic;
using System.Xml;
using SharpNeat.Core;
using SharpNeat.Domains;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;

namespace Player.AI.Neat.Trainer
{
    internal sealed class NeuromonExperiment : INeatExperiment
    {
        private readonly IPhenomeEvaluator<IBlackBox> _phenomeEvaluator;

        public string Name { get; }
        public string Description { get; }
        public int InputCount { get; }
        public int OutputCount { get; }
        public int DefaultPopulationSize { get; }

        public NeatEvolutionAlgorithmParameters NeatEvolutionAlgorithmParameters { get; }
        public NeatGenomeParameters NeatGenomeParameters { get; }

        public NeuromonExperiment(NeuromonExperimentSettings experimentSettings)
        {
            Name = experimentSettings.ExperimentName;
            Description = experimentSettings.Description;

            InputCount = experimentSettings.InputCount;
            OutputCount = experimentSettings.OutputCount;
            DefaultPopulationSize = experimentSettings.PopulationSize;
            _phenomeEvaluator = experimentSettings.PhenomeEvaluator;

            NeatEvolutionAlgorithmParameters = experimentSettings.NeatEvolutionAlgorithmParameters;
            NeatGenomeParameters = experimentSettings.NeatGenomeParameters;
        }

        public void Initialize(string name, XmlElement xmlConfig)
        {
            throw new NotImplementedException();
        }

        public List<NeatGenome> LoadPopulation(XmlReader xr)
        {
            throw new NotImplementedException();
        }

        public void SavePopulation(XmlWriter xw, IList<NeatGenome> genomeList)
        {
            throw new NotImplementedException();
        }

        public IGenomeDecoder<NeatGenome, IBlackBox> CreateGenomeDecoder()
        {
            throw new NotImplementedException();
        }

        public IGenomeFactory<NeatGenome> CreateGenomeFactory()
        {
            throw new NotImplementedException();
        }

        public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm()
        {
            throw new NotImplementedException();
        }

        public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(int populationSize)
        {
            throw new NotImplementedException();
        }

        public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(IGenomeFactory<NeatGenome> genomeFactory, List<NeatGenome> genomeList)
        {
            throw new NotImplementedException();
        }
    }
}