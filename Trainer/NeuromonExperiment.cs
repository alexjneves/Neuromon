using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using SharpNeat.Core;
using SharpNeat.Decoders;
using SharpNeat.Decoders.Neat;
using SharpNeat.DistanceMetrics;
using SharpNeat.Domains;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using SharpNeat.SpeciationStrategies;

namespace Trainer
{
    internal sealed class NeuromonExperiment : INeatExperiment
    {
        private readonly IPhenomeEvaluator<IBlackBox> _neuromonPhenomeEvaluator;
        private readonly string _complexityRegulationStrategy;
        private readonly int _complexityThreshold;

        private readonly NetworkActivationScheme _networkActivationScheme;

        public string Name { get; }
        public string Description { get; }
        public int InputCount { get; }
        public int OutputCount { get; }
        public int DefaultPopulationSize { get; }

        public NeatEvolutionAlgorithmParameters NeatEvolutionAlgorithmParameters { get; }
        public NeatGenomeParameters NeatGenomeParameters { get; }

        public NeuromonExperiment(ExperimentSettings experimentSettings, NeatEvolutionAlgorithmParameters evolutionAlgorithmParameters, IPhenomeEvaluator<IBlackBox> neuromonPhenomeEvaluator, NeatGenomeParameters genomeParameters)
        {
            Name = experimentSettings.ExperimentName;
            Description = experimentSettings.Description;
            InputCount = experimentSettings.InputCount;
            OutputCount = experimentSettings.OutputCount;
            DefaultPopulationSize = experimentSettings.PopulationSize;

            _neuromonPhenomeEvaluator = neuromonPhenomeEvaluator;
            _complexityRegulationStrategy = experimentSettings.ComplexityRegulationStrategy;
            _complexityThreshold = experimentSettings.ComplexityThreshold;

            // Removed from .xml configuration. TODO: Possibly make configurable via json config
            _networkActivationScheme = NetworkActivationScheme.CreateCyclicFixedTimestepsScheme(1);

            NeatEvolutionAlgorithmParameters = evolutionAlgorithmParameters;
            NeatGenomeParameters = genomeParameters;
        }

        public void Initialize(string name, XmlElement xmlConfig)
        {
        }

        public List<NeatGenome> LoadPopulation(XmlReader xmlReader)
        {
            var genomeFactory = (NeatGenomeFactory) CreateGenomeFactory();
            return NeatGenomeXmlIO.ReadCompleteGenomeList(xmlReader, false, genomeFactory);
        }

        public void SavePopulation(XmlWriter xmlWriter, IList<NeatGenome> genomePopulation)
        {
            NeatGenomeXmlIO.WriteComplete(xmlWriter, genomePopulation, false);
        }

        public IGenomeDecoder<NeatGenome, IBlackBox> CreateGenomeDecoder()
        {
            return new NeatGenomeDecoder(_networkActivationScheme);
        }

        public IGenomeFactory<NeatGenome> CreateGenomeFactory()
        {
            return new NeatGenomeFactory(InputCount, OutputCount, NeatGenomeParameters);
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
            var parallelOptions = new ParallelOptions();

            // Create distance metric. Mismatched genes have a fixed distance of 10; for matched genes the distance is their weigth difference.
            var distanceMetric = new ManhattanDistanceMetric(1.0, 0.0, 10.0);
            var speciationStrategy = new ParallelKMeansClusteringStrategy<NeatGenome>(distanceMetric, parallelOptions);

            // Create complexity regulation strategy.
            var complexityRegulationStrategy = ExperimentUtils.CreateComplexityRegulationStrategy(_complexityRegulationStrategy, _complexityThreshold);

            // Create the evolution algorithm.
            var evolutionAlgorithm = new NeatEvolutionAlgorithm<NeatGenome>(NeatEvolutionAlgorithmParameters, speciationStrategy, complexityRegulationStrategy);

            // Create genome decoder.
            var genomeDecoder = CreateGenomeDecoder();

            // Create a genome list evaluator. This packages up the genome decoder with the genome evaluator.
            var genomeListEvaluator = new ParallelGenomeListEvaluator<NeatGenome, IBlackBox>(genomeDecoder, _neuromonPhenomeEvaluator, parallelOptions);

            // Initialize the evolution algorithm.
            evolutionAlgorithm.Initialize(genomeListEvaluator, genomeFactory, genomeList);

            return evolutionAlgorithm;
        }
    }
}