using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Common;

namespace Trainer.GUI
{
    public partial class TrainerWindow : Window
    {
        private readonly JsonSettingsIO _jsonSettingsIo;
        private readonly TrainingProgressBox _trainingProgressBox;
        private readonly SessionStatistics _sessionStatistics;
        private readonly DispatcherTimer _dispatcherTimer;

        private NeatTrainer _neatTrainer;
        private volatile TrainingState _trainingState;
        private bool _settingsMinimised;

        public TrainerViewModel TrainerViewModel { get; private set; }

        public TrainerWindow()
        {
            InitializeComponent();

            _jsonSettingsIo = new JsonSettingsIO();
            _trainingProgressBox = new TrainingProgressBox(TrainingProgressTextBlock);
            _sessionStatistics = new SessionStatistics(this);

            _dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(5)
            };

            _dispatcherTimer.Tick += (sender, args) => TriggerAutoSave();

            TrainerViewModel = new TrainerViewModel
            {
                TrainingGameSettings = _jsonSettingsIo.ReadTrainingGameSettings(),
                ExperimentSettings = _jsonSettingsIo.ReadExperimentSettings(),
                NeatEvolutionAlgorithmParameters = _jsonSettingsIo.ReadEvolutionAlgorithmParameters()
            };

            DataContext = TrainerViewModel;

            _trainingState = TrainingState.Uninitialised;
            _settingsMinimised = false;
        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            _jsonSettingsIo.SaveSettings(TrainerViewModel);

            _trainingProgressBox.WriteLine("Current configuration settings have been saved.");
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

            _trainingProgressBox.WriteLine("Default settings have been restored.");
        }

        private void MinimiseSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            _settingsMinimised = !_settingsMinimised;

            SettingsRow.Height = _settingsMinimised ? new GridLength(0) : GridLength.Auto;

            MinMaxSettingsButton.Content = _settingsMinimised ? "Maximise Settings" : "Minimise Settings";
        }

        private void CreateSessionButton_Click(object sender, RoutedEventArgs e)
        {
            if (_trainingState == TrainingState.AutoSaving)
            {
                _trainingProgressBox.WriteLine("Please wait for auto save to complete.");
                return;
            }

            if (!(_trainingState == TrainingState.Paused || _trainingState == TrainingState.Uninitialised))
            {
                _trainingProgressBox.WriteLine("Please pause the current session before creating a new one.");
                return;
            }

            _trainingProgressBox.Clear();
            ResetState();

            if (!ValidateFields())
            {
                _trainingProgressBox.WriteLine("Could not create session. Please fix the invalid configuration settings.");
                return;
            }

            _neatTrainer = CreateTrainingSession();
            _trainingState = TrainingState.Paused;

            _trainingProgressBox.WriteLine("Created a new training session.");
        }

        private void StartTrainingButton_Click(object sender, RoutedEventArgs e)
        {
            if (_trainingState == TrainingState.AutoSaving)
            {
                _trainingProgressBox.WriteLine("Please wait for auto save to complete.");
                return;
            }

            if (_trainingState == TrainingState.Uninitialised)
            {
                _trainingProgressBox.WriteLine("Must create a session before starting one.");
                return;
            }

            if (_trainingState != TrainingState.Paused)
            {
                _trainingProgressBox.WriteLine("Already training or awaiting pause.");
                return;
            }

            _trainingProgressBox.WriteLine("Starting training...");

            StartTraining();
        }

        private void PauseTrainingButton_Click(object sender, RoutedEventArgs e)
        {
            if (_trainingState != TrainingState.Training)
            {
                _trainingProgressBox.WriteLine("Not currently training.");
                return;
            }

            _trainingProgressBox.WriteLine("Pausing training...");

            PauseTraining();
        }

        private void SavePopulationButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CanSave()) return;

            _neatTrainer.SavePopulation(TrainerViewModel.ExperimentSettings.OutputPopulationFilePath);

            _trainingProgressBox.WriteLine($"Current population has been saved to {TrainerViewModel.ExperimentSettings.OutputPopulationFilePath}");
        }

        private void SaveChampionGenomeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CanSave()) return;

            _neatTrainer.SaveChampionGenome(TrainerViewModel.ExperimentSettings.OutputChampionFilePath);

            _trainingProgressBox.WriteLine($"Current champion chromosome has been saved to {TrainerViewModel.ExperimentSettings.OutputChampionFilePath}");
        }

        private void ResetState()
        {
            if (_trainingState == TrainingState.Paused)
            {
                _trainingProgressBox.WriteLine("Destroying previous session...");
                _sessionStatistics.Reset();
            }

            _neatTrainer = null;
            _trainingState = TrainingState.Uninitialised;
        }

        private bool ValidateFields()
        {
            var valid = true;

            if (TrainerViewModel.TrainingGameSettings.OpponentType == PlayerTypes.NeatAi)
            {
                if (!File.Exists(TrainerViewModel.TrainingGameSettings.OpponentBrainFileName))
                {
                    _trainingProgressBox.WriteLine($"Opponent brain file not found: {TrainerViewModel.TrainingGameSettings.OpponentBrainFileName}");
                    valid = false;
                }
            }

            if (TrainerViewModel.ExperimentSettings.LoadExistingPopulation)
            {
                if (!File.Exists(TrainerViewModel.ExperimentSettings.ExistingPopulationFilePath))
                {
                    _trainingProgressBox.WriteLine($"Existing population file not found: {TrainerViewModel.ExperimentSettings.ExistingPopulationFilePath}");
                    valid = false;
                }
            }

            if (!valid)
            {
                System.Media.SystemSounds.Beep.Play();
            }

            return valid;
        }

        private NeatTrainer CreateTrainingSession()
        {
            var neatTrainer = new NeatTrainer(TrainerViewModel.ExperimentSettings, TrainerViewModel.NeatEvolutionAlgorithmParameters, TrainerViewModel.TrainingGameSettings);

            neatTrainer.OnStatusUpdate += (generation, highestFitness, averageFitness) =>
            {
                if (_trainingState == TrainingState.AwaitingTraining)
                {
                    _trainingState = TrainingState.Training;
                }

                _sessionStatistics.CurrentGeneration = generation;
                _sessionStatistics.CurrentAverageFitness = averageFitness;

                _trainingProgressBox.WriteStatusUpdate(generation, highestFitness, averageFitness);
            };

            neatTrainer.OnTrainingPaused += () =>
            {
                _sessionStatistics.StopTimer();
                _dispatcherTimer.Stop();

                if (_trainingState == TrainingState.AutoSaving)
                {
                    PerformAutoSave();
                }
                else
                {
                    _trainingState = TrainingState.Paused;
                    _trainingProgressBox.WriteLine("Training paused.");
                }
            };

            var stagnationDetectedMessage =
                $"Stagnation detected: Previous {TrainerViewModel.ExperimentSettings.StagnationDetectionTriggerValue} generations had the same fitness value.";

            if (TrainerViewModel.ExperimentSettings.StopTrainingOnStagnationDetection)
            {
                neatTrainer.OnStagnationDetected += () =>
                {
                    OnStagnationDetected(stagnationDetectedMessage);

                    if (_trainingState == TrainingState.Training)
                    {
                        _trainingProgressBox.WriteLine("Auto stop enabled, training will now be paused...");
                        PauseTraining();
                    }
                };
            }
            else
            {
                neatTrainer.OnStagnationDetected += () => OnStagnationDetected(stagnationDetectedMessage);
            }

            neatTrainer.OnHighestFitnessAchieved += fitness =>
            {
                _sessionStatistics.OverallHighestFitness = fitness;
            };

            neatTrainer.OnDesiredFitnessAchieved += () =>
            {
                _sessionStatistics.DesiredFitnessAchieved = true;
                _trainingProgressBox.WriteLine("Desired fitness has been achieved.");
            };

            return neatTrainer;
        }

        private void StartTraining()
        {
            _trainingState = TrainingState.AwaitingTraining;

            Task.Run(() => _neatTrainer.StartTraining());

            _sessionStatistics.StartTimer();
            _dispatcherTimer.Start();
        }

        private void PauseTraining()
        {
            _trainingState = TrainingState.AwaitingPause;

            _neatTrainer.StopTraining();
        }

        private void OnStagnationDetected(string message)
        {
            System.Media.SystemSounds.Beep.Play();
            _trainingProgressBox.WriteLine(message);
            _sessionStatistics.StagnationDetected = true;
        }

        private bool CanSave()
        {
            if (_trainingState == TrainingState.Paused)
            {
                return true;
            }

            var errorMessage = "Please stop training before saving.";

            if (_trainingState == TrainingState.Uninitialised)
            {
                errorMessage = "There is no session to save.";
            }
            else if (_trainingState == TrainingState.AutoSaving)
            {
                errorMessage = "Currently auto saving.";
            }

            _trainingProgressBox.WriteLine(errorMessage);
            return false;
        }

        private void TriggerAutoSave()
        {
            if (_trainingState != TrainingState.Training)
            {
                return;
            }

            _trainingState = TrainingState.AutoSaving;
            _trainingProgressBox.WriteLine("Auto Save: Pausing training...");

            _neatTrainer.StopTraining();
        }

        private void PerformAutoSave()
        {
            _neatTrainer.SavePopulation(TrainerViewModel.ExperimentSettings.OutputPopulationFilePath);
            _neatTrainer.SaveChampionGenome(TrainerViewModel.ExperimentSettings.OutputChampionFilePath);

            _trainingProgressBox.WriteLine("Auto Save: Saved population and champion genome.");
            _trainingProgressBox.WriteLine("Auto Save: Resuming training...");

            StartTraining();
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
