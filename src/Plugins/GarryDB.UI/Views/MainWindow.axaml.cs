using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

using GarryDB.UI.ViewModels;

namespace GarryDB.UI.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
