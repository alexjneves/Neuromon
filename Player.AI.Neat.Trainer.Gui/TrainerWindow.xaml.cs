using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace Player.AI.Neat.Trainer.Gui
{
    public partial class TrainerWindow : Window
    {
        public TrainerViewModel TrainerViewModel { get; }

        public TrainerWindow()
        {
            InitializeComponent();

            var jsonSettingsReader = new JsonSettingsReader();

            TrainerViewModel = new TrainerViewModel
            {
                TrainingGameSettings = jsonSettingsReader.ReadTrainingGameSettings()
            };

            DataContext = TrainerViewModel;
        }
    }

    internal sealed class JsonSettingsReader
    {
        private const string NeuromonExperimentSettingsFileName = "ExperimentSettings.json";
        private const string EvolutionAlgorithmParametersFileName = "EvolutionAlgorithmParameters.json";
        private const string TrainingGameSettingsFileName = "TrainingGameSettings.json";

        public TrainingGameSettings ReadTrainingGameSettings()
        {
            var trainingGameSettingsJson = File.ReadAllText(TrainingGameSettingsFileName);
            return JsonConvert.DeserializeObject<TrainingGameSettings>(trainingGameSettingsJson);
        }
    }
}
