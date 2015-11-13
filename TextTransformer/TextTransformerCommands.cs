using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace TextTransformer
{
    public static class TextTransformerCommands
    {
        public static readonly RoutedUICommand AddRule;
        public static readonly RoutedUICommand DeleteRule;
        public static readonly RoutedUICommand Run;

        static TextTransformerCommands()
        {
            AddRule = new RoutedUICommand("Add Rule", "AddRule", typeof(UIElement));
            AddRule.InputGestures.Add(new KeyGesture(Key.A, ModifierKeys.Alt));

            DeleteRule = new RoutedUICommand("Delete Rule", "DeleteRule", typeof(UIElement));
            DeleteRule.InputGestures.Add(new KeyGesture(Key.D, ModifierKeys.Alt));

            Run = new RoutedUICommand("Run", "Run", typeof(UIElement));
            Run.InputGestures.Add(new KeyGesture(Key.R, ModifierKeys.Alt));
        }
    }
}
