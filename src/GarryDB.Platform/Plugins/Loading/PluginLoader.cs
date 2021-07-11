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
            pluginLoadContext.LoadFromAssemblyName(typeof(Plugin).Assembly.GetName());

            Assembly pluginAssembly = pluginLoadContext.LoadFromStream(inspectedPlugin.PluginAssembly.Load());
            TypeInfo pluginType = pluginAssembly.DefinedTypes.Single(x => x.IsPluginType());

            var plugin = (Plugin)Activator.CreateInstance(pluginType)!;

            return plugin;
        }

        public void LoadDependencies()
        {
            foreach (ReferencedAssembly referencedAssembly in inspectedPlugin.ReferencedAssemblies)
            {
                pluginLoadContext.LoadFromStream(referencedAssembly.Load());
            }
        }
    }
}
