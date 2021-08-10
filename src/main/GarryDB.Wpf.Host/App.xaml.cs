using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

using GarryDB.Platform;
using GarryDB.Platform.Persistence;

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

            using (var garry = new Garry(fileSystem, connectionFactory))
            {
                garry.PluginLoading += (_, loaded) =>
                                       {
                                           Dispatcher.Invoke(() =>
                                                             {
                                                                 splashScreen.Current++;
                                                                 splashScreen.CurrentPlugin = loaded.PluginIdentity;
                                                                 splashScreen.Total = loaded.TotalNumberOfPlugins + 1;
                                                             });
                                       };

                garry.Starting += (_, _) => Dispatcher.Invoke(() =>
                                                              {
                                                                  splashScreen.Current++;
                                                                  splashScreen.CurrentPlugin = null;
                                                              });

                garry.Start("C:\\Projects\\GarryDB\\Plugins");

                Dispatcher.Invoke(() => Shutdown());
            }
        }
    }
}
