using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

using GarryDb.Avalonia.Host.ViewModels;

using ReactiveUI;

namespace GarryDb.Avalonia.Host.Views
{
    public partial class FirstView : ReactiveUserControl<FirstViewModel>
    {
        public FirstView()
        {
            this.WhenActivated(disposables =>
                               {
                               });
            AvaloniaXamlLoader.Load(this);
        }
    }
}
