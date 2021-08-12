using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using GarryDB.Platform.Extensions;

#pragma warning disable 1591
namespace GarryDB.Platform.Plugins
{
    public abstract class PluginPackage : IComparable<PluginPackage>
    {
        protected PluginPackage(string name)
        {
            Name = name;
        }

        /// <summary>
        ///     Gets the name of the plugin.
        /// </summary>
        public string Name { get; }

        public abstract IEnumerable<AssemblyName> Assemblies { get; }

        public abstract Stream? ResolveAssembly(AssemblyName assemblyName);
        public abstract string? ResolveUnmanagedDllPath(string unmanagedDllName);
        
        /// <summary>
        ///     Determins whether this <see cref="PluginPackage" /> is dependent on <paramref name="pluginPackage" />.
        /// </summary>
        /// <param name="pluginPackage">The <see cref="PluginPackage" /> to check.</param>
        /// <returns>
        ///     <c>true</c> if this plugin depends on assemblies provided by <paramref name="pluginPackage" />,
        ///     otherwise <c>false</c>.
        /// </returns>
        public bool IsDependentOn(PluginPackage pluginPackage)
        {
            return DependentAssemblies.Any(dependent => pluginPackage.ProvidedAssemblies.Any(provided => provided.IsCompatibleWith(dependent)));
        }

        private IEnumerable<AssemblyName> ProvidedAssemblies
        {
            get
            {
                return Assemblies
                    .Where(assemblyName => string.Equals(assemblyName.Name, $"{Name}.Contract", StringComparison.OrdinalIgnoreCase) ||
                                           string.Equals(assemblyName.Name, $"{Name}.Shared", StringComparison.OrdinalIgnoreCase));
            }
        }

        private IEnumerable<AssemblyName> DependentAssemblies
        {
            get
            {
                return Assemblies
                    .Where(assemblyName =>
                        (assemblyName.Name?.EndsWith(".Contract", StringComparison.InvariantCultureIgnoreCase) ?? false) ||
                        (assemblyName.Name?.EndsWith(".Shared", StringComparison.InvariantCultureIgnoreCase) ?? false))
                    .Except(ProvidedAssemblies);
            }
        }

        /// <inheritdoc />
        public int CompareTo(PluginPackage? other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            if (IsDependentOn(other))
            {
                return 1;
            }

            if (other.IsDependentOn(this))
            {
                return -1;
            }

            return 0;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }
    }
}
