using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

using GarryDB.Platform.Extensions;
using GarryDB.Platform.Plugins.Extensions;
using GarryDB.Plugins;

namespace GarryDB.Platform.Plugins
{
    /// <summary>
    ///     A dedicated <see cref="AssemblyLoadContext" /> for loading assemblies for a <see cref="Plugin" />.
    /// </summary>
    public sealed class PluginLoadContext : AssemblyLoadContext
    {
        private readonly IEnumerable<AssemblyLoadContext> providers;
        private readonly PluginPackage pluginPackage;

        /// <summary>
        ///     Initializes a new <see cref="PluginLoadContext" />.
        /// </summary>
        /// <param name="pluginPackage">The package containing the plugin.</param>
        /// <param name="providers">The <see cref="AssemblyLoadContext" />s to use for referenced assemblies.</param>
        public PluginLoadContext(PluginPackage pluginPackage, IEnumerable<AssemblyLoadContext> providers)
            : base(pluginPackage.Name)
        {
            this.pluginPackage = pluginPackage;
            this.providers = providers;
        }

        /// <summary>
        ///     Loads the assembly that contains a <see cref="Plugin" />.
        /// </summary>
        /// <returns>The <see cref="PluginAssembly" />.</returns>
        public PluginAssembly? Load()
        {
            foreach (AssemblyName assemblyName in pluginPackage.Assemblies)
            {
                LoadFromAssemblyName(assemblyName);
            }

            Assembly? assembly = Assemblies.SingleOrDefault(x => x.GetName().Name == pluginPackage.Name);

            return assembly != null ? new PluginAssembly(assembly) : null;
        }

        /// <inheritdoc />
        protected override Assembly? Load(AssemblyName assemblyName)
        {
            var assemblyFromParent = providers.Select(x => new
                {
                    Success = x.TryLoad(assemblyName, out Assembly? assembly),
                    Assembly = assembly,
                    Provider = x.Name
                })
                .FirstOrDefault(x => x.Success);

            if (assemblyFromParent != null)
            {
                return assemblyFromParent.Assembly;
            }

            if (Assemblies.Any(x => x.GetName().IsCompatibleWith(assemblyName)))
            {
                return Assemblies.Single(x => x.GetName().IsCompatibleWith(assemblyName));
            }

            Stream? assemblyStream = pluginPackage.ResolveAssembly(assemblyName);
            if (assemblyStream == null)
            {
                return null;
            }

            using (assemblyStream)
            {
                return LoadFromStream(assemblyStream);
            }
        }

        /// <inheritdoc />
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string? fullPath = pluginPackage.ResolveUnmanagedDllPath(unmanagedDllName);

            return fullPath != null ? LoadUnmanagedDllFromPath(fullPath) : base.LoadUnmanagedDll(unmanagedDllName);
        }
    }
}
