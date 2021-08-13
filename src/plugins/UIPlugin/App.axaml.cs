using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

using ReactiveUI;

using UIPlugin.Shared;
using UIPlugin.ViewModels;
using UIPlugin.Views;

namespace UIPlugin
{
    internal sealed class App : Application
    {
        private readonly Func<Task> shutdown;
        private readonly Action whenWindowIsShown;

        public App()
            : this(() => Task.CompletedTask, () => { })
        {
        }

        public App(Func<Task> shutdown, Action whenWindowIsShown)
        {
            this.shutdown = shutdown;
            this.whenWindowIsShown = whenWindowIsShown;
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void Extend(Extension extension)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                Styles.AddRange(extension.Styles);

                foreach (IResourceProvider resource in extension.Resources)
                {
                    Resources.MergedDictionaries.Add(resource);
                }
            });
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;

                var mainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel()
                };

                mainWindow.WhenActivated(disposables =>
                {
                    mainWindow.WhenAnyValue(x => x.IsVisible)
                        .Where(x => x)
                        .Do(_ => whenWindowIsShown())
                        .Subscribe()
                        .DisposeWith(disposables);
                });

                desktop.MainWindow = mainWindow;

                desktop.Events()
                       .Exit.InvokeCommand(ReactiveCommand.CreateFromTask((ControlledApplicationLifetimeExitEventArgs _) =>
                                                                          {
                                                                              _.ApplicationExitCode = -1;

                                                                              return shutdown();
                                                                          }));
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
