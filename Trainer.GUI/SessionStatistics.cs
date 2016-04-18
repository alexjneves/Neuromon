using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Trainer.GUI
{
    internal sealed class SessionStatistics
    {
        private static readonly SolidColorBrush RedBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xC3, 0x1C, 0x0C));
        private static readonly SolidColorBrush GreenBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x4B, 0xC3, 0x13));

        private readonly Label _currentGeneration;
        private readonly Label _overallHighestFitness;
        private readonly Label _currentAverageFitness;
        private readonly Label _stagnationDetected;
        private readonly Label _desiredFitnessAchieved;
        private readonly Label _elapsedTime;

        private readonly Stopwatch _stopwatch;
        private readonly DispatcherTimer _dispatcherTimer;

        public SessionStatistics(TrainerWindow trainerWindow)
        {
            _currentGeneration = trainerWindow.CurrentGenerationValueLabel;
            _overallHighestFitness = trainerWindow.OverallHighestFitnessValueLabel;
            _currentAverageFitness = trainerWindow.CurrentAverageFitnessValueLabel;
            _stagnationDetected = trainerWindow.StagnationDetectedValueLabel;
            _desiredFitnessAchieved = trainerWindow.DesiredFitnessAchievedValueLabel;
            _elapsedTime = trainerWindow.ElapsedTimeValueLabel;

            _stopwatch = new Stopwatch();

            _dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };

            _dispatcherTimer.Tick += (sender, args) => ElapsedTime = FormatTime(_stopwatch.Elapsed);
        }

        public uint CurrentGeneration
        {
            set
            {
                _currentGeneration.InvokeOnUiThread(label =>
                {
                    label.Content = $"{value:n0}";
                });
            }
        }

        public double OverallHighestFitness
        {
            set
            {
                _overallHighestFitness.InvokeOnUiThread(label =>
                {
                    label.Content = $"{value:0.000}";
                });
            }
        }

        public double CurrentAverageFitness
        {
            set
            {
                _currentAverageFitness.InvokeOnUiThread(label =>
                {
                    label.Content = $"{value:0.000}";
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

        private string ElapsedTime
        {
            set
            {
                _elapsedTime.InvokeOnUiThread(label =>
                {
                    label.Content = value;
                });
            }
        }

        public void StartTimer()
        {
            _dispatcherTimer.Start();
            _stopwatch.Start();
        }

        public void StopTimer()
        {
            _dispatcherTimer.Stop();
            _stopwatch.Stop();
        }

        public void Reset()
        {
            StopTimer();
            _stopwatch.Reset();

            CurrentGeneration = 0;
            OverallHighestFitness = 0.0;
            CurrentAverageFitness = 0.0;
            StagnationDetected = false;
            DesiredFitnessAchieved = false;
            ElapsedTime = FormatTime(TimeSpan.Zero);
        }

        private static string FormatTime(TimeSpan timespan)
        {
            return $"{timespan.Days:00}:{timespan.Hours:00}:{timespan.Minutes:00}:{timespan.Seconds:00}";
        }
    }
}