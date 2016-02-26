using System.IO;
using Newtonsoft.Json;
using SharpNeat.EvolutionAlgorithms;

namespace Player.AI.Neat.Trainer.Gui
{
    internal sealed class JsonSettingsReader
    {
        private const string NeuromonExperimentSettingsFileName = "ExperimentSettings.json";
        private const string EvolutionAlgorithmParametersFileName = "EvolutionAlgorithmParameters.json";
        private const string TrainingGameSettingsFileName = "TrainingGameSettings.json";

        public ExperimentSettings ReadExperimentSettings()
        {
            var experimentSettingsJson = File.ReadAllText(NeuromonExperimentSettingsFileName);
            return JsonConvert.DeserializeObject<ExperimentSettings>(experimentSettingsJson);
        }

        public NeatEvolutionAlgorithmParameters ReadEvolutionAlgorithmParameters()
        {

            var evolutionAlgorithmParametersJson = File.ReadAllText(EvolutionAlgorithmParametersFileName);
            return JsonConvert.DeserializeObject<NeatEvolutionAlgorithmParameters>(evolutionAlgorithmParametersJson);
        }

        public TrainingGameSettings ReadTrainingGameSettings()
        {
            var trainingGameSettingsJson = File.ReadAllText(TrainingGameSettingsFileName);
            return JsonConvert.DeserializeObject<TrainingGameSettings>(trainingGameSettingsJson);
        }
    }
}