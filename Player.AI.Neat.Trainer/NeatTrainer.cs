using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly NeuromonExperiment _neuromonExperiment;
        private readonly IGenomeFactory<NeatGenome> _genomeFactory;
        private readonly List<NeatGenome> _genomePopulation;
        private readonly FitnessStagnationDetector _fitnessStagnationDetector;

        private NeatEvolutionAlgorithm<NeatGenome> _evolutionAlgorithm;

        public event StatusUpdateDelegate OnStatusUpdate;
        public event TrainingPausedDelegate OnTrainingPaused;
        public event StagnationDetectedDelegate OnStagnationDetected;

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

            _fitnessStagnationDetector = new FitnessStagnationDetector(experimentSettings.StagnationDetectionTriggerValue);
        }

        public void StartTraining()
        {
            if (_evolutionAlgorithm == null)
            {
                _evolutionAlgorithm = _neuromonExperiment.CreateEvolutionAlgorithm(_genomeFactory, _genomePopulation);
                _evolutionAlgorithm.UpdateEvent += (sender, e) => OnStatusUpdate?.Invoke(_evolutionAlgorithm.CurrentGeneration, _evolutionAlgorithm.Statistics._maxFitness);
                _evolutionAlgorithm.PausedEvent += (sender, args) => OnTrainingPaused?.Invoke();
                _evolutionAlgorithm.UpdateEvent += (sender, args) => HandleStagnationDetection(_evolutionAlgorithm.Statistics._maxFitness);
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
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            var writerSettings = new XmlWriterSettings { Indent = true };
            using (var xmlWriter = XmlWriter.Create(filePath, writerSettings))
            {
                func(xmlWriter);
            }
        }

        private void HandleStagnationDetection(double currentBestFitness)
        {
            _fitnessStagnationDetector.Add(currentBestFitness);

            if (_fitnessStagnationDetector.HasFitnessStagnated())
            {
                OnStagnationDetected?.Invoke();
            }
        }
    }
}