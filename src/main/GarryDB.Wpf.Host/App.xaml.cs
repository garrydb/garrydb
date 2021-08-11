using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using GarryDB.Platform;
using GarryDB.Platform.Persistence;
using GarryDB.Platform.Plugins;

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

            using (var garry = new Garry(config =>
            {
                return
                    config
                        .Replace(_ => fileSystem)
                        .Replace(_ => connectionFactory)
                        .Finder(inner => pluginsDirectory =>
                        {
                            IEnumerable<PluginDirectory> result = inner(pluginsDirectory).ToList();

                            Dispatcher.Invoke(() => splashScreen.Total = result.Count() + 1);

                            return result;
                        })
                        .Loader(inner => pluginIdentity =>
                        {
                            Dispatcher.Invoke(() =>
                            {
                                splashScreen.Current++;

                                splashScreen.CurrentPlugin = pluginIdentity.Name;
                            });

                            return inner(pluginIdentity);
                        })
                        .Starter(inner => pluginIdentities =>
                        {
                            inner(pluginIdentities);

                            Dispatcher.Invoke(() =>
                            {
                                splashScreen.Current++;
                                splashScreen.CurrentPlugin = null;
                            });
                        });
            }))
            {
                garry.Start("C:\\Projects\\GarryDB\\Plugins");

                Dispatcher.Invoke(() => Shutdown());
            }
        }
    }
}
