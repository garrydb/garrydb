using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

using GarryDb.Avalonia.Host.ViewModels;
using GarryDb.Avalonia.Host.Views;

using GarryDb.Platform;
using GarryDb.Platform.Persistence;
using GarryDb.Platform.Plugins;
using GarryDb.Platform.Plugins.Configuration;
using GarryDb.Platform.Plugins.Lifecycles;
using GarryDb.Platform.Plugins.PackageSources;

using UIPlugin.Shared;

namespace GarryDb.Avalonia.Host
{
    public class App : Application
    {
        private SplashScreen? splashScreen;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var splashScreenViewModel = new SplashScreenViewModel();
                splashScreen = new SplashScreen
                {
                    DataContext = splashScreenViewModel
                };

                splashScreen.Show();
                splashScreen.Activate();
                
                desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                Task.Run(() => TestAsync(splashScreenViewModel, desktop));
            }

            base.OnFrameworkInitializationCompleted();
        }

        private async Task TestAsync(SplashScreenViewModel splashScreenViewModel, IClassicDesktopStyleApplicationLifetime desktop)
        {
            var fileSystem = new WindowsFileSystem();
            string databasePath = Path.Combine(Environment.CurrentDirectory, "data");
            var connectionFactory = new PersistentSqLiteConnectionFactory(fileSystem, databasePath);

            var pluginPackageSources = new List<PluginPackageSource>
            {
                new FromDirectoryPluginPackageSource(fileSystem, "C:\\Projects\\GarryDB\\Plugins"),
                new HardCodedPluginPackageSource(new UIPluginPackage()),
                new HardCodedPluginPackageSource(new GarryPluginPackage())
            };

            await Dispatcher.UIThread.InvokeAsync(() => splashScreenViewModel.Total = pluginPackageSources.SelectMany(x => x.PluginPackages).Count()).ConfigureAwait(false);

            var lifeCycle = new AkkaPluginLifecycle(new ConfigurationStorage(connectionFactory));
            PluginRegistry pluginRegistry = lifeCycle.PluginRegistry;
            var pluginLoader = new PluginLoader(pluginPackageSources, pluginRegistry);

            pluginRegistry.WhenPluginLoaded.Subscribe(identity =>
            {
                Debug.WriteLine($"UI loaded {identity}");

                Dispatcher.UIThread.Post(() =>
                {
                    Debug.WriteLine($"Loaded {identity}");

                    splashScreenViewModel.PluginsLoaded++;
                    splashScreenViewModel.CurrentPlugin = identity.Name;
                });
            });

            var garry = new Garry(pluginLoader, pluginRegistry, lifeCycle);

            await garry.StartAsync().ConfigureAwait(false);
            await Dispatcher.UIThread.InvokeAsync(() => desktop.Shutdown()).ConfigureAwait(false);
        }

        public void OnStarted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow { DataContext = new MainWindowViewModel() };
                desktop.MainWindow.Show();
                desktop.MainWindow.Activate();

                splashScreen!.Close();
            }
        }
   
        public Task ExtendAsync(Extension extension)
        {
            return Dispatcher.UIThread.InvokeAsync(() =>
            {
                Styles.AddRange(extension.Styles);

                foreach (IResourceProvider resource in extension.Resources)
                {
                    Resources.MergedDictionaries.Add(resource);
                }
            });
        }
    }
}
