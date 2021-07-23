using System;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using GarryDB.UI.ViewModels;
using GarryDB.UI.Views;

using ReactiveUI;

namespace GarryDB.UI
{
    public class App : Application
    {
        public Func<Task> Shutdown;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
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

                desktop.Events().Exit.InvokeCommand(
                    ReactiveCommand.CreateFromTask((ControlledApplicationLifetimeExitEventArgs _) =>
                    {
                        _.ApplicationExitCode = -1;

                        return Shutdown();
                    }));
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
