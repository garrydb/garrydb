using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using GarryDB.Platform.Extensions;
using GarryDB.Plugins;

namespace GarryDB.Platform.Plugins
{
    /// <summary>
    ///     A package containing a <see cref="Plugin" /> and other related files.
    /// </summary>
    public abstract class PluginPackage : IComparable<PluginPackage>
    {
        /// <summary>
        ///     Initializes a new <see cref="PluginPackage" />.
        /// </summary>
        /// <param name="name">The name of the package.</param>
        protected PluginPackage(string name)
        {
            Name = name;
        }

        /// <summary>
        ///     Gets the name of the plugin.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Gets the assemblies of this plugin.
        /// </summary>
        public abstract IEnumerable<AssemblyName> Assemblies { get; }

        /// <summary>
        ///     Resolve the specified <see cref="AssemblyName" /> to a <see cref="Stream" />.
        /// </summary>
        /// <param name="assemblyName">The assembly name.</param>
        /// <returns>A stream containing the assembly.</returns>
        public abstract Stream? ResolveAssembly(AssemblyName assemblyName);
        
        /// <summary>
        ///     Resolve the full path of an unmanaged .dll.
        /// </summary>
        /// <param name="unmanagedDllName">The name of the .dll.</param>
        /// <returns>The full path to the unmanaged .dll.</returns>
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

            return -1;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is PluginPackage that && Name == that.Name;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }
    }
}
