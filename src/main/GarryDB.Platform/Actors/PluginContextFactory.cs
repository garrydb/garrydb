using GarryDB.Platform.Plugins;
using GarryDB.Plugins;

namespace GarryDB.Platform.Actors
{
    /// <summary>
    ///     Creates a <see cref="PluginContext" /> for a <see cref="Plugin" />.
    /// </summary>
    internal interface PluginContextFactory
    {
        /// <summary>
        ///     Create the <see cref="PluginContext" /> for <paramref name="pluginIdentity" />.
        /// </summary>
        /// <param name="pluginIdentity">The identity of the plugin.</param>
        /// <returns>A <see cref="PluginContext" />.</returns>
        PluginContext Create(PluginIdentity pluginIdentity);
    }
}
