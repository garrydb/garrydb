using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

using GarryDB.UI.ViewModels;

namespace GarryDB.UI.Views
{
    public class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
