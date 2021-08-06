using System;
using System.Threading.Tasks;
using System.Windows;

using GarryDB.Platform;
using GarryDB.Platform.Infrastructure;
using GarryDB.Platform.Startup;

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

            Task.Run(async () =>
                     {
                         using (var garry = new Garry(new WindowsFileSystem()))
                         {
                             using (garry.WhenProgressUpdated.Subscribe(updated => OnProgressUpdated(splashScreen, updated),
                                                                        () => OnProgressCompleted(splashScreen)))
                             {
                                 await garry.StartAsync("C:\\Projects\\GarryDB\\Plugins").ConfigureAwait(false);
                             }

                             Dispatcher.Invoke(() => Shutdown());
                         }
                     });
        }

        private void OnProgressUpdated(SplashScreen splashScreen, StartupProgressUpdated updated)
        {
            Dispatcher.Invoke(() =>
                              {
                                  splashScreen.Phase = updated.Stage;
                                  splashScreen.Current = updated.CurrentStep;
                                  splashScreen.CurrentPlugin = updated.PluginIdentity;
                                  splashScreen.Total = updated.TotalSteps;
                              });
        }

        private void OnProgressCompleted(SplashScreen splashScreen)
        {
            Dispatcher.Invoke(() => splashScreen.Close());
        }
    }
}
