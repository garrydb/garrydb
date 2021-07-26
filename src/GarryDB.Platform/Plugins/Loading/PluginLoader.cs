using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Autofac;

using GarryDb.Platform.Extensions;

using GarryDB.Platform.Plugins;

using GarryDb.Platform.Plugins.Inpections;
using GarryDb.Plugins;

namespace GarryDb.Platform.Plugins.Loading
{
    /// <summary>
    ///     Loads a <see cref="Plugin" />.
    /// </summary>
    public sealed class PluginLoader
    {
        private readonly InspectedPlugin inspectedPlugin;

        /// <summary>
        ///     Initializes a new <see cref="PluginLoader" />.
        /// </summary>
        /// <param name="inspectedPlugin">The plugin metadata.</param>
        public PluginLoader(InspectedPlugin inspectedPlugin)
        {
            this.inspectedPlugin = inspectedPlugin;
            PluginLoadContext = new PluginLoadContext(inspectedPlugin);
            PluginIdentity = inspectedPlugin.PluginIdentity;
        }

        /// <summary>
        ///     Gets the identity of the plugin.
        /// </summary>
        public PluginIdentity PluginIdentity { get; }

        /// <summary>
        ///     Gets the associated <see cref="PluginLoadContext" />.
        /// </summary>
        public PluginLoadContext PluginLoadContext { get; }

        /// <summary>
        ///     Loads the <see cref="Plugin" />.
        /// </summary>
        /// <param name="containerBuilder">The Autofac container builder.</param>
        /// <returns>The plugin.</returns>
        public LoadedPlugin Load(ContainerBuilder containerBuilder)
        {
            Assembly pluginAssembly = PluginLoadContext.LoadPluginAssembly();
            IEnumerable<Assembly> referencedAssemblies = PluginLoadContext.LoadReferencedAssemblies();
            containerBuilder.RegisterAssemblyModules(referencedAssemblies.Concat(pluginAssembly).ToArray());
            
            Type pluginType = pluginAssembly.GetType(inspectedPlugin.PluginAssembly.PluginType)!;
            var plugin = new LoadedPlugin(inspectedPlugin.PluginIdentity, inspectedPlugin.StartupOrder);

            containerBuilder
                .RegisterType(pluginType)
                .Keyed<Plugin>(plugin.PluginIdentity)
                .SingleInstance();
            
            return plugin;
        }
    }
}
