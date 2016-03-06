using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        private uint _previousGeneration;
        private double _overallBestFitness;
        private string _currentChampionGenomeXml;

        public event StatusUpdateDelegate OnStatusUpdate;
        public event TrainingPausedDelegate OnTrainingPaused;
        public event StagnationDetectedDelegate OnStagnationDetected;
        public event HighestFitnessAchievedDelegate OnHighestFitnessAchievedDelegate;

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

            _previousGeneration = 0;
            _overallBestFitness = 0.0;
            _currentChampionGenomeXml = "";
        }

        public void StartTraining()
        {
            if (_evolutionAlgorithm == null)
            {
                _evolutionAlgorithm = _neuromonExperiment.CreateEvolutionAlgorithm(_genomeFactory, _genomePopulation);
                _evolutionAlgorithm.UpdateEvent += (sender, e) => OnStatusUpdate?.Invoke(_evolutionAlgorithm.CurrentGeneration, _evolutionAlgorithm.Statistics._maxFitness);
                _evolutionAlgorithm.UpdateEvent += (sender, args) => HandleUpdateEvent(_evolutionAlgorithm.CurrentGeneration, _evolutionAlgorithm.Statistics._maxFitness);
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
            var genomesXml = FormatGenomesToXml(_genomePopulation);
            WriteToFile(filePath, genomesXml);
        }

        public void SaveChampionGenome(string filePath)
        {
            WriteToFile(filePath, _currentChampionGenomeXml);
        }

        private string FormatGenomesToXml(NeatGenome genome)
        {
            return FormatGenomesToXml(new List<NeatGenome> { genome });
        }

        private string FormatGenomesToXml(IList<NeatGenome> genomes)
        {
            var sb = new StringBuilder();

            var writerSettings = new XmlWriterSettings { Indent = true };
            using (var xmlWriter = XmlWriter.Create(sb, writerSettings))
            {
                _neuromonExperiment.SavePopulation(xmlWriter, genomes);
                xmlWriter.Flush();
            }

            return sb.ToString();
        }

        private static void WriteToFile(string filePath, string content)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, content);
        }

        private void HandleUpdateEvent(uint generation, double generationBestFitness)
        {
            // SharpNEAT will sometimes trigger an update event multiple times for the same generation.
            // We want to ignore these instances.
            if (_previousGeneration == generation)
            {
                return;
            }

            if (generationBestFitness > _overallBestFitness)
            {
                // Cache the XML of the current champion genome
                // This is necessary as further iterations may result in a lower fitness
                _currentChampionGenomeXml = FormatGenomesToXml(_evolutionAlgorithm?.CurrentChampGenome);
                _overallBestFitness = generationBestFitness;

                OnHighestFitnessAchievedDelegate?.Invoke(_overallBestFitness);
            }

            _previousGeneration = generation;
            _fitnessStagnationDetector.Add(generationBestFitness);

            if (_fitnessStagnationDetector.HasFitnessStagnated())
            {
                OnStagnationDetected?.Invoke();
            }
        }
    }
}