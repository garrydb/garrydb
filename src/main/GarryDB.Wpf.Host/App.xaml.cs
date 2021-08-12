using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using GarryDB.Platform;
using GarryDB.Platform.Persistence;
using GarryDB.Platform.Plugins;
using GarryDB.Platform.Plugins.Configuration;
using GarryDB.Platform.Plugins.Lifecycles;
using GarryDB.Plugins;

using Dispatcher = System.Windows.Threading.Dispatcher;

namespace GarryDB.Wpf.Host
{
    public sealed class UiPluginLifecycle : PluginLifecycle
    {
        private readonly PluginLifecycle next;
        private readonly Dispatcher dispatcher;
        private readonly SplashScreen splashScreen;

        public UiPluginLifecycle(PluginLifecycle next, Dispatcher dispatcher, SplashScreen splashScreen) 
        {
            this.next = next;
            this.dispatcher = dispatcher;
            this.splashScreen = splashScreen;
        }

        public IEnumerable<PluginPackage> Find(string pluginsDirectory)
        {
            IEnumerable<PluginPackage> result = next.Find(pluginsDirectory).ToList();

            dispatcher.Invoke(() => splashScreen.Total = result.Count() + 1);

            return result;
        }

        public PluginIdentity Register(PluginContextFactory pluginContextFactory, PluginPackage pluginPackage)
        {
            dispatcher.Invoke(() =>
            {
                splashScreen.Current++;

                splashScreen.CurrentPlugin = pluginPackage.Name;
            });

            return next.Register(pluginContextFactory, pluginPackage);
        }

        public void Start(IEnumerable<PluginIdentity> pluginIdentities)
        {
            next.Start(pluginIdentities);

            dispatcher.Invoke(() =>
            {
                splashScreen.Current++;
                splashScreen.CurrentPlugin = null;
            });
        }

        public void DetermineDependencies(IEnumerable<PluginPackage> pluginPackages)
        {
            next.DetermineDependencies(pluginPackages);
        }

        public Plugin Load(PluginIdentity pluginIdentity)
        {
            return next.Load(pluginIdentity);
        }

        public object Configure(PluginIdentity pluginIdentity)
        {
            return next.Configure(pluginIdentity);
        }

        public void Stop(IEnumerable<PluginIdentity> pluginIdentities)
        {
            next.Stop(pluginIdentities);
        }
    }


    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            var splashScreen = new SplashScreen();
            splashScreen.Show();

            Task.Run(() => Start(splashScreen));
        }

        private void Start(SplashScreen splashScreen)
        {
            var fileSystem = new WindowsFileSystem();
            string databasePath = Path.Combine(Environment.CurrentDirectory, "data");
            var connectionFactory = new PersistentSqLiteConnectionFactory(fileSystem, databasePath);

            var defaultLifecycle = new DefaultPluginLifecycle(fileSystem, new ConfigurationStorage(connectionFactory));
            var akkaLifecycle = new AkkaPluginLifecycle(defaultLifecycle);
            var uiLifecycle = new UiPluginLifecycle(akkaLifecycle, Dispatcher, splashScreen);

            var garry = new Garry(uiLifecycle);

            garry.Start("C:\\Projects\\GarryDB\\Plugins");

            Dispatcher.Invoke(() => Shutdown());
        }
    }
}
