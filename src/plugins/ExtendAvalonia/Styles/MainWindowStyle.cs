using System;

using Avalonia.Controls;
using Avalonia.Styling;

namespace ExtendAvalonia.Styles
{
    public sealed class MainWindowStyle : Style
    {
        public MainWindowStyle()
        {
            Selector = default(Selector).Is<Window>(); // Garry("UIPlugin.Views.MainWindow");
            // Setters.Add(new Setter(TemplatedControl.BackgroundProperty, Brush.Parse("#00146E")));
        }

        public static Selector Garry(string type)
        {
            return default(Selector).Is(Type.GetType($"{type}, UIPlugin"));
        }
    }
}
