using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

using GarryDb.Avalonia.Host.ViewModels;
using GarryDb.Avalonia.Host.Views;

using GarryDB.Platform;
using GarryDB.Platform.Persistence;
using GarryDB.Platform.Plugins;
using GarryDB.Platform.Plugins.Configuration;
using GarryDB.Platform.Plugins.Lifecycles;
using GarryDB.Platform.Plugins.PackageSources;

using UIPlugin.Shared;

namespace GarryDb.Avalonia.Host
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var splashScreen = new SplashScreen();
                splashScreen.Show();
                splashScreen.Activate();
                
                desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                Task.Run(() => Test(splashScreen, desktop));
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void Test(SplashScreen splashScreen, IClassicDesktopStyleApplicationLifetime desktop)
        {
            var fileSystem = new WindowsFileSystem();
            string databasePath = Path.Combine(Environment.CurrentDirectory, "data");
            var connectionFactory = new PersistentSqLiteConnectionFactory(fileSystem, databasePath);

            var pluginPackageSources = new List<PluginPackageSource>
            {
                new FromDirectoryPluginPackageSource(fileSystem, "C:\\Projects\\GarryDB\\Plugins"),
                new HardCodedPluginPackageSource(new UIPluginPackage())
            };

            var defaultLifecycle = new DefaultPluginLifecycle(pluginPackageSources, new ConfigurationStorage(connectionFactory));
            var uiLifecycle = new UIPluginLifecycle(defaultLifecycle, splashScreen, () =>
            {
                desktop.MainWindow = new MainWindow { DataContext = new MainWindowViewModel() };
                desktop.MainWindow.Show();
                desktop.MainWindow.Activate();

                splashScreen.Close();
            });

            var garry = new Garry(uiLifecycle);

            garry.StartAsync().ConfigureAwait(true).GetAwaiter().GetResult();

            desktop.Shutdown();
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
    }
}
