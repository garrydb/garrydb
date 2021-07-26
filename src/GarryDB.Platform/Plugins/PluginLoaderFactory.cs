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
            IDictionary<InspectedPlugin, PluginLoader> mapping = inspectedPlugins.ToDictionary(x => x, x => new PluginLoader(x));

            foreach ((InspectedPlugin plugin, PluginLoader context) in mapping)
            {
                context.PluginLoadContext.AddProvider(AssemblyLoadContext.Default);

                foreach (ReferencedAssembly assembly in plugin.ReferencedAssemblies)
                {
                    InspectedPlugin? provider =
                        inspectedPlugins.SingleOrDefault(p =>
                            p.ProvidedAssemblies.Any(x => x.IsCompatibleWith(assembly.AssemblyName)));

                    if (provider != null)
                    {
                        context.PluginLoadContext.AddProvider(mapping[provider].PluginLoadContext);
                    }
                }
            }

            return mapping.Select(x => x.Value).ToList();
        }
    }
}
