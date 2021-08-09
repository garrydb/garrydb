using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using GarryDB.Platform.Infrastructure;

namespace GarryDB.Platform.Plugins
{
    internal sealed class PluginDirectory : IComparable<PluginDirectory>
    {
        private readonly FileSystem fileSystem;

        public PluginDirectory(FileSystem fileSystem, string directory)
        {
            this.fileSystem = fileSystem;
            Directory = directory;
        }

        public string PluginName
        {
            get { return Path.GetFileNameWithoutExtension(Directory); }
        }

        public string Directory { get; }

        public IEnumerable<string> ProvidedAssemblies
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
        
        public IEnumerable<string> DependentAssemblies
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

        public IEnumerable<string> Files
        {
            get { return fileSystem.GetFiles(Directory); }
        }

        public string? PluginAssembly
        {
            get { return fileSystem.GetFiles(Directory, $"{PluginName}.dll").SingleOrDefault(); }
        }

        public override string ToString()
        {
            return PluginName;
        }

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
    }
}