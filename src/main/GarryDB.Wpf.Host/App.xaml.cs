using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

using GarryDB.Platform;
using GarryDB.Platform.Persistence;
using GarryDB.Platform.Plugins.Configuration;
using GarryDB.Platform.Plugins.Lifecycles;

namespace GarryDB.Wpf.Host
{
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

            var defaultLifecycle = new DefaultPluginLifecycle(fileSystem);
            var akkaLifecycle = new AkkaPluginLifecycle(defaultLifecycle, new ConfigurationStorage(connectionFactory));
            var uiLifecycle = new UiPluginLifecycle(akkaLifecycle, Dispatcher, splashScreen);

            var garry = new Garry(uiLifecycle);

            garry.Start("C:\\Projects\\GarryDB\\Plugins");

            Dispatcher.Invoke(() => Shutdown());
        }
    }
}
