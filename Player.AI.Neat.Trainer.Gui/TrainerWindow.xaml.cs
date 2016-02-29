using System.Windows;

namespace Player.AI.Neat.Trainer.Gui
{
    public partial class TrainerWindow : Window
    {
        private readonly JsonSettingsIO _jsonSettingsIo;

        public TrainerViewModel TrainerViewModel { get; private set; }

        public TrainerWindow()
        {
            InitializeComponent();

            _jsonSettingsIo = new JsonSettingsIO();

            TrainerViewModel = new TrainerViewModel
            {
                TrainingGameSettings = _jsonSettingsIo.ReadTrainingGameSettings(),
                ExperimentSettings = _jsonSettingsIo.ReadExperimentSettings(),
                NeatEvolutionAlgorithmParameters = _jsonSettingsIo.ReadEvolutionAlgorithmParameters()
            };

            DataContext = TrainerViewModel;
        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            _jsonSettingsIo.SaveSettings(TrainerViewModel);
        }

        private void RestoreDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            TrainerViewModel = new TrainerViewModel
            {
                TrainingGameSettings = _jsonSettingsIo.ReadDefaultTrainingGameSettings(),
                ExperimentSettings = _jsonSettingsIo.ReadDefaultExperimentSettings(),
                NeatEvolutionAlgorithmParameters = _jsonSettingsIo.ReadDefaultEvolutionAlgorithmParameters()
            };

            DataContext = TrainerViewModel;
        }
    }
}
