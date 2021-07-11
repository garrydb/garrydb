using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

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
        }

        /// <summary>
        ///     Loads the <see cref="Plugin" />.
        /// </summary>
        /// <returns>The plugin.</returns>
        public Plugin Load()
        {
            Assembly pluginAssembly = pluginLoadContext.LoadFromStream(inspectedPlugin.PluginAssembly.Load());
            // foreach (ProvidedAssembly providedAssembly in inspectedPlugin.ProvidedAssemblies)
            // {
            //     pluginLoadContext.LoadFromStream(providedAssembly.Load());
            // }
            //
            // foreach (ReferencedAssembly referencedAssembly in inspectedPlugin.ReferencedAssemblies)
            // {
            //     pluginLoadContext.LoadFromStream(referencedAssembly.Load());
            // }
            Type? pluginType = pluginAssembly.GetType(inspectedPlugin.PluginAssembly.PluginType)!;
            if (inspectedPlugin.PluginIdentity.Name == "GarryDB.Avalonia")
            {
                var plugin1 = (Plugin)Activator.CreateInstance(pluginType, pluginLoadContext)!;

                return plugin1;
                
            }
            var plugin = (Plugin)Activator.CreateInstance(pluginType)!;

            return plugin;
        }
    }
}
