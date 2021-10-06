using System.Threading.Tasks;

using GarryDb.Platform.Plugins;
using GarryDb.Platform.Plugins.Lifecycles;

namespace GarryDb.Platform
{
    /// <summary>
    ///     The Garry.
    /// </summary>
    public sealed class Garry
    {
        private readonly PluginLoader pluginLoader;
        private readonly PluginRegistry pluginRegistry;
        private readonly PluginLifecycle pluginLifecycle;

        /// <summary>
        ///     Initializes <see cref="Garry" />.
        /// </summary>
        /// <param name="pluginLoader">The plugin loader.</param>
        /// <param name="pluginRegistry">The plugin registry.</param>
        /// <param name="pluginLifecycle">The plugin lifecycle.</param>
        public Garry(PluginLoader pluginLoader, PluginRegistry pluginRegistry, PluginLifecycle pluginLifecycle)
        {
            this.pluginLoader = pluginLoader;
            this.pluginRegistry = pluginRegistry;
            this.pluginLifecycle = pluginLifecycle;
        }

        /// <summary>
        ///     Start <see cref="Garry" />.
        /// </summary>
        public async Task StartAsync()
        {
            await pluginLoader.LoadAsync().ConfigureAwait(false);

            foreach (PluginIdentity pluginIdentity in pluginRegistry)
            {
                await pluginLifecycle.ConfigureAsync(pluginIdentity).ConfigureAwait(false);
            }

            foreach (PluginIdentity pluginIdentity in pluginRegistry)
            {
                await pluginLifecycle.StartAsync(pluginIdentity).ConfigureAwait(false);
            }

            var garryPlugin = (GarryPlugin)pluginRegistry[GarryPlugin.PluginIdentity];
            garryPlugin.WaitUntilShutdownRequested();

            foreach (PluginIdentity pluginIdentity in pluginRegistry)
            {
                await pluginLifecycle.StopAsync(pluginIdentity).ConfigureAwait(false);
            }
        }
    }
}
