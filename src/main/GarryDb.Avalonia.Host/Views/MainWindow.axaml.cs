using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

using GarryDb.Avalonia.Host.ViewModels;

using ReactiveUI;

namespace GarryDb.Avalonia.Host.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
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

                        Debug.WriteLine("**** WINDOW SHOWN ****");
                    })
                    .Subscribe()
                    .DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
