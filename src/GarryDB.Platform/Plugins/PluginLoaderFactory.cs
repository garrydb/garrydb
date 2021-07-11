using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;

using GarryDb.Platform.Plugins.Inpections;
using GarryDb.Platform.Plugins.Loading;

namespace GarryDb.Platform.Plugins
{
    /// <summary>
    ///     Creates <see cref="PluginLoader" />s.
    /// </summary>
    public sealed class PluginLoaderFactory
    {
        /// <summary>
        ///     Create <see cref="PluginLoader" /> from the supplied <paramref name="inspectedPlugins" />.
        /// </summary>
        /// <param name="inspectedPlugins">The inspected plugins.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> containing the <see cref="PluginLoader" />s for every plugin.</returns>
        public IEnumerable<PluginLoader> Create(params InspectedPlugin[] inspectedPlugins)
        {
            IDictionary<InspectedPlugin, PluginLoadContext> mapping = inspectedPlugins.ToDictionary(x => x, x => new PluginLoadContext(x));

            foreach ((InspectedPlugin plugin, PluginLoadContext context) in mapping)
            {
                context.AddProvider(AssemblyLoadContext.Default);
                foreach (ReferencedAssembly assembly in plugin.ReferencedAssemblies)
                {
                    InspectedPlugin? provider =
                        inspectedPlugins.SingleOrDefault(p => p.ProvidedAssemblies.Any(x => x.IsCompatibleWith(assembly.AssemblyName)));
                    if (provider != null)
                    {
                        context.AddProvider(mapping[provider]);
                    }
                }
            }

            return mapping.Select(x => new PluginLoader(x.Key, x.Value)).ToList();
        }
    }
}
