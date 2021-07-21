using System;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using GarryDB.UI.ViewModels;
using GarryDB.UI.Views;

using ReactiveUI;

namespace GarryDB.UI
{
    public class App : Application
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

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel()
                };

                desktop.Events().Exit.InvokeCommand(
                    ReactiveCommand.CreateFromTask((ControlledApplicationLifetimeExitEventArgs _) =>
                    {
                        _.ApplicationExitCode = -1;

                        return shutdown();
                    }));
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
