using System;
using System.Windows.Controls;

namespace Trainer.GUI
{
    internal sealed class TrainingProgressBox
    {
        private const int MaximumNumberOfLines = 100;

        private const string SpaceSeparator = "    ";
        private const string StatusSeparator = "\t\t\t";
        private const string Column = "|    ";

        private readonly TextBlock _textBlock;

        public TrainingProgressBox(TextBlock textBlock)
        {
            _textBlock = textBlock;
        }

        public void Clear()
        {
            _textBlock.InvokeOnUiThread(lines =>
            {
                lines.Clear();
            });
        }

        public void WriteStatusUpdate(uint generation, double highestFitness, double averageFitness)
        {
            WriteLine(
                $"Generation:{SpaceSeparator}{generation,-6:n0}{StatusSeparator}" +
                $"{Column}Best Fitness:{SpaceSeparator}{highestFitness:000.000}{StatusSeparator}" +
                $"{Column}Average Fitness:{SpaceSeparator}{averageFitness:000.000}"
            );
        }

        public void WriteLine(string content)
        {
            _textBlock.InvokeOnUiThread(lines =>
            {
                if (_textBlock.Inlines.Count >= MaximumNumberOfLines)
                {
                    lines.Clear();
                }

                lines.Add($"{content}{Environment.NewLine}");
            });
        }
    }
}