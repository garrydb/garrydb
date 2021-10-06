using System.Collections.Generic;
using System.Linq;

using GarryDb.Platform.Infrastructure;

namespace GarryDb.Platform.Plugins.PackageSources
{
    /// <summary>
    ///     Finds the plugin packages in a directory on the file system.
    /// </summary>
    public sealed class FromDirectoryPluginPackageSource : PluginPackageSource
    {
        private readonly FileSystem fileSystem;
        private readonly string pluginsDirectory;

        /// <summary>
        ///     Initializes a new <see cref="FromDirectoryPluginPackageSource" />.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="pluginsDirectory">The directory containing plugins.</param>
        public FromDirectoryPluginPackageSource(FileSystem fileSystem, string pluginsDirectory)
        {
            this.fileSystem = fileSystem;
            this.pluginsDirectory = pluginsDirectory;
        }

        /// <inheritdoc />
        public IEnumerable<PluginPackage> PluginPackages
        {
            get
            {
                return
                    fileSystem.GetTopLevelDirectories(pluginsDirectory)
                        .Select(directory => (PluginPackage)new PluginDirectory(fileSystem, directory))
                        .ToList();
            }
        }
    }
}
