using System;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Player.AI.Neat.Trainer.Gui
{
    internal sealed class TrainingProgressBox
    {
        private readonly TextBlock _textBlock;
        private readonly InlineCollection _lines;

        public TrainingProgressBox(TextBlock textBlock)
        {
            _textBlock = textBlock;
            _lines = textBlock.Inlines;
        }

        public void Clear()
        {
            InvokeOnUiThread(() =>
            {
                _lines.Clear();
            });
        }

        public void WriteLine(string content)
        {
            InvokeOnUiThread(() =>
            {
                _lines.Add($"{content}{Environment.NewLine}");
            });
        }

        private void InvokeOnUiThread(Action action)
        {
            _textBlock.Dispatcher.Invoke(action);
        }
    }
}