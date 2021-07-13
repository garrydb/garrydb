using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

using GarryDb.Platform.Extensions;
using GarryDb.Platform.Plugins.Inpections;

using GarryDB.Platform.Plugins.Loading.Extensions;

using GarryDb.Plugins;

namespace GarryDb.Platform.Plugins.Loading
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
        /// <param name="inspectedPlugin">The inpected plugin.</param>
        public PluginLoadContext(InspectedPlugin inspectedPlugin)
            : base(inspectedPlugin.PluginIdentity.ToString())
        {
            providers = new List<AssemblyLoadContext>();
            resolver = new AssemblyDependencyResolver(inspectedPlugin.Path);
            this.inspectedPlugin = inspectedPlugin;
        }

        /// <summary>
        ///     Add a provider of assemblies to this load context.
        /// </summary>
        /// <param name="provider">An <see cref="AssemblyLoadContext" /> that can provide references.</param>
        public void AddProvider(AssemblyLoadContext provider)
        {
            providers.Add(provider);
        }

        /// <inheritdoc />
        protected override Assembly? Load(AssemblyName name)
        {
            Assembly? assembly = LoadInternal(name);

            return assembly;
        }

        private Assembly? LoadInternal(AssemblyName name)
        {
            if (Assemblies.Any(x => x.GetName().IsCompatibleWith(name)))
            {
                return Assemblies.Single(x => x.GetName().IsCompatibleWith(name));
            }

            Assembly? assemblyFromParent =
                providers
                    .Select(x => new {Success = x.TryLoad(name, out Assembly? assembly), Assembly = assembly})
                    .Where(x => x.Success)
                    .Select(x => x.Assembly!)
                    .FirstOrDefault();

            if (assemblyFromParent != null)
            {
                return assemblyFromParent;
            }

            Assembly? result = null;
            ProvidedAssembly? providedAssembly =
                inspectedPlugin.ProvidedAssemblies.SingleOrDefault(p => name.IsCompatibleWith(p.AssemblyName));
            string? assemblyPath = resolver.ResolveAssemblyToPath(name);

            if (providedAssembly != null)
            {
                result = LoadFromStream(providedAssembly.Load());
            }
            else if (assemblyPath != null)
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
                fullPath = Path.Combine(assemblyPath, "runtimes", "osx" + arch, "native", $"{unmanagedDllName}.dylib");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                fullPath = Path.Combine(assemblyPath, "runtimes", "linux" + arch, "native", $"{unmanagedDllName}.so");
            }
            else
            {
                fullPath = Path.Combine(assemblyPath, "runtimes", "win" + arch, "native", $"{unmanagedDllName}.dll");
            }

            return fullPath;
        }
    }
}
