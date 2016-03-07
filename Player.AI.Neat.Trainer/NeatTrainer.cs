using System.Collections.Generic;
using System.Linq;
using SharpNeat.Core;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using static Player.AI.Neat.Trainer.NeatTrainerDelegates;

namespace Player.AI.Neat.Trainer
{
    public sealed class NeatTrainer
    {
        private readonly NeuromonExperiment _neuromonExperiment;
        private readonly GenomeIo _genomeIo;
        private readonly IGenomeFactory<NeatGenome> _genomeFactory;
        private readonly List<NeatGenome> _genomePopulation;
        private readonly FitnessStagnationDetector _fitnessStagnationDetector;
        private readonly double _desiredFitness;

        private NeatEvolutionAlgorithm<NeatGenome> _evolutionAlgorithm;

        private uint _previousGeneration;
        private double _overallBestFitness;

        public event StatusUpdateDelegate OnStatusUpdate;
        public event TrainingPausedDelegate OnTrainingPaused;
        public event StagnationDetectedDelegate OnStagnationDetected;
        public event HighestFitnessAchievedDelegate OnHighestFitnessAchieved;
        public event DesiredFitnessAchievedDelegate OnDesiredFitnessAchieved;

        public NeatTrainer(ExperimentSettings experimentSettings, NeatEvolutionAlgorithmParameters evolutionAlgorithmParameters, TrainingGameSettings gameSettings)
        {
            var neuromonPhenomeEvaluator = new NeuromonEvaluator(gameSettings, experimentSettings);
            var neatGenomeParameters = new NeatGenomeParameters();

            _neuromonExperiment = new NeuromonExperiment(experimentSettings, evolutionAlgorithmParameters, neuromonPhenomeEvaluator, neatGenomeParameters);

            _genomeIo = new GenomeIo(_neuromonExperiment);
            _genomeFactory = _neuromonExperiment.CreateGenomeFactory();

            if (experimentSettings.LoadExistingPopulation)
            {
                _genomePopulation = _genomeIo.Read(experimentSettings.ExistingPopulationFilePath);
            }
            else
            {
                // Randomly generate a new population
                _genomePopulation = _genomeFactory.CreateGenomeList(experimentSettings.PopulationSize, 0);
            }

            _genomeIo.CacheChampion(_genomePopulation.OrderByDescending(g => g.EvaluationInfo.Fitness).First());

            _fitnessStagnationDetector = new FitnessStagnationDetector(experimentSettings.StagnationDetectionTriggerValue);

            _desiredFitness = experimentSettings.DesiredFitness;

            _previousGeneration = 0;
            _overallBestFitness = 0.0;
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
            _genomeIo.Write(filePath, _genomePopulation);
        }

        public void SaveChampionGenome(string filePath)
        {
            _genomeIo.WriteChampion(filePath);
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
                _genomeIo.CacheChampion(_evolutionAlgorithm?.CurrentChampGenome);

                _overallBestFitness = generationBestFitness;

                OnHighestFitnessAchieved?.Invoke(_overallBestFitness);

                if (_overallBestFitness >= _desiredFitness)
                {
                    OnDesiredFitnessAchieved?.Invoke();
                }
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