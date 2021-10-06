using System.Threading.Tasks;

namespace GarryDb.Platform.Plugins
{
    /// <summary>
    ///     Manages the lifecycle of the plugins.
    /// </summary>
    public interface PluginLifecycle
    {
        /// <summary>
        ///     Configure the plugin.
        /// </summary>
        /// <param name="pluginIdentity">The identity of the plugin.</param>
        Task ConfigureAsync(PluginIdentity pluginIdentity);

        /// <summary>
        ///     Starts the plugin.
        /// </summary>
        /// <param name="pluginIdentity">The identity of the plugin.</param>
        Task StartAsync(PluginIdentity pluginIdentity);

        /// <summary>
        ///     Stops the plugin.
        /// </summary>
        /// <param name="pluginIdentity">The identity of the plugin.</param>
        Task StopAsync(PluginIdentity pluginIdentity);
    }
}
