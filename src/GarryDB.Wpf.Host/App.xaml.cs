using System.Threading.Tasks;
using System.Windows;

using GarryDb.Platform;
using GarryDb.Platform.Infrastructure;

namespace GarryDB.Wpf.Host
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            var splashScreen = new SplashScreen();
            splashScreen.Show();

            Task.Run(async () =>
            {
                var garry = new Garry(new WindowsFileSystem());
                garry.PluginFound += (_, _) => Dispatcher.Invoke(() => splashScreen.NumberOfPlugins++);
                garry.PluginLoading += (_, args) => Dispatcher.Invoke(() => splashScreen.CurrentPlugin = args.PluginIdentity);
                garry.PluginLoaded += (_, _) => Dispatcher.Invoke(() => splashScreen.NumberOfPluginsLoaded++);
                garry.PluginStarting += (_, args) => Dispatcher.Invoke(() =>
                {
                    splashScreen.Phase = "Starting";
                    splashScreen.CurrentPlugin = args.PluginIdentity;
                });

                garry.PluginStarted += (_, _) => Dispatcher.Invoke(() =>
                {
                    splashScreen.NumberOfPluginsStarted++;
                    if (splashScreen.NumberOfPluginsStarted == splashScreen.NumberOfPlugins)
                    {
                        splashScreen.Close();
                    }
                });

                garry.PluginStopped += (_, _) => Dispatcher.Invoke(() =>
                {
                    splashScreen.NumberOfPluginsStarted--;
                    if (splashScreen.NumberOfPluginsStarted == 0)
                    {
                        Shutdown();
                    }
                });

                await garry.StartAsync("C:\\Projects\\GarryDb\\Plugins");
            });
        }
    }
}
