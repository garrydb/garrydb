using GarryDB.Platform.Plugins;

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
        /// <param name="startupOrder">The startup order of the plugin.</param>
        public LoadedPlugin(PluginIdentity pluginIdentity, int startupOrder)
        {
            PluginIdentity = pluginIdentity;
            StartupOrder = startupOrder;
        }

        /// <summary>
        ///     Gets the identity of the plugin.
        /// </summary>
        public PluginIdentity PluginIdentity { get; }

        /// <summary>
        ///     Gets the startup order.
        /// </summary>
        public int StartupOrder { get; }
    }
}
