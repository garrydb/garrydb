using GarryDb.Platform.Plugins;
using GarryDb.Plugins;

namespace GarryDb.Platform.Actors
{
    /// <summary>
    ///     The message that is sent when a <see cref="GarryDb.Plugins.Plugin" /> has been loaded.
    /// </summary>
    internal sealed class PluginLoaded
    {
        /// <summary>
        ///     Initializes a new <see cref="PluginLoaded" />.
        /// </summary>
        /// <param name="pluginIdentity">The identity of the plugin.</param>
        /// <param name="plugin">The plugin.</param>
        public PluginLoaded(PluginIdentity pluginIdentity, Plugin plugin)
        {
            PluginIdentity = pluginIdentity;
            Plugin = plugin;
        }

        /// <summary>
        ///     Gets the identity of the plugin.
        /// </summary>
        public PluginIdentity PluginIdentity { get; }

        /// <summary>
        ///     Gets the plugin.
        /// </summary>
        public Plugin Plugin { get; }
    }
}
