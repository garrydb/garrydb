using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Threading;

using GarryDb.Avalonia.Host.ViewModels;

using GarryDB.Platform.Plugins;
using GarryDB.Plugins;

namespace GarryDb.Avalonia.Host
{
    public sealed class UIPluginLifecycle : PluginLifecycle
    {
        private readonly PluginLifecycle next;
        private readonly SplashScreenViewModel splashScreen;
        private readonly Action afterLoadingComplete;

        public UIPluginLifecycle(PluginLifecycle next, SplashScreenViewModel splashScreen, Action afterLoadingComplete) 
        {
            this.next = next;
            this.splashScreen = splashScreen;
            this.afterLoadingComplete = afterLoadingComplete;
        }

        public async Task<IEnumerable<PluginPackage>> FindAsync()
        {
            IEnumerable<PluginPackage> result = await next.FindAsync().ConfigureAwait(false);

            await Dispatcher.UIThread.InvokeAsync(() => splashScreen.Total = result.Count()).ConfigureAwait(false);

            return result;
        }

        public async Task<PluginIdentity?> LoadAsync(PluginPackage pluginPackage)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                splashScreen.Loading++;
                splashScreen.CurrentPlugin = pluginPackage.Name;
            }).ConfigureAwait(false);

            PluginIdentity? result = await next.LoadAsync(pluginPackage).ConfigureAwait(false);

            await Dispatcher.UIThread.InvokeAsync(() => splashScreen.PluginsLoaded++).ConfigureAwait(false);

            return result;
        }

        public Task<Plugin?> InstantiateAsync(PluginIdentity pluginIdentity)
        {
            return next.InstantiateAsync(pluginIdentity);
        }

        public Task ConfigureAsync(PluginIdentity pluginIdentity)
        {
            return next.ConfigureAsync(pluginIdentity);
        }

        public async Task StartAsync()
        {
            await next.StartAsync().ConfigureAwait(false);
            
            await Dispatcher.UIThread.InvokeAsync(() => afterLoadingComplete()).ConfigureAwait(false);
        }

        public Task StopAsync()
        {
            return next.StopAsync();
        }
    }
}
