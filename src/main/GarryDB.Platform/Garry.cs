using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GarryDB.Platform.Plugins;
using GarryDB.Plugins;

namespace GarryDB.Platform
{
    /// <summary>
    ///     The Garry.
    /// </summary>
    public sealed class Garry
    {
        private readonly PluginLifecycle pluginLifecycle;

        /// <summary>
        ///     Initializes <see cref="Garry" />.
        /// </summary>
        /// <param name="pluginLifecycle">The lifecycle of the plugins.</param>
        public Garry(PluginLifecycle pluginLifecycle)
        {
            this.pluginLifecycle = pluginLifecycle;
        }

        /// <summary>
        ///     Start <see cref="Garry" />.
        /// </summary>
        public async Task StartAsync()
        {
            IReadOnlyList<PluginPackage> pluginPackages = (await pluginLifecycle.FindAsync().ConfigureAwait(false)).ToList();
            IList<PluginIdentity> pluginIdentities = await LoadAsync(pluginPackages).ToListAsync().ConfigureAwait(false);
            IDictionary<PluginIdentity, Plugin> plugins = await InstantiateAsync(pluginIdentities).ToDictionaryAsync(x => x.Key, x => x.Value).ConfigureAwait(false);

            foreach (PluginIdentity pluginIdentity in plugins.Keys)
            {
                await pluginLifecycle.ConfigureAsync(pluginIdentity).ConfigureAwait(false);
            }

            await pluginLifecycle.StartAsync().ConfigureAwait(false);

            GarryPlugin garryPlugin = plugins.Values.OfType<GarryPlugin>().Single();
            garryPlugin.WaitUntilShutdownRequested();

            await pluginLifecycle.StopAsync().ConfigureAwait(false);
        }

        private async IAsyncEnumerable<KeyValuePair<PluginIdentity, Plugin>> InstantiateAsync(IList<PluginIdentity> pluginIdentities)
        {
            foreach (PluginIdentity pluginIdentity in pluginIdentities)
            {
                Plugin? plugin = await pluginLifecycle.InstantiateAsync(pluginIdentity).ConfigureAwait(false);
                if (plugin != null)
                {
                    yield return new KeyValuePair<PluginIdentity, Plugin>(pluginIdentity, plugin);
                }
            }
        }

        private async IAsyncEnumerable<PluginIdentity> LoadAsync(IReadOnlyList<PluginPackage> pluginPackages)
        {
            foreach (PluginPackage pluginPackage in pluginPackages)
            {
                PluginIdentity? identity = await pluginLifecycle.LoadAsync(pluginPackage).ConfigureAwait(false);
                if (identity != null)
                {
                    yield return identity;
                }
            }
        }
    }
}
