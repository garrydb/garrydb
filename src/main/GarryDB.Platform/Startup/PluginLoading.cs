using GarryDB.Platform.Plugins;

namespace GarryDB.Platform.Startup
{
    /// <summary>
    ///     Is raised when a plugin is being loaded.
    /// </summary>
    public sealed class PluginLoading
    {
        /// <summary>
        ///     Initializes a new <see cref="PluginLoading" />.
        /// </summary>
        /// <param name="pluginIdentity">The identity of the plugin.</param>
        /// <param name="totalNumberOfPlugins">The total number of plugins.</param>
        public PluginLoading(PluginIdentity pluginIdentity, int totalNumberOfPlugins)
        {
            PluginIdentity = pluginIdentity;
            TotalNumberOfPlugins = totalNumberOfPlugins;
        }

        /// <summary>
        ///     Gets the identity of the plugin.
        /// </summary>
        public PluginIdentity PluginIdentity { get; }

        /// <summary>
        ///     Gets the total number of steps.
        /// </summary>
        public int TotalNumberOfPlugins { get; }
    }
}
