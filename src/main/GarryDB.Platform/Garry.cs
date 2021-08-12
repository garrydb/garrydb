using System.Collections.Generic;
using System.Linq;

using GarryDB.Platform.Bootstrapping;
using GarryDB.Platform.Bootstrapping.Extensions;
using GarryDB.Platform.Extensions;
using GarryDB.Platform.Plugins;
using GarryDB.Plugins;

namespace GarryDB.Platform
{
    /// <summary>
    ///     The Garry.
    /// </summary>
    public sealed class Garry
    {
        private readonly Bootstrapper bootstrapper;

        /// <summary>
        ///     Initializes <see cref="Garry" />.
        /// </summary>
        /// <param name="modifiers"></param>
        public Garry(params Modifier.BootstrapperModifier[] modifiers)
        {
            bootstrapper = new Bootstrapper().ApplyDefault().ApplyAkka();

            modifiers.ForEach(modifier => modifier(bootstrapper));
        }

        /// <summary>
        ///     Start <see cref="Garry" /> and load the plugins from the <paramref name="pluginsDirectory" />.
        /// </summary>
        /// <param name="pluginsDirectory">The directory containing the plugins.</param>
        public void Start(string pluginsDirectory)
        {
            IReadOnlyList<PluginPackage> pluginDirectories = bootstrapper.Find(pluginsDirectory).ToList();
            IReadOnlyList<PluginLoadContext> pluginLoadContexts = bootstrapper.Prepare(pluginDirectories).ToList();
            IReadOnlyList<PluginIdentity> pluginIdentities = bootstrapper.Register(pluginLoadContexts).ToList();
            IDictionary<PluginIdentity, Plugin> plugins =
                pluginIdentities
                    .Select(pluginIdentity => new { Plugin = bootstrapper.Load(pluginIdentity), PluginIdentity = pluginIdentity })
                    .Where(x => x.Plugin != null)
                    .ToDictionary(x => x.PluginIdentity, x => x.Plugin!);

            foreach (PluginIdentity pluginIdentity in plugins.Keys)
            {
                bootstrapper.Configure(pluginIdentity, null!);
            }

            bootstrapper.Start(plugins.Keys.ToList());

            GarryPlugin garryPlugin = plugins.Values.OfType<GarryPlugin>().Single();
            garryPlugin.WaitUntilShutdownRequested();

            bootstrapper.Stop(plugins.Keys.ToList());
        }
    }
}
