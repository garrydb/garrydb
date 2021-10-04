using System.Collections.Generic;
using System.IO;
using System.Reflection;

using GarryDB.Platform.Plugins;

using UIPlugin.Shared;

namespace GarryDb.Avalonia.Host
{
    public sealed class UIPluginPackage : PluginPackage
    {
        public UIPluginPackage()
            : base(nameof(UIPlugin))
        {
        }

        public override IEnumerable<AssemblyName> Assemblies
        {
            get
            {
                yield return typeof(UIPlugin).Assembly.GetName();
                yield return typeof(Extension).Assembly.GetName();
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
