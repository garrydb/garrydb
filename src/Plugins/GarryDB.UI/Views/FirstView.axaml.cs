using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

using GarryDB.UI.ViewModels;

using ReactiveUI;

namespace GarryDB.UI.Views
{
    public partial class FirstView : ReactiveUserControl<FirstViewModel>
    {
        public FirstView()
        {
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}
