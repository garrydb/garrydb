using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

using GarryDb.Plugins;

namespace GarryDb.Platform.Plugins
{
    /// <summary>
    ///     Contains a list of all loaded <see cref="Plugin" />s.
    /// </summary>
    public sealed class PluginRegistry : IEnumerable<PluginIdentity>, IDisposable
    {
        private readonly PluginFactory pluginFactory;
        private readonly IDictionary<PluginIdentity, Plugin> plugins;
        private readonly IDictionary<PluginIdentity, int> startupOrders;
        private readonly ReplaySubject<PluginIdentity> pluginLoaded;

        /// <summary>
        ///     Initializes a new <see cref="PluginRegistry" />.
        /// </summary>
        /// <param name="pluginFactory">The factory for creating plugins.</param>
        public PluginRegistry(PluginFactory pluginFactory)
        {
            plugins = new Dictionary<PluginIdentity, Plugin>();
            startupOrders = new Dictionary<PluginIdentity, int>();

            this.pluginFactory = pluginFactory;
            pluginLoaded = new ReplaySubject<PluginIdentity>();
        }

        /// <summary>
        /// 
        /// </summary>
        public IObservable<PluginIdentity> WhenPluginLoaded
        {
            get { return pluginLoaded; }
        }

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

            pluginLoaded.OnNext(pluginIdentity);
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

        /// <inheritdoc />
        public void Dispose()
        {
            pluginLoaded.Dispose();
        }
    }
}
