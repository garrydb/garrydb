using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

using GarryDb.Platform.Extensions;
using GarryDb.Platform.Infrastructure;
using GarryDb.Plugins;

namespace GarryDb.Platform.Plugins.Inpections
{
    /// <summary>
    ///     Inspects the <see cref="Plugin" /> at a location.
    /// </summary>
    public sealed class Inspector
    {
        private readonly FileSystem fileSystem;

        /// <summary>
        ///     Initializes a new <see cref="Inspector" />.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        public Inspector(FileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        /// <summary>
        ///     Inspect the <see cref="Plugin" /> located in <paramref name="directory" />.
        /// </summary>
        /// <param name="directory">The directory containing the plugin.</param>
        public InspectedPlugin? Inspect(string directory)
        {
            IEnumerable<string> files = fileSystem.GetFiles(directory, "*.dll").ToList();

            var loadContext = new AssemblyLoadContext("inspect", true);
            using (loadContext.EnterContextualReflection())
            {
                IList<Assembly> assemblies = files.Select(file => loadContext.LoadFromAssemblyPath(file)).ToList();

                InspectedPlugin? result = Inspect(assemblies);
                loadContext.Unload();
                return result;
            }
        }

        private InspectedPlugin? Inspect(IList<Assembly> assemblies)
        {
            Assembly? pluginAssembly =
                assemblies.SingleOrDefault(assembly => assembly.DefinedTypes.Any(type => type.IsPluginType()));

            if (pluginAssembly == null)
            {
                return null;
            }

            IEnumerable<Assembly> providedAssemblies =
                assemblies.Where(assembly => IsProvidedAssembly(assembly, pluginAssembly)).ToList();
            IEnumerable<Assembly> referencedAssemblies = assemblies.Except(pluginAssembly).Except(providedAssemblies).ToList();

            return new InspectedPlugin(
                new PluginAssembly(pluginAssembly),
                providedAssemblies.Select(assembly => new ProvidedAssembly(assembly)),
                referencedAssemblies.Select(assembly => new ReferencedAssembly(assembly))
            );
        }

        private bool IsProvidedAssembly(Assembly assembly, Assembly pluginAssembly)
        {
            string? pluginAssemblyName = pluginAssembly.GetName().Name;
            if (pluginAssemblyName == null)
            {
                return false;
            }

            string? assemblyName = assembly.GetName().Name;

            if (assemblyName == null)
            {
                return false;
            }

            return assemblyName.ToLowerInvariant().StartsWith(pluginAssemblyName.ToLowerInvariant() + ".contract") ||
                   assemblyName.ToLowerInvariant().StartsWith(pluginAssemblyName.ToLowerInvariant() + ".shared");
        }
    }
}
