using System.Windows;

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
                TrainingGameSettings = jsonSettingsReader.ReadTrainingGameSettings(),
                ExperimentSettings = jsonSettingsReader.ReadExperimentSettings(),
                NeatEvolutionAlgorithmParameters = jsonSettingsReader.ReadEvolutionAlgorithmParameters()
            };

            DataContext = TrainerViewModel;
        }
    }
}
