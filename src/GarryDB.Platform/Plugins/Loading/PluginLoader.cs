using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

using Autofac;

using GarryDb.Platform.Extensions;
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
        private readonly AssemblyLoadContext pluginLoadContext;

        /// <summary>
        ///     Initializes a new <see cref="PluginLoader" />.
        /// </summary>
        /// <param name="inspectedPlugin">The plugin metadata.</param>
        /// <param name="pluginLoadContext"></param>
        public PluginLoader(InspectedPlugin inspectedPlugin, AssemblyLoadContext pluginLoadContext)
        {
            this.inspectedPlugin = inspectedPlugin;
            this.pluginLoadContext = pluginLoadContext;
            PluginIdentity = inspectedPlugin.PluginIdentity;
        }

        /// <summary>
        ///     Gets the identity of the plugin.
        /// </summary>
        public PluginIdentity PluginIdentity { get; }

        /// <summary>
        ///     Loads the <see cref="Plugin" />.
        /// </summary>
        /// <param name="containerBuilder">The Autofac container builder.</param>
        /// <returns>The plugin.</returns>
        public LoadedPlugin Load(ContainerBuilder containerBuilder)
        {
            Assembly pluginAssembly = pluginLoadContext.LoadFromStream(inspectedPlugin.PluginAssembly.Load());
            IEnumerable<Assembly> referencedAssemblies =
                inspectedPlugin.ReferencedAssemblies.Select(assembly =>
                    pluginLoadContext.LoadFromStream(assembly.Load()));

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
