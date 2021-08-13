using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

using GarryDB.Platform.Plugins;
using GarryDB.Plugins;

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

            dispatcher.Invoke(() => splashScreen.Total = result.Count());

            return result;
        }

        public PluginIdentity Load(PluginPackage pluginPackage)
        {
            dispatcher.Invoke(() =>
            {
                splashScreen.Loading++;
                splashScreen.CurrentPlugin = pluginPackage.Name;
            });

            PluginIdentity result = next.Load(pluginPackage);

            dispatcher.Invoke(() =>
            {
                splashScreen.PluginsLoaded++;
            });

            return result;
        }

        public Plugin Instantiate(PluginIdentity pluginIdentity)
        {
            return next.Instantiate(pluginIdentity);
        }

        public void Configure(PluginIdentity pluginIdentity)
        {
            next.Configure(pluginIdentity);
        }

        public async Task StartAsync()
        {
            await next.StartAsync().ConfigureAwait(false);

            dispatcher.Invoke(() =>
            {
                splashScreen.Close();
            });
        }

        public Task StopAsync()
        {
            return next.StopAsync();
        }
    }
}
