using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using GarryDB.UI.Shared;
using GarryDB.UI.ViewModels;
using GarryDB.UI.Views;

using ReactiveUI;

namespace GarryDB.UI
{
    public class App : Application
    {
        private readonly IEnumerable<Extension> extensions;
        private readonly Func<Task> shutdown;

        public App()
            : this(() => Task.CompletedTask, Enumerable.Empty<Extension>())
        {
        }

        public App(Func<Task> shutdown, IEnumerable<Extension> extensions)
        {
            this.shutdown = shutdown;
            this.extensions = extensions;
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            Styles.AddRange(extensions.SelectMany(extension => extension.Styles));

            foreach (IResourceProvider resource in extensions.SelectMany(extension => extension.Resources))
            {
                Resources.MergedDictionaries.Add(resource);
            }
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
