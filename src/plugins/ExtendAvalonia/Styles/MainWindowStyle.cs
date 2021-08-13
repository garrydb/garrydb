using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;

using UIPlugin.Shared;

namespace ExtendAvalonia.Styles
{
    internal sealed class MainWindowStyle : Style
    {
        public MainWindowStyle()
        {
            Selector = new TypeSelector("UIPlugin.Views.MainWindow");
            Setters.Add(new Setter(TemplatedControl.BackgroundProperty, Brush.Parse("#C7D1FF")));
        }
    }
}
