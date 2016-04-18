using System.IO;
using Newtonsoft.Json;
using SharpNeat.EvolutionAlgorithms;

namespace Trainer.GUI
{
    internal sealed class JsonSettingsIO
    {
        private const string FolderPrefix = "Config/";
        private const string DefaultPrefix = "Default/Default";

        private const string ExperimentSettingsFileName = "ExperimentSettings.json";
        private const string EvolutionAlgorithmParametersFileName = "EvolutionAlgorithmParameters.json";
        private const string TrainingGameSettingsFileName = "TrainingGameSettings.json";

        public ExperimentSettings ReadExperimentSettings()
        {
            var experimentSettingsFilePath = $"{FolderPrefix}{ExperimentSettingsFileName}";

            return ReadSettings<ExperimentSettings>(experimentSettingsFilePath);
        }

        public ExperimentSettings ReadDefaultExperimentSettings()
        {
            var defaultExperimentSettingsFilePath = $"{FolderPrefix}{DefaultPrefix}{ExperimentSettingsFileName}";

            return ReadSettings<ExperimentSettings>(defaultExperimentSettingsFilePath);
        }

        public NeatEvolutionAlgorithmParameters ReadEvolutionAlgorithmParameters()
        {
            var evolutionAlgorithmParametersFilePath = $"{FolderPrefix}{EvolutionAlgorithmParametersFileName}";

            return ReadSettings<NeatEvolutionAlgorithmParameters>(evolutionAlgorithmParametersFilePath);
        }

        public NeatEvolutionAlgorithmParameters ReadDefaultEvolutionAlgorithmParameters()
        {
            var defaultEvolutionAlgorithmParametersFilePath = $"{FolderPrefix}{DefaultPrefix}{EvolutionAlgorithmParametersFileName}";

            return ReadSettings<NeatEvolutionAlgorithmParameters>(defaultEvolutionAlgorithmParametersFilePath);
        }

        public TrainingGameSettings ReadTrainingGameSettings()
        {
            var trainingGameSettingsFilePath = $"{FolderPrefix}{TrainingGameSettingsFileName}";

            return ReadSettings<TrainingGameSettings>(trainingGameSettingsFilePath);
        }

        public TrainingGameSettings ReadDefaultTrainingGameSettings()
        {
            var defaultTrainingGameSettingsFilePath = $"{FolderPrefix}{DefaultPrefix}{TrainingGameSettingsFileName}";

            return ReadSettings<TrainingGameSettings>(defaultTrainingGameSettingsFilePath);
        }

        public void SaveSettings(TrainerViewModel trainerViewModel)
        {
            SaveSettings(trainerViewModel.ExperimentSettings, $"{FolderPrefix}{ExperimentSettingsFileName}");
            SaveSettings(trainerViewModel.NeatEvolutionAlgorithmParameters, $"{FolderPrefix}{EvolutionAlgorithmParametersFileName}");
            SaveSettings(trainerViewModel.TrainingGameSettings, $"{FolderPrefix}{TrainingGameSettingsFileName}");
        }

        private static T ReadSettings<T>(string filePath)
        {
            var settingsJson = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(settingsJson);
        }

        private static void SaveSettings<T>(T settings, string filePath)
        {
            var settingsJson = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(filePath, settingsJson);
        }
    }
}