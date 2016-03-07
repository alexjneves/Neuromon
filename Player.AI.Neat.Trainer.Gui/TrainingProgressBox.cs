using System;
using System.Windows.Controls;

namespace Player.AI.Neat.Trainer.Gui
{
    internal sealed class TrainingProgressBox
    {
        private const int MaximumNumberOfLines = 10000;

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