using System.Collections.Generic;
using System.Threading.Tasks;

using GarryDB.Plugins;

namespace GarryDB.Platform.Plugins
{
    /// <summary>
    ///     The lifecycle of the plugins.
    /// </summary>
    public interface PluginLifecycle
    {
        /// <summary>
        ///     Finds all the <see cref="PluginPackage" />s.
        /// </summary>
        /// <returns>The <see cref="PluginPackage" /> for every plugin.</returns>
        Task<IEnumerable<PluginPackage>> FindAsync();

        /// <summary>
        ///     Load the plugin.
        /// </summary>
        /// <param name="pluginPackage">The package to register.</param>
        /// <returns>The identity of the plugin, or <c>null</c> if the package doesn't contain a plugin.</returns>
        Task<PluginIdentity?> LoadAsync(PluginPackage pluginPackage);

        /// <summary>
        ///     Instantiate the plugin.
        /// </summary>
        /// <param name="pluginIdentity">The identity of the plugin.</param>
        /// <returns>The plugin, or <c>null</c> if the plugin is not found.</returns>
        Task<Plugin?> InstantiateAsync(PluginIdentity pluginIdentity);

        /// <summary>
        ///     Configure the plugin.
        /// </summary>
        /// <param name="pluginIdentity">The identity of the plugin.</param>
        Task ConfigureAsync(PluginIdentity pluginIdentity);

        /// <summary>
        ///     Starts the plugins.
        /// </summary>
        Task StartAsync();

        /// <summary>
        ///     Stops the plugins.
        /// </summary>
        Task StopAsync();
    }
}
