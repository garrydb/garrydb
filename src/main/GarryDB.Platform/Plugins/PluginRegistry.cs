using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using GarryDb.Plugins;

namespace GarryDb.Platform.Plugins
{
    /// <summary>
    ///     Contains a list of all loaded <see cref="Plugin" />s.
    /// </summary>
    public sealed class PluginRegistry : IEnumerable<PluginIdentity>
    {
        private readonly PluginFactory pluginFactory;
        private readonly IDictionary<PluginIdentity, Plugin> plugins;
        private readonly IDictionary<PluginIdentity, int> startupOrders;

        /// <summary>
        ///     Initializes a new <see cref="PluginRegistry" />.
        /// </summary>
        /// <param name="pluginFactory">The factory for creating plugins.</param>
        public PluginRegistry(PluginFactory pluginFactory)
        {
            plugins = new Dictionary<PluginIdentity, Plugin>();
            startupOrders = new Dictionary<PluginIdentity, int>();

            this.pluginFactory = pluginFactory;
        }

        /// <summary>
        ///     Raised when a <see cref="Plugin" /> is loaded.
        /// </summary>
        public event EventHandler<PluginIdentity> PluginLoaded = delegate { };

        /// <summary>
        ///     Load the plugin from the <paramref name="pluginAssembly" />.
        /// </summary>
        /// <param name="pluginAssembly">The assembly containing the plugin.</param>
        public void Load(PluginAssembly pluginAssembly)
        {
            Plugin plugin = pluginFactory.Create(pluginAssembly);
            PluginIdentity pluginIdentity = pluginAssembly.PluginIdentity;

            plugins[pluginIdentity] = plugin;
            startupOrders[pluginIdentity] = pluginAssembly.StartupOrder;
            
            PluginLoaded(this, pluginIdentity);
        }

        /// <summary>
        ///     Gets the<see cref="Plugin" />.
        /// </summary>
        /// <param name="identity">The identity of the plugin.</param>
        public Plugin this[PluginIdentity identity]
        {
            get { return plugins[identity]; }
        }

        /// <inheritdoc />
        public IEnumerator<PluginIdentity> GetEnumerator()
        {
            return startupOrders.OrderBy(x => x.Value).Select(x => x.Key).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
