using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using GarryDB.Avalonia.ViewModels;
using ReactiveUI;

namespace GarryDB.Avalonia.Views
{
    public class FirstView : ReactiveUserControl<FirstViewModel>
    {
        public FirstView()
        {
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}