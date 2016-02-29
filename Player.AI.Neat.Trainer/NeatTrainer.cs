using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using SharpNeat.Core;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using static Player.AI.Neat.Trainer.NeatTrainerDelegates;

namespace Player.AI.Neat.Trainer
{
    public sealed class NeatTrainer
    {
        // TODO: Remove XML configuration
        private const string NeuromonTrainingConfigFile = "NeuromonTrainingConfig.xml";

        private readonly NeuromonExperiment _neuromonExperiment;
        private readonly IGenomeFactory<NeatGenome> _genomeFactory;
        private readonly List<NeatGenome> _genomePopulation;

        private NeatEvolutionAlgorithm<NeatGenome> _evolutionAlgorithm;

        public event StatusUpdateDelegate OnStatusUpdate;
        public event TrainingPausedDelegate OnTrainingPaused;

        public NeatTrainer(ExperimentSettings experimentSettings, NeatEvolutionAlgorithmParameters evolutionAlgorithmParameters, TrainingGameSettings gameSettings)
        {
            var neuromonPhenomeEvaluator = new NeuromonEvaluator(gameSettings, experimentSettings);
            var neatGenomeParameters = new NeatGenomeParameters();

            _neuromonExperiment = new NeuromonExperiment(experimentSettings, evolutionAlgorithmParameters, neuromonPhenomeEvaluator, neatGenomeParameters);

            _genomeFactory = _neuromonExperiment.CreateGenomeFactory();

            if (experimentSettings.LoadExistingPopulation)
            {
                using (var xmlReader = XmlReader.Create(experimentSettings.ExistingPopulationFilePath))
                {
                    _genomePopulation = _neuromonExperiment.LoadPopulation(xmlReader);
                }
            }
            else
            {
                // Randomly generate a new population
                _genomePopulation = _genomeFactory.CreateGenomeList(experimentSettings.PopulationSize, 0);
            }

            // TODO: Remove initialisation
            var xmlConfig = new XmlDocument();
            xmlConfig.Load(NeuromonTrainingConfigFile);
            _neuromonExperiment.Initialize(experimentSettings.ExperimentName, xmlConfig.DocumentElement);
        }

        public void StartTraining()
        {
            if (_evolutionAlgorithm == null)
            {
                _evolutionAlgorithm = _neuromonExperiment.CreateEvolutionAlgorithm(_genomeFactory, _genomePopulation);
                _evolutionAlgorithm.UpdateEvent += (sender, e) => OnStatusUpdate?.Invoke(_evolutionAlgorithm.CurrentGeneration, _evolutionAlgorithm.Statistics._maxFitness);
                _evolutionAlgorithm.PausedEvent += (sender, args) => OnTrainingPaused?.Invoke();
            }

            _evolutionAlgorithm.StartContinue();   
        }

        public void StopTraining()
        {
            _evolutionAlgorithm?.RequestPause();
        }

        public void SavePopulation(string filePath)
        {
            SaveToXml(filePath, xmlWriter => _neuromonExperiment.SavePopulation(xmlWriter, _genomePopulation));
        }

        public void SaveChampionGenome(string filePath)
        {
            SaveToXml(filePath, xmlWriter =>
            {
                var champion = new List<NeatGenome> { _evolutionAlgorithm?.CurrentChampGenome ?? _genomePopulation.OrderByDescending(g => g.EvaluationInfo.Fitness).First() };
                _neuromonExperiment.SavePopulation(xmlWriter, champion);
            });
        }

        private static void SaveToXml(string filePath, Action<XmlWriter> func)
        {
            var writerSettings = new XmlWriterSettings { Indent = true };
            using (var xmlWriter = XmlWriter.Create(filePath, writerSettings))
            {
                func(xmlWriter);
            }
        }
    }
}