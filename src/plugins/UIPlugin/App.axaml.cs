using System;
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

        public App()
            : this(() => Task.CompletedTask)
        {
        }

        public App(Func<Task> shutdown)
        {
            this.shutdown = shutdown;
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

                desktop.MainWindow = new MainWindow
                                     {
                                         DataContext = new MainWindowViewModel()
                                     };

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
