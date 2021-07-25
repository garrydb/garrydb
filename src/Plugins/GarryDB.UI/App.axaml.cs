using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

using GarryDB.UI.ViewModels;
using GarryDB.UI.Views;

using ReactiveUI;

namespace GarryDB.UI
{
    public class App : Application
    {
        private readonly Func<Task> shutdown;
        private readonly IEnumerable<Style> newStyles;

        public App()
            : this(() => Task.CompletedTask, Enumerable.Empty<Style>())
        {
        }

        public App(Func<Task> shutdown, IEnumerable<Style> newStyles)
        {
            this.shutdown = shutdown;
            this.newStyles = newStyles;
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            Styles.AddRange(newStyles);
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

                        return shutdown();
                    }));
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
