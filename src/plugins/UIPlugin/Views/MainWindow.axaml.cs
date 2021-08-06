using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

using ReactiveUI;

using UIPlugin.ViewModels;

namespace UIPlugin.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);

#if DEBUG
            this.AttachDevTools();
#endif
            this.WhenActivated(disposables =>
                               {
                                   this.WhenAnyValue(window => window.IsVisible)
                                       .Where(x => x)
                                       .Do(_ =>
                                           {
                                               Topmost = true;
                                               Topmost = false;
                                           })
                                       .Subscribe()
                                       .DisposeWith(disposables);
                               });
        }
    }
}
