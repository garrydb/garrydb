using System.Collections.Generic;
using System.IO;
using System.Reflection;

using GarryDb.Platform.Plugins;
using GarryDb.Plugins;

[assembly: StartupOrder(int.MinValue)]

namespace GarryDb.Platform
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class GarryPluginPackage : PluginPackage
    {
        /// <summary>
        /// 
        /// </summary>
        public GarryPluginPackage()
            : base(nameof(GarryPlugin))
        {
        }

        /// <inheritdoc />
        public override IEnumerable<AssemblyName> Assemblies
        {
            get
            {
                yield return typeof(GarryPlugin).Assembly.GetName();
            }
        }

        /// <inheritdoc />
        public override Stream? ResolveAssembly(AssemblyName assemblyName)
        {
            return null;
        }

        /// <inheritdoc />
        public override Stream? ResolveAssemblySymbols(AssemblyName assemblyName)
        {
            return null;
        }

        /// <inheritdoc />
        public override string? ResolveUnmanagedDllPath(string unmanagedDllName)
        {
            return null;
        }
    }
}
