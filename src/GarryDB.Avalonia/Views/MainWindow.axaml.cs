using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

using GarryDB.Avalonia.ViewModels;

namespace GarryDB.Avalonia.Views
{
    public class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            Opened += (_, _) => AvaloniaPlugin.StartupCompleted.Set();
            AvaloniaXamlLoader.Load(this);
        }
    }
}
