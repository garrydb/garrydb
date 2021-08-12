using System.Collections.Generic;

using GarryDB.Plugins;

namespace GarryDB.Platform.Plugins
{
    /// <summary>
    ///     The lifecycle of the plugins.
    /// </summary>
    public interface PluginLifecycle
    {
        /// <summary>
        ///     Finds all the <see cref="PluginPackage" />s in <paramref name="pluginsDirectory" />.
        /// </summary>
        /// <param name="pluginsDirectory">The directory containing the plugins.</param>
        /// <returns>The <see cref="PluginPackage" /> for every plugin.</returns>
        IEnumerable<PluginPackage> Find(string pluginsDirectory);

        /// <summary>
        ///     Determines the dependencies between the <see cref="PluginPackage" />s.
        /// </summary>
        /// <param name="pluginPackages">The plugin packages.</param>
        void DetermineDependencies(IEnumerable<PluginPackage> pluginPackages);

        /// <summary>
        ///     Register the plugins in an IoC-container.
        /// </summary>
        /// <param name="pluginContextFactory">
        ///     The <see cref="PluginContextFactory"/> for creating <see cref="PluginContext" />.
        /// </param>
        /// <param name="pluginPackage">The package to register.</param>
        /// <returns>The identity of the plugin, or <c>null</c> if the package doesn't contain a plugin.</returns>
        PluginIdentity? Register(PluginContextFactory pluginContextFactory, PluginPackage pluginPackage);

        /// <summary>
        ///     Load the plugin.
        /// </summary>
        /// <param name="pluginIdentity">The identity of the plugin.</param>
        /// <returns>The plugin, or <c>null</c> if the plugin is not found.</returns>
        Plugin? Load(PluginIdentity pluginIdentity);

        /// <summary>
        ///     Configure the plugin.
        /// </summary>
        /// <param name="pluginIdentity">The identity of the plugin.</param>
        /// <returns>The configuration of the plugin.</returns>
        object? Configure(PluginIdentity pluginIdentity);

        /// <summary>
        ///     Start the plugins.
        /// </summary>
        /// <param name="pluginIdentities">The identities of the plugin.</param>
        void Start(IEnumerable<PluginIdentity> pluginIdentities);

        /// <summary>
        ///     Stop the plugins.
        /// </summary>
        /// <param name="pluginIdentities">The identities of the plugin.</param>
        void Stop(IEnumerable<PluginIdentity> pluginIdentities);
    }
}
