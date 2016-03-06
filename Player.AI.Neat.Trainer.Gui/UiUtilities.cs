using System;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Player.AI.Neat.Trainer.Gui
{
    internal static class UiUtilities
    {
        public static void InvokeOnUiThread(this Label label, Action<Label> action)
        {
            label.Dispatcher.Invoke(() => action(label));
        }

        public static void InvokeOnUiThread(this TextBlock textBlock, Action<InlineCollection> action)
        {
            textBlock.Dispatcher.Invoke(() => action(textBlock.Inlines));
        }
    }
}