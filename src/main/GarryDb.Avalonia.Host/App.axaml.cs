using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

using GarryDb.Avalonia.Host.ViewModels;
using GarryDb.Avalonia.Host.Views;

using GarryDb.Platform;
using GarryDb.Platform.Akka;
using GarryDb.Platform.Persistence;
using GarryDb.Platform.Plugins;
using GarryDb.Platform.Plugins.Configuration;
using GarryDb.Platform.Plugins.PackageSources;

using ReactiveUI;

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

                Task.Run(() => StartGarryAsync(splashScreenViewModel, desktop));
            }

            base.OnFrameworkInitializationCompleted();
        }

        private async Task StartGarryAsync(SplashScreenViewModel splashScreenViewModel, IClassicDesktopStyleApplicationLifetime desktop)
        {
            var fileSystem = new WindowsFileSystem();
            var pluginPackageSources = new List<PluginPackageSource>
            {
                new FromDirectoryPluginPackageSource(fileSystem, "C:\\Projects\\GarryDB\\Plugins"),
                new HardCodedPluginPackageSource(new UIPluginPackage()),
                new HardCodedPluginPackageSource(new GarryPluginPackage())
            };

            splashScreenViewModel.Total = pluginPackageSources.SelectMany(x => x.PluginPackages).Count();

            string databasePath = Path.Combine(Environment.CurrentDirectory, "data");
            var connectionFactory = new PersistentSqLiteConnectionFactory(fileSystem, databasePath);

            var akkaInitializer = new AkkaInitializer(new ConfigurationStorage(connectionFactory));

            PluginRegistry pluginRegistry = akkaInitializer.PluginRegistry;

            pluginRegistry.WhenPluginLoaded.ObserveOn(RxApp.MainThreadScheduler).Subscribe(identity =>
            {
                Debug.WriteLine($"UI loaded {identity}");

                splashScreenViewModel.PluginsLoaded++;
                splashScreenViewModel.CurrentPlugin = identity.Name;
            });

            var pluginLoader = new PluginLoader(pluginPackageSources, pluginRegistry);
            var garry = new Garry(pluginLoader, pluginRegistry, akkaInitializer.PluginLifecycle);

            await garry.StartAsync().ConfigureAwait(false);
            await Dispatcher.UIThread.InvokeAsync(() => desktop.Shutdown()).ConfigureAwait(false);
        }

        public void OnStarted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    desktop.MainWindow = new MainWindow { DataContext = new MainWindowViewModel() };
                    desktop.MainWindow.Show();
                    desktop.MainWindow.Activate();

                    splashScreen!.Close();
                });
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
