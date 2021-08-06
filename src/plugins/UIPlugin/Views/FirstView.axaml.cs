using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

using ReactiveUI;

using UIPlugin.ViewModels;

namespace UIPlugin.Views
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
