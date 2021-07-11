using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

using GarryDb.Platform.Extensions;
using GarryDb.Platform.Plugins.Inpections;

namespace GarryDb.Platform.Plugins.Loading
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class PluginLoadContext : AssemblyLoadContext
    {
        private readonly InspectedPlugin inspectedPlugin;
        private readonly IList<AssemblyLoadContext> providers;
        private readonly AssemblyDependencyResolver resolver;

        /// <summary>
        ///     
        /// </summary>
        /// <param name="inspectedPlugin"></param>
        public PluginLoadContext(InspectedPlugin inspectedPlugin)
            : base(inspectedPlugin.PluginAssembly.PluginIdentity.Name)
        {
            providers = new List<AssemblyLoadContext>();
            resolver = new AssemblyDependencyResolver(inspectedPlugin.Path);
            this.inspectedPlugin = inspectedPlugin;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        public void AddProvider(AssemblyLoadContext provider)
        {
            providers.Add(provider);
        }

        /// <inheritdoc />
        protected override Assembly? Load(AssemblyName name)
        {
            if (Assemblies.Any(x => x.GetName().IsCompatibleWith(name)))
            {
                return Assemblies.Single(x => x.GetName().IsCompatibleWith(name));
            }

            Assembly? assemblyFromParent =
                providers.Select(x =>
                    {
                        try
                        {
                            return x.LoadFromAssemblyName(name);
                        }
                        catch (FileNotFoundException)
                        {
                            return null;
                        }
                    })
                    .FirstOrDefault(x => x != null);
            
            if (assemblyFromParent != null)
            {
                return assemblyFromParent;
            }

            ProvidedAssembly? providedAssembly = inspectedPlugin.ProvidedAssemblies.SingleOrDefault(p => name.IsCompatibleWith(p.AssemblyName));
            if (providedAssembly != null)
            {
                return LoadFromStream(providedAssembly.Load());
            }

            ReferencedAssembly? referencedAssembly = inspectedPlugin.ReferencedAssemblies.SingleOrDefault(p => name.IsCompatibleWith(p.AssemblyName));
            if (referencedAssembly != null)
            {
                return LoadFromStream(referencedAssembly.Load());
            }

            string? assemblyPath = resolver.ResolveAssemblyToPath(name);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string assemblyPath = Path.GetDirectoryName(inspectedPlugin.Path)!;
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
            else // RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            {
                fullPath = Path.Combine(assemblyPath, "runtimes", "win" + arch, "native", $"{unmanagedDllName}.dll");
            }

            if (File.Exists(fullPath))
            {
                IntPtr result = LoadUnmanagedDllFromPath(fullPath);
                return result;
            }

            return IntPtr.Zero;
        }
    }
}
