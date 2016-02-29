using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Player.AI.Neat.Trainer.Gui
{
    public partial class TrainerWindow : Window
    {
        private readonly JsonSettingsIO _jsonSettingsIo;
        private readonly TrainingProgressBox _trainingProgressBox;

        private NeatTrainer _neatTrainer;
        private volatile TrainingState _trainingState;

        public TrainerViewModel TrainerViewModel { get; private set; }

        public TrainerWindow()
        {
            InitializeComponent();

            _jsonSettingsIo = new JsonSettingsIO();
            _trainingProgressBox = new TrainingProgressBox(TrainingProgressTextBlock);

            TrainerViewModel = new TrainerViewModel
            {
                TrainingGameSettings = _jsonSettingsIo.ReadTrainingGameSettings(),
                ExperimentSettings = _jsonSettingsIo.ReadExperimentSettings(),
                NeatEvolutionAlgorithmParameters = _jsonSettingsIo.ReadEvolutionAlgorithmParameters()
            };

            DataContext = TrainerViewModel;

            _neatTrainer = CreateTrainingSession();
            _trainingState = TrainingState.Paused;
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

        private void CreateSessionButton_Click(object sender, RoutedEventArgs e)
        {
            if (_trainingState != TrainingState.Paused)
            {
                _trainingProgressBox.WriteLine("Please pause the current session before creating a new one.");
                return;
            }

            _trainingProgressBox.Clear();

            _neatTrainer = CreateTrainingSession();

            _trainingProgressBox.WriteLine("Created a new training session.");
        }

        private void StartTrainingButton_Click(object sender, RoutedEventArgs e)
        {
            if (_trainingState != TrainingState.Paused)
            {
                _trainingProgressBox.WriteLine("Already training or awaiting pause.");
                return;
            }

            _trainingState = TrainingState.AwaitingTraining;
            _trainingProgressBox.WriteLine("Starting training...");

            Task.Run(() => _neatTrainer.StartTraining());
        }

        private void PauseTrainingButton_Click(object sender, RoutedEventArgs e)
        {
            if (_trainingState != TrainingState.Training)
            {
                _trainingProgressBox.WriteLine("Not yet training.");
                return;
            }

            _trainingState = TrainingState.AwaitingPause;
            _trainingProgressBox.WriteLine("Pausing training...");

            _neatTrainer.StopTraining();
        }

        private NeatTrainer CreateTrainingSession()
        {
            var neatTrainer = new NeatTrainer(TrainerViewModel.ExperimentSettings, TrainerViewModel.NeatEvolutionAlgorithmParameters, TrainerViewModel.TrainingGameSettings);
            neatTrainer.OnStatusUpdate += (generation, fitness) =>
            {
                if (_trainingState == TrainingState.AwaitingTraining)
                {
                    _trainingState = TrainingState.Training;
                }

                _trainingProgressBox.WriteLine($"Generation: {generation}, Best Fitness: {fitness}");
            };

            neatTrainer.OnTrainingPaused += () =>
            {
                _trainingProgressBox.WriteLine("Training paused.");
                _trainingState = TrainingState.Paused;
            };

            return neatTrainer;
        }

        // http://stackoverflow.com/questions/25761795/doing-autoscroll-with-scrollviewer-scrolltoend-only-worked-while-debugging-ev
        private void ScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var sv = sender as ScrollViewer;
            var autoScrollToEnd = true;

            if (sv == null)
            {
                throw new Exception();
            }

            if (sv.Tag != null)
            {
                autoScrollToEnd = (bool)sv.Tag;
            }

            if (e.ExtentHeightChange == 0)
            {
                autoScrollToEnd = sv.ScrollableHeight == sv.VerticalOffset;
            }
            else if (autoScrollToEnd)
            {
                sv.ScrollToEnd();
            }

            sv.Tag = autoScrollToEnd;
        }
    }
}
