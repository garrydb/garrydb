using GarryDb.Plugins;

namespace GarryDb.Platform.Plugins.Loading
{
    /// <summary>
    ///     A plugin that has been loaded.
    /// </summary>
    public sealed class LoadedPlugin
    {
        /// <summary>
        ///     Initializes a new <see cref="LoadedPlugin" />.
        /// </summary>
        /// <param name="pluginIdentity">The identity of the plugin.</param>
        /// <param name="plugin">The plugin.</param>
        public LoadedPlugin(PluginIdentity pluginIdentity, Plugin plugin)
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
