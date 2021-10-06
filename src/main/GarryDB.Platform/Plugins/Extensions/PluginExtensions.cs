using System;

using GarryDb.Platform.Extensions;
using GarryDb.Plugins;

namespace GarryDb.Platform.Plugins.Extensions
{
    /// <summary>
    ///     Extends <see cref="Plugin" />.
    /// </summary>
    internal static class PluginExtensions
    {
        /// <summary>
        ///     Find the configuration type for the plugin.
        /// </summary>
        /// <param name="plugin">The plugin.</param>
        /// <returns>
        ///     The type of the configuration, or <c>null</c> if the plugin
        ///     is not a <see cref="ConfigurablePlugin{TConfiguration}" />.
        /// </returns>
        public static Type? FindConfigurationType(this Plugin plugin)
        {
            return plugin.GetType().FindConfigurationType();
        }
    }
}
