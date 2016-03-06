using System.Windows.Controls;
using System.Windows.Media;

namespace Player.AI.Neat.Trainer.Gui
{
    internal sealed class SessionStatistics
    {
        private static readonly SolidColorBrush RedBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xC3, 0x1C, 0x0C));
        private static readonly SolidColorBrush GreenBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x4B, 0xC3, 0x13));

        private readonly Label _currentGeneration;
        private readonly Label _overallHighestFitness;
        private readonly Label _stagnationDetected;
        private readonly Label _desiredFitnessAchieved;

        public SessionStatistics(TrainerWindow trainerWindow)
        {
            _currentGeneration = trainerWindow.CurrentGenerationValueLabel;
            _overallHighestFitness = trainerWindow.OverallHighestFitnessValueLabel;
            _stagnationDetected = trainerWindow.StagnationDetectedValueLabel;
            _desiredFitnessAchieved = trainerWindow.DesiredFitnessAchievedValueLabel;
        }

        public uint CurrentGeneration
        {
            set
            {
                _currentGeneration.InvokeOnUiThread(label =>
                {
                    label.Content = value;
                });
            }
        }

        public double OverallHighestFitness
        {
            set
            {
                _overallHighestFitness.InvokeOnUiThread(label =>
                {
                    label.Content = value;
                });
            }
        }

        public bool StagnationDetected
        {
            set
            {
                _stagnationDetected.InvokeOnUiThread(label =>
                {
                    label.Content = value;
                    label.Foreground = value ? RedBrush : GreenBrush;
                });
            }
        }

        public bool DesiredFitnessAchieved
        {
            set
            {
                _desiredFitnessAchieved.InvokeOnUiThread(label =>
                {
                    label.Content = value;
                    label.Foreground = value ? GreenBrush : RedBrush;
                });
            }
        }

        public void Clear()
        {
            CurrentGeneration = 0;
            OverallHighestFitness = 0.0;
            StagnationDetected = false;
            DesiredFitnessAchieved = false;
        }
    }
}