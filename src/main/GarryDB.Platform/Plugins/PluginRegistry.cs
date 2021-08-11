using System.Collections;
using System.Collections.Generic;

using GarryDB.Plugins;

namespace GarryDB.Platform.Plugins
{
    /// <summary>
    ///     A registry containing all loaded <see cref="Plugin" />s.
    /// </summary>
    public sealed class PluginRegistry : IEnumerable<PluginIdentity>
    {
        private readonly IDictionary<PluginIdentity, Plugin> plugins;

        /// <summary>
        ///     Intializes a new <see cref="PluginRegistry" />.
        /// </summary>
        public PluginRegistry()
        {
            plugins = new Dictionary<PluginIdentity, Plugin>();
        }

        /// <summary>
        ///     Gets the <see cref="Plugin" /> based on the identity.
        /// </summary>
        /// <param name="pluginIdentity">The identity of the plugin.</param>
        /// <returns>The plugin, or <c>null</c> if the plugin can't be found.</returns>
        public Plugin? this[PluginIdentity pluginIdentity]
        {
            get { return plugins.ContainsKey(pluginIdentity) ? plugins[pluginIdentity] : null; }
        }

        internal void Register(PluginIdentity pluginIdentity, Plugin plugin)
        {
            plugins[pluginIdentity] = plugin;
        }

        /// <inheritdoc />
        public IEnumerator<PluginIdentity> GetEnumerator()
        {
            return plugins.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
