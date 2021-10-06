using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;

using GarryDb.Platform.Extensions;

namespace GarryDb.Platform.Plugins
{
    /// <summary>
    ///     Loads the plugins from every <see cref="PluginPackageSource" />.
    /// </summary>
    public sealed class PluginLoader
    {
        private readonly PluginRegistry pluginRegistry;
        private readonly IEnumerable<PluginPackageSource> pluginPackageSources;

        private readonly IDictionary<PluginPackage, PluginLoadContext> pluginLoadContexts;

        /// <summary>
        ///     Initializes a new <see cref="PluginLoader" />.
        /// </summary>
        /// <param name="pluginPackageSources">The plugin package sources.</param>
        /// <param name="pluginRegistry">The plugin registry.</param>
        public PluginLoader(IEnumerable<PluginPackageSource> pluginPackageSources, PluginRegistry pluginRegistry)
        {
            this.pluginRegistry = pluginRegistry;
            this.pluginPackageSources = pluginPackageSources;

            pluginLoadContexts = new Dictionary<PluginPackage, PluginLoadContext>();
        }

        /// <summary>
        ///     Load the plugins.
        /// </summary>
        public Task LoadAsync()
        {
            IEnumerable<PluginPackage> packages =
                pluginPackageSources
                    .SelectMany(pluginPackageSource => pluginPackageSource.PluginPackages)
                    .ToList();
            
            DetermineDependencies(packages);

            foreach (PluginPackage pluginPackage in packages)
            {
                PluginLoadContext pluginLoadContext = pluginLoadContexts[pluginPackage];

                PluginAssembly? pluginAssembly = pluginLoadContext.Load();
                if (pluginAssembly != null)
                {
                    pluginRegistry.Load(pluginAssembly);
                }
            }

            return Task.CompletedTask;
        }
        
        private void DetermineDependencies(IEnumerable<PluginPackage> pluginPackages)
        {
            IDictionary<AssemblyLoadContext, AssemblyProvider> assemblyProviders =
                new Dictionary<AssemblyLoadContext, AssemblyProvider>
                {
                    { AssemblyLoadContext.Default, new AssemblyProvider(AssemblyLoadContext.Default) }
                };

            foreach (PluginPackage pluginPackage in pluginPackages)
            {
                IEnumerable<AssemblyLoadContext> providers =
                    AssemblyLoadContext.Default.AsEnumerable()
                        .Concat(
                            pluginLoadContexts
                                .Where(x => pluginPackage.IsDependentOn(x.Key))
                                .Select(x => x.Value)
                                .Select(provider => provider))
                        .ToList();

                var loadContext = new PluginLoadContext(pluginPackage, providers.Select(provider => assemblyProviders[provider]).ToList());

                pluginLoadContexts[pluginPackage] = loadContext;
                assemblyProviders[loadContext] = new AssemblyProvider(loadContext);
            }
        }
    }
}
