using System;
using System.Collections.Generic;
using System.Xml;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;

namespace Player.AI.Neat.Trainer
{
    internal sealed class Train
    {
        // TODO: Read following settings from config
        private const string ExperimentName = "Neuromon";
        private const string NeuromonTrainingConfigFile = "NeuromonTrainingConfig.xml";

        private static NeatEvolutionAlgorithm<NeatGenome> _evolutionAlgorithm;

        private static void Main()
        {
            // TODO: Read from JSON file
            var neuromonExperimentSettings = new NeuromonExperimentSettings
            {
                ExperimentName = "Neuromon Neat AI Experiment",
                Description = "",
                InputCount = 0,
                OutputCount = 0,
                PopulationSize = 0,
                LoadExistingPopulation = false,
                ExistingPopulationFilePath = "",
                OutputPopulationFilePath = "NeuromonGenomePopulation.xml",
                OutputChampionFilePath = "NeuromonChampion.xml",
                PhenomeEvaluator = new NeuromonEvaluator(),
                NeatEvolutionAlgorithmParameters = new NeatEvolutionAlgorithmParameters { SpecieCount = 10 },
                NeatGenomeParameters = new NeatGenomeParameters()
            };

            var neuromonExperiment = new NeuromonExperiment(neuromonExperimentSettings);

            var genomeFactory = neuromonExperiment.CreateGenomeFactory();
            List<NeatGenome> genomePopulation;

            if (neuromonExperimentSettings.LoadExistingPopulation)
            {
                using (var xmlReader = XmlReader.Create(neuromonExperimentSettings.ExistingPopulationFilePath))
                {
                    genomePopulation = neuromonExperiment.LoadPopulation(xmlReader);
                }
            }
            else
            {
                // Randomly generate a new population
                genomePopulation = genomeFactory.CreateGenomeList(neuromonExperimentSettings.PopulationSize, 0);
            }

            // TODO: Remove initialisation
            var xmlConfig = new XmlDocument();
            xmlConfig.Load(NeuromonTrainingConfigFile);
            neuromonExperiment.Initialize(ExperimentName, xmlConfig.DocumentElement);

            _evolutionAlgorithm = neuromonExperiment.CreateEvolutionAlgorithm(genomeFactory, genomePopulation);

            _evolutionAlgorithm = neuromonExperiment.CreateEvolutionAlgorithm();
            _evolutionAlgorithm.UpdateEvent += (sender, args) =>
            {
                Console.WriteLine($"Generation: {_evolutionAlgorithm.CurrentGeneration}, Best Fitness: {_evolutionAlgorithm.Statistics._maxFitness}");
            };

            Console.WriteLine("Press Enter to Quit Training");

            _evolutionAlgorithm.StartContinue();

            Console.ReadLine();
            Console.WriteLine("Stopping and Saving...");

            _evolutionAlgorithm.RequestPauseAndWait();

            // Save population and champion genome
            var writerSettings = new XmlWriterSettings { Indent = true };
            using (var xmlWriter = XmlWriter.Create(neuromonExperimentSettings.OutputPopulationFilePath, writerSettings))
            {
                neuromonExperiment.SavePopulation(xmlWriter, genomePopulation);
            }

            using (var xmlWriter = XmlWriter.Create(neuromonExperimentSettings.OutputChampionFilePath, writerSettings))
            {
                var champion = new List<NeatGenome> { _evolutionAlgorithm.CurrentChampGenome };
                neuromonExperiment.SavePopulation(xmlWriter, champion);
            }
        }
    }
}
