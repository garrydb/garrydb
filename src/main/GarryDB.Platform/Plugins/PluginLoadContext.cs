using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

using GarryDB.Platform.Extensions;
using GarryDB.Platform.Plugins.Extensions;
using GarryDB.Plugins;

namespace GarryDB.Platform.Plugins
{
    /// <summary>
    ///     A dedicated <see cref="AssemblyLoadContext" /> for loading assemblies for a <see cref="Plugin" />.
    /// </summary>
    internal sealed class PluginLoadContext : AssemblyLoadContext
    {
        private readonly IEnumerable<AssemblyLoadContext> providers;
        private readonly AssemblyDependencyResolver resolver;

        /// <summary>
        ///     Initializes a new <see cref="PluginLoadContext" />.
        /// </summary>
        /// <param name="pluginDirectory">The directory containing the .dlls.</param>
        /// <param name="providers">The <see cref="AssemblyLoadContext" />s to use for referenced assemblies.</param>
        public PluginLoadContext(PluginDirectory pluginDirectory, IEnumerable<AssemblyLoadContext> providers)
            : base(pluginDirectory.PluginName)
        {
            PluginDirectory = pluginDirectory;
            this.providers = providers;
            resolver = new AssemblyDependencyResolver(
                Path.Combine(pluginDirectory.Directory, $"{pluginDirectory.PluginName}.dll"));
        }

        /// <summary>
        ///     Gets the plugin directory.
        /// </summary>
        public PluginDirectory PluginDirectory { get; }

        /// <summary>
        ///     Loads the assembly that contains a <see cref="Plugin" />.
        /// </summary>
        /// <returns>The <see cref="PluginAssembly" />.</returns>
        public PluginAssembly? Load()
        {
            PluginDirectory.LoadInto(this);

            Assembly? assembly = Assemblies.SingleOrDefault(x => x.GetName().Name == PluginDirectory.PluginName);

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

            Assembly? result = null;
            string? assemblyPath = resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                result = LoadFromAssemblyPath(assemblyPath);
            }

            return result;
        }

        /// <inheritdoc />
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string fullPath = DetermineFullPath(unmanagedDllName, PluginDirectory.Directory);

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
