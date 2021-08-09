using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using GarryDB.Platform.Extensions;
using GarryDB.Platform.Infrastructure;
using GarryDB.Plugins;

namespace GarryDB.Platform.Plugins
{
    /// <summary>
    ///     Contains information about the directory where a <see cref="Plugin" /> is stored.
    /// </summary>
    internal sealed class PluginDirectory : IComparable<PluginDirectory>
    {
        private readonly FileSystem fileSystem;

        /// <summary>
        ///     Initializes a new <see cref="PluginDirectory" />.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="directory">The directory containing the plugin.</param>
        public PluginDirectory(FileSystem fileSystem, string directory)
        {
            this.fileSystem = fileSystem;
            Directory = directory;
            PluginName = Path.GetFileNameWithoutExtension(Directory);
        }

        /// <summary>
        ///     Gets the name of the plugin.
        /// </summary>
        public string PluginName { get; }

        /// <summary>
        ///     Gets the directory where the plugin is stored.
        /// </summary>
        public string Directory { get; }

        /// <summary>
        ///     Determins whether this <see cref="PluginDirectory" /> is dependent on <paramref name="pluginDirectory" />.
        /// </summary>
        /// <param name="pluginDirectory">The <see cref="PluginDirectory" /> to check.</param>
        /// <returns>
        ///     <c>true</c> if this plugin depends on assemblies provided by <paramref name="pluginDirectory" />,
        ///     otherwise <c>false</c>.
        /// </returns>
        public bool IsDependentOn(PluginDirectory pluginDirectory)
        {
            return DependentAssemblies.Any(assembly => pluginDirectory.ProvidedAssemblies.Contains(assembly));
        }

        /// <summary>
        ///     Load the plugin into <paramref name="pluginLoadContext" />.
        /// </summary>
        /// <param name="pluginLoadContext">The plugin load context to load the assemblies into.</param>
        public void LoadInto(PluginLoadContext pluginLoadContext)
        {
            fileSystem.GetFiles(Directory, "*.dll")
                      .Select(file => Path.GetFileNameWithoutExtension(file))
                      .ForEach(file => pluginLoadContext.LoadFromAssemblyName(new AssemblyName(file)));
        }

        private IEnumerable<string> ProvidedAssemblies
        {
            get
            {
                return fileSystem.GetFiles(Directory)
                                 .Select(path => Path.GetFileName(path))
                                 .Where(filename =>
                                        {
                                            return filename.Equals($"{PluginName}.Contract.dll", StringComparison.InvariantCultureIgnoreCase) ||
                                                   filename.Equals($"{PluginName}.Shared.dll", StringComparison.InvariantCultureIgnoreCase);
                                        });
            }
        }
        
        private IEnumerable<string> DependentAssemblies
        {
            get
            {
                return fileSystem.GetFiles(Directory)
                                 .Select(path => Path.GetFileName(path))
                                 .Where(filename =>
                                        {
                                            return filename.EndsWith(".Contract.dll", StringComparison.InvariantCultureIgnoreCase) ||
                                                   filename.EndsWith(".Shared.dll", StringComparison.InvariantCultureIgnoreCase);
                                        })
                                 .Except(ProvidedAssemblies);
            }
        }

        /// <inheritdoc />
        public int CompareTo(PluginDirectory? other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            if (DependentAssemblies.Any(assembly => other.ProvidedAssemblies.Contains(assembly)))
            {
                return 1;
            }

            if (other.DependentAssemblies.Any(assembly => ProvidedAssemblies.Contains(assembly)))
            {
                return -1;
            }

            return 0;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return PluginName;
        }
    }
}
