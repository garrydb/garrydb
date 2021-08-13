using System.Collections.Generic;
using System.Linq;

using GarryDB.Platform.Plugins;
using GarryDB.Plugins;

namespace GarryDB.Platform
{
    internal sealed class DummyPluginContextFactory : PluginContextFactory
    {
        public PluginContext Create(PluginIdentity pluginIdentity)
        {
            throw new System.NotImplementedException();
        }
    }

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
        ///     Start <see cref="Garry" /> and load the plugins from the <paramref name="pluginsDirectory" />.
        /// </summary>
        /// <param name="pluginsDirectory">The directory containing the plugins.</param>
        public void Start(string pluginsDirectory)
        {
            IReadOnlyList<PluginPackage> pluginPackages = pluginLifecycle.Find(pluginsDirectory).ToList();

            IReadOnlyList<PluginIdentity> pluginIdentities =
                pluginPackages
                    .Select(pluginPackage => pluginLifecycle.Load(new DummyPluginContextFactory(), pluginPackage))
                    .Where(x => x != null)
                    .Select(x => x!)
                    .ToList();

            IDictionary<PluginIdentity, Plugin> plugins =
                pluginIdentities
                    .Select(pluginIdentity => new { Plugin = pluginLifecycle.Instantiate(pluginIdentity), PluginIdentity = pluginIdentity })
                    .Where(x => x.Plugin != null)
                    .ToDictionary(x => x.PluginIdentity, x => x.Plugin!);

            foreach (PluginIdentity pluginIdentity in plugins.Keys)
            {
                pluginLifecycle.Configure(pluginIdentity);
            }

            pluginLifecycle.Start(plugins.Keys.ToList());

            GarryPlugin garryPlugin = plugins.Values.OfType<GarryPlugin>().Single();
            garryPlugin.WaitUntilShutdownRequested();

            pluginLifecycle.Stop(plugins.Keys.ToList());
        }
    }
}
