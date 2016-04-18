using System;
using System.IO;
using Newtonsoft.Json;
using SharpNeat.EvolutionAlgorithms;

namespace Player.AI.Neat.Trainer.Console
{
    internal sealed class Program
    {
        private const string NeuromonExperimentSettingsFileName = "Config/ExperimentSettings.json";
        private const string EvolutionAlgorithmParametersFileName = "Config/EvolutionAlgorithmParameters.json";
        private const string TrainingGameSettingsFileName = "Config/TrainingGameSettings.json";

        private const ConsoleKey ResumeKey = ConsoleKey.R;
        private const ConsoleKey PauseKey = ConsoleKey.P;
        private const ConsoleKey QuitKey = ConsoleKey.Enter;

        private static volatile bool _trainingStopped;
        private static volatile bool _quit;

        private static double _highestFitness;

        private static void Main()
        {
            var neuromonExperimentSettingsJson = File.ReadAllText(NeuromonExperimentSettingsFileName);
            var evolutionAlgorithmParametersJson = File.ReadAllText(EvolutionAlgorithmParametersFileName);
            var trainingGameSettingsJson = File.ReadAllText(TrainingGameSettingsFileName);

            var experimentSettings = JsonConvert.DeserializeObject<ExperimentSettings>(neuromonExperimentSettingsJson);
            var evolutionAlgorithmParameters =
                JsonConvert.DeserializeObject<NeatEvolutionAlgorithmParameters>(evolutionAlgorithmParametersJson);
            var trainingGameSettings = JsonConvert.DeserializeObject<TrainingGameSettings>(trainingGameSettingsJson);

            var neatTrainer = new NeatTrainer(experimentSettings, evolutionAlgorithmParameters, trainingGameSettings);

            neatTrainer.OnStatusUpdate += (generation, highestFitness, averageFitness) =>
            {
                if (!_quit)
                {
                    System.Console.WriteLine($"Generation: {generation}, Best Fitness: {highestFitness:000.000}, Average Fitness: {averageFitness:000.000}");
                    _trainingStopped = false;
                }
            };

            neatTrainer.OnTrainingPaused += () =>
            {
                System.Console.WriteLine($"Training has been paused. Press {ResumeKey} to resume.");
                _trainingStopped = true;
            };

            if (experimentSettings.StopTrainingOnStagnationDetection)
            {
                neatTrainer.OnStagnationDetected += () =>
                {
                    System.Console.WriteLine("Stagnation detected, stopping training...");
                    neatTrainer.StopTraining();
                };
            }
            else
            {
                neatTrainer.OnStagnationDetected += () => System.Console.WriteLine("Stagnation detected.");
            }

            neatTrainer.OnHighestFitnessAchieved += fitness =>
            {
                _highestFitness = fitness;
            };

            neatTrainer.OnDesiredFitnessAchieved += () =>
            {
                System.Console.WriteLine(
                    $"Desired fitness of {experimentSettings.DesiredFitness} has been achieved. Training will be stopped.");
            };

            System.Console.WriteLine($"Press {QuitKey} to quit, {PauseKey} to pause and {ResumeKey} to resume training.");

            neatTrainer.StartTraining();

            var keyPressed = ResumeKey;
            while (keyPressed != QuitKey)
            {
                keyPressed = System.Console.ReadKey().Key;
                System.Console.WriteLine();

                switch (keyPressed)
                {
                    case ResumeKey:
                    {
                        if (_trainingStopped)
                        {
                            System.Console.WriteLine("Resuming training...");
                            neatTrainer.StartTraining();
                        }
                        else
                        {
                            System.Console.WriteLine("Already training.");
                        }
                    }
                    break;
                    case PauseKey:
                    {
                        if (!_trainingStopped)
                        {
                            System.Console.WriteLine("Stopping training...");
                            neatTrainer.StopTraining();
                        }
                        else
                        {
                            System.Console.WriteLine("Already stopped.");
                        }
                    }
                    break;
                    case QuitKey:
                    {
                        _quit = true;
                        System.Console.WriteLine("Quitting training...");

                        if (!_trainingStopped)
                        {
                            neatTrainer.StopTraining();
                        }
                    }
                    break;
                }
            }

            System.Console.WriteLine("Saving...");

            neatTrainer.SavePopulation(experimentSettings.OutputPopulationFilePath);
            neatTrainer.SaveChampionGenome(experimentSettings.OutputChampionFilePath);

            System.Console.WriteLine("Saved.");

            System.Console.WriteLine();
            System.Console.WriteLine($"Highest fitness achieved: {_highestFitness}");

            System.Console.ReadLine();
        }
    }
}
