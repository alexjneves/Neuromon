using System;
using System.Windows.Controls;

namespace Player.AI.Neat.Trainer.Gui
{
    internal sealed class TrainingProgressBox
    {
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
                lines.Add($"{content}{Environment.NewLine}");
            });
        }
    }
}