using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ConnectionStringCreatorGUI.Behaviors
{
    internal static class WindowBehaviors
    {
        public static readonly DependencyProperty IsOpenProperty =
                 DependencyProperty.RegisterAttached("IsOpen", typeof(bool), typeof(WindowBehaviors),
                 new PropertyMetadata(IsOpenChanged));

        private static void IsOpenChanged(DependencyObject obj,
                                          DependencyPropertyChangedEventArgs args)
        {
            Window window = Window.GetWindow(obj);

            if (window != null && !((bool)args.NewValue))
                window.Close();
        }

        public static bool GetIsOpen(Window target)
        {
            return (bool)target.GetValue(IsOpenProperty);
        }

        public static void SetIsOpen(Window target, bool value)
        {
            target.SetValue(IsOpenProperty, value);
        }
    }
}
