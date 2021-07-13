using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

using GarryDB.Avalonia.ViewModels;

using ReactiveUI;

namespace GarryDB.Avalonia.Views
{
    public class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}
