using System;

using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;

namespace ExtendAvalonia.Styles
{
    public sealed class MainWindowStyle : Style
    {
        public MainWindowStyle()
        {
            Selector = default(Selector).Is<Window>();// Garry("GarryDB.UI.Views.MainWindow");
            // Setters.Add(new Setter(TemplatedControl.BackgroundProperty, Brush.Parse("#00146E")));
        }

        public static Selector Garry(string type)
        {
            return default(Selector).Is(Type.GetType($"{type}, GarryDB.UI"));
        }
    }
}
