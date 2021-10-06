using GarryDb.Plugins;

namespace GarryDb.Platform.Plugins
{
    /// <summary>
    ///     Creates a <see cref="Plugin" />.
    /// </summary>
    public interface PluginFactory
    {
        /// <summary>
        ///     Create the <see cref="Plugin" /> from the <paramref name="pluginAssembly" />.
        /// </summary>
        /// <param name="pluginAssembly">The identity of the plugin.</param>
        /// <returns>A <see cref="Plugin" />.</returns>
        Plugin Create(PluginAssembly pluginAssembly);
    }
}
