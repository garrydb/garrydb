using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

using GarryDB.Avalonia.ViewModels;

namespace GarryDB.Avalonia.Views
{
    public class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
