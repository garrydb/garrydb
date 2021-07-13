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

            _ = Task.Run(async () =>
              {
                  var garry = new Garry(new WindowsFileSystem());
                  garry.PluginFound += (_, _) => Dispatcher.Invoke(() =>
                  {
                      splashScreen.NumberOfPlugins++;
                      if (splashScreen.NumberOfSteps == int.MaxValue)
                      {
                          splashScreen.NumberOfSteps = 3;
                      }
                      else
                      {
                          splashScreen.NumberOfSteps += 3;
                      }
                  });
                  garry.PluginLoading += (_, args) => Dispatcher.Invoke(() => splashScreen.CurrentPlugin = args.PluginIdentity);
                  garry.PluginLoaded += (_, _) => Dispatcher.Invoke(() => splashScreen.CurrentStep++);
                  garry.PluginConfiguring += (_, args) => Dispatcher.Invoke(() =>
                  {
                      splashScreen.Phase = "Configuring";
                      splashScreen.CurrentPlugin = args.PluginIdentity;
                      splashScreen.CurrentStep++;
                  });
                  garry.PluginStarting += (_, args) => Dispatcher.Invoke(() =>
                  {
                      splashScreen.Phase = "Starting";
                      splashScreen.CurrentPlugin = args.PluginIdentity;
                      splashScreen.CurrentStep++;
                  });
                  garry.PluginStarted += (_, _) => Dispatcher.Invoke(() =>
                  {
                      splashScreen.CurrentStep++;
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
