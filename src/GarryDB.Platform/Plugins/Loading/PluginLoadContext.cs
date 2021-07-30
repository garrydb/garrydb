using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

using GarryDB.Platform.Extensions;

using GarryDB.Platform.Plugins.Inpections;

using GarryDB.Platform.Plugins.Loading.Extensions;
using GarryDB.Plugins;

namespace GarryDB.Platform.Plugins.Loading
{
    /// <summary>
    ///     Loads the assemblies of a <see cref="Plugin" /> into their own scoped <see cref="AssemblyLoadContext" />.
    /// </summary>
    public sealed class PluginLoadContext : AssemblyLoadContext
    {
        private readonly InspectedPlugin inspectedPlugin;
        private readonly IList<AssemblyLoadContext> providers;
        private readonly AssemblyDependencyResolver resolver;

        /// <summary>
        ///     Initializes a new <see cref="PluginLoadContext" />.
        /// </summary>
        /// <param name="inspectedPlugin">The inspected plugin.</param>
        public PluginLoadContext(InspectedPlugin inspectedPlugin)
            : base(inspectedPlugin.PluginIdentity.ToString())
        {
            this.inspectedPlugin = inspectedPlugin;
            providers = new List<AssemblyLoadContext>();
            resolver = new AssemblyDependencyResolver(inspectedPlugin.Path);
        }

        /// <summary>
        ///     Add a provider of assemblies to this load context.
        /// </summary>
        /// <param name="provider">An <see cref="AssemblyLoadContext" /> that can provide references.</param>
        public void AddProvider(AssemblyLoadContext provider)
        {
            providers.Add(provider);
        }

        /// <summary>
        ///     Loads the assembly that contains the plugin.
        /// </summary>
        /// <returns>The assembly containing the plugin.</returns>
        public Assembly LoadPluginAssembly()
        {
            return Load(inspectedPlugin.PluginAssembly.AssemblyName)!;
        }

        /// <summary>
        ///     Loads the referenced assemblies.
        /// </summary>
        /// <returns>The assemblies that are referenced by the plugin.</returns>
        public IEnumerable<Assembly> LoadReferencedAssemblies()
        {
            return inspectedPlugin.ReferencedAssemblies.Select(assembly => Load(assembly.AssemblyName)!);
        }

        /// <inheritdoc />
        protected override Assembly? Load(AssemblyName name)
        {
            Assembly? assembly = LoadInternal(name);

            return assembly;
        }

        private static readonly ConcurrentDictionary<AssemblyName, Assembly?> Cache = new ConcurrentDictionary<AssemblyName, Assembly?>();
        
        private Assembly? LoadInternal(AssemblyName name)
        {
            return Cache.GetOrAdd(name, n => FindAssembly(n));
        }

        private Assembly? FindAssembly(AssemblyName name)
        {
            var assemblyFromParent =
                providers
                    .Select(x => new
                    {
                        Success = x.TryLoad(name, out Assembly? assembly),
                        Assembly = assembly,
                        Provider = x.Name
                    })
                    .FirstOrDefault(x => x.Success);

            if (assemblyFromParent != null)
            {
                return assemblyFromParent.Assembly;
            }

            if (Assemblies.Any(x => x.GetName().IsCompatibleWith(name)))
            {
                return Assemblies.Single(x => x.GetName().IsCompatibleWith(name));
            }

            Assembly? result = null;
            string? assemblyPath = resolver.ResolveAssemblyToPath(name);
            if (assemblyPath != null)
            {
                result = LoadFromAssemblyPath(assemblyPath);
            }

            return result;
        }

        /// <inheritdoc />
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string assemblyPath = Path.GetDirectoryName(inspectedPlugin.Path)!;

            string fullPath = DetermineFullPath(unmanagedDllName, assemblyPath);

            return File.Exists(fullPath) ? LoadUnmanagedDllFromPath(fullPath) : base.LoadUnmanagedDll(unmanagedDllName);
        }

        private static string DetermineFullPath(string unmanagedDllName, string assemblyPath)
        {
            string arch = Environment.Is64BitProcess ? "-x64" : "-x86";

            string fullPath;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                fullPath = Path.Combine(assemblyPath, "runtimes", "osx" + arch, "native", $"lib{unmanagedDllName}.dylib");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                fullPath = Path.Combine(assemblyPath, "runtimes", "linux" + arch, "native", $"lib{unmanagedDllName}.so");
            }
            else
            {
                if (unmanagedDllName.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
                {
                    unmanagedDllName = Path.GetFileNameWithoutExtension(unmanagedDllName);
                }

                fullPath = Path.Combine(assemblyPath, "runtimes", "win" + arch, "native", $"{unmanagedDllName}.dll");
                if (!File.Exists(fullPath))
                {
                    fullPath = Path.Combine(assemblyPath, "runtimes", "win7" + arch, "native", $"{unmanagedDllName}.dll");
                }
            }

            return fullPath;
        }
    }
}
