using System;
using System.Collections.Generic;
using System.Xml;
using Newtonsoft.Json;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;

namespace Player.AI.Neat.Trainer
{
    internal sealed class Train
    {
        // TODO: Remove XML configuration
        private const string NeuromonTrainingConfigFile = "NeuromonTrainingConfig.xml";

        private const string NeuromonExperimentSettingsFileName = "ExperimentSettings.json";
        private const string EvolutionAlgorithmParametersFileName = "EvolutionAlgorithmParameters.json";
        private const string TrainingGameSettingsFileName = "TrainingGameSettings.json";

        private static void Main()
        {
            var neuromonExperimentSettings = JsonConvert.DeserializeObject<ExperimentSettings>(NeuromonExperimentSettingsFileName);
            var evolutionAlgorithmParameters = JsonConvert.DeserializeObject<NeatEvolutionAlgorithmParameters>(EvolutionAlgorithmParametersFileName);
            var trainingGameSettings = JsonConvert.DeserializeObject<TrainingGameSettings>(TrainingGameSettingsFileName);

            var neuromonPhenomeEvaluator = new NeuromonEvaluator(trainingGameSettings, neuromonExperimentSettings.DesiredFitness);
            var neatGenomeParameters = new NeatGenomeParameters();

            var neuromonExperiment = new NeuromonExperiment(neuromonExperimentSettings, evolutionAlgorithmParameters, neuromonPhenomeEvaluator, neatGenomeParameters);

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
            neuromonExperiment.Initialize(neuromonExperimentSettings.ExperimentName, xmlConfig.DocumentElement);

            var evolutionAlgorithm = neuromonExperiment.CreateEvolutionAlgorithm(genomeFactory, genomePopulation);

            evolutionAlgorithm.UpdateEvent += (sender, args) =>
            {
                Console.WriteLine($"Generation: {evolutionAlgorithm.CurrentGeneration}, Best Fitness: {evolutionAlgorithm.Statistics._maxFitness}");
            };

            Console.WriteLine("Press Enter to Quit Training");

            evolutionAlgorithm.StartContinue();

            Console.ReadLine();
            Console.WriteLine("Stopping and Saving...");

            evolutionAlgorithm.RequestPauseAndWait();

            // Save population and champion genome
            var writerSettings = new XmlWriterSettings { Indent = true };
            using (var xmlWriter = XmlWriter.Create(neuromonExperimentSettings.OutputPopulationFilePath, writerSettings))
            {
                neuromonExperiment.SavePopulation(xmlWriter, genomePopulation);
            }

            using (var xmlWriter = XmlWriter.Create(neuromonExperimentSettings.OutputChampionFilePath, writerSettings))
            {
                var champion = new List<NeatGenome> { evolutionAlgorithm.CurrentChampGenome };
                neuromonExperiment.SavePopulation(xmlWriter, champion);
            }
        }
    }
}
