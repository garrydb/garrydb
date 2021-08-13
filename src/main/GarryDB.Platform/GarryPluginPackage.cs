using System.Collections.Generic;
using System.IO;
using System.Reflection;

using GarryDB.Platform.Plugins;
using GarryDB.Plugins;

[assembly: StartupOrder(int.MinValue)]

namespace GarryDB.Platform
{
    internal sealed class GarryPluginPackage : PluginPackage
    {
        public GarryPluginPackage()
            : base(nameof(GarryPlugin))
        {
        }

        public override IEnumerable<AssemblyName> Assemblies
        {
            get
            {
                yield return typeof(GarryPlugin).Assembly.GetName();
            }
        }

        public override Stream? ResolveAssembly(AssemblyName assemblyName)
        {
            return null;
        }

        public override Stream? ResolveAssemblySymbols(AssemblyName assemblyName)
        {
            return null;
        }

        public override string? ResolveUnmanagedDllPath(string unmanagedDllName)
        {
            return null;
        }
    }
}
