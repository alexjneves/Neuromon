using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;

namespace Player.AI.Neat.Trainer.Gui
{
    internal static class UiUtilities
    {
        private static readonly DispatcherPriority DispatcherPriority = DispatcherPriority.Background;

        public static void InvokeOnUiThread(this Label label, Action<Label> action)
        {
            label.Dispatcher.InvokeAsync(() => action(label), DispatcherPriority);
        }

        public static void InvokeOnUiThread(this TextBlock textBlock, Action<InlineCollection> action)
        {
            textBlock.Dispatcher.InvokeAsync(() => action(textBlock.Inlines), DispatcherPriority);
        }
    }
}