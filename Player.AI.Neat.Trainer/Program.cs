using System;
using System.IO;
using Newtonsoft.Json;
using SharpNeat.EvolutionAlgorithms;

namespace Player.AI.Neat.Trainer
{
    internal sealed class Program
    {
        private const string NeuromonExperimentSettingsFileName = "Config/ExperimentSettings.json";
        private const string EvolutionAlgorithmParametersFileName = "Config/EvolutionAlgorithmParameters.json";
        private const string TrainingGameSettingsFileName = "Config/TrainingGameSettings.json";

        private static void Main()
        {
            var neuromonExperimentSettingsJson = File.ReadAllText(NeuromonExperimentSettingsFileName);
            var evolutionAlgorithmParametersJson = File.ReadAllText(EvolutionAlgorithmParametersFileName);
            var trainingGameSettingsJson = File.ReadAllText(TrainingGameSettingsFileName);

            var neuromonExperimentSettings = JsonConvert.DeserializeObject<ExperimentSettings>(neuromonExperimentSettingsJson);
            var evolutionAlgorithmParameters = JsonConvert.DeserializeObject<NeatEvolutionAlgorithmParameters>(evolutionAlgorithmParametersJson);
            var trainingGameSettings = JsonConvert.DeserializeObject<TrainingGameSettings>(trainingGameSettingsJson);

            var neatTrainer = new NeatTrainer(neuromonExperimentSettings, evolutionAlgorithmParameters, trainingGameSettings);

            neatTrainer.OnStatusUpdate += (generation, fitness) =>
            {
                Console.WriteLine($"Generation: {generation}, Best Fitness: {fitness}");
            };

            var trainingStopped = false;

            neatTrainer.OnStagnationDetected += () => Console.WriteLine("Stagnation Detected.");

            if (neuromonExperimentSettings.StopTrainingOnStagnationDetection)
            {
                neatTrainer.OnStagnationDetected += () =>
                {
                    Console.WriteLine("Stopping training due to stagnation detection.");
                    Console.WriteLine("Press Enter to save results.");

                    neatTrainer.StopTraining();
                    trainingStopped = true;
                };
            }

            Console.WriteLine("Press Enter to Quit Training");

            neatTrainer.StartTraining();

            Console.ReadLine();

            if (!trainingStopped)
            {
                neatTrainer.StopTraining();
                trainingStopped = true;
            }

            Console.WriteLine("Saving...");

            neatTrainer.SavePopulation(neuromonExperimentSettings.OutputPopulationFilePath);
            neatTrainer.SaveChampionGenome(neuromonExperimentSettings.OutputChampionFilePath);
        }
    }
}
