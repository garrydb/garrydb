using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

using GarryDb.Platform.Infrastructure;
using GarryDb.Plugins;

namespace GarryDb.Platform.Plugins
{
    /// <summary>
    ///     Contains information about the directory where a <see cref="Plugin" /> is stored.
    /// </summary>
    internal sealed class PluginDirectory : PluginPackage
    {
        private readonly FileSystem fileSystem;
        private readonly AssemblyDependencyResolver resolver;
        private readonly string directory;

        /// <summary>
        ///     Initializes a new <see cref="PluginDirectory" />.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="directory">The directory containing the plugin.</param>
        public PluginDirectory(FileSystem fileSystem, string directory)
            : base(Path.GetFileNameWithoutExtension(directory))
        {
            this.fileSystem = fileSystem;
            this.directory = directory;
            resolver = new AssemblyDependencyResolver(Path.Combine(directory, $"{Name}.dll"));
            Assemblies =
                fileSystem.GetFiles(directory, $"{Name}.dll")
                    .Concat(fileSystem.GetFiles(directory, "*.Contract.dll"))
                    .Concat(fileSystem.GetFiles(directory, "*.Shared.dll"))
                    .Select(file => AssemblyName.GetAssemblyName(file))
                    .ToList();
        }

        /// <inheritdoc />
        public override IEnumerable<AssemblyName> Assemblies { get; }

        /// <inheritdoc />
        public override Stream? ResolveAssembly(AssemblyName assemblyName)
        {
            string? assemblyPath = resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return fileSystem.LoadFile(assemblyPath);
            }

            return null;
        }

        /// <inheritdoc />
        public override Stream? ResolveAssemblySymbols(AssemblyName assemblyName)
        {
            string? assemblyPath = resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath == null)
            {
                return null;
            }

            string pdb = Path.ChangeExtension(assemblyPath, "pdb");

            if (fileSystem.Exists(pdb))
            {
                return fileSystem.LoadFile(assemblyPath);
            }

            return null;
        }

        /// <inheritdoc />
        public override string? ResolveUnmanagedDllPath(string unmanagedDllName)
        {
            string arch = Environment.Is64BitProcess ? "-x64" : "-x86";

            string fullPath;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                fullPath = Path.Combine(directory, "runtimes", "osx" + arch, "native", $"lib{unmanagedDllName}.dylib");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                fullPath = Path.Combine(directory, "runtimes", "linux" + arch, "native", $"lib{unmanagedDllName}.so");
            }
            else
            {
                if (unmanagedDllName.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
                {
                    unmanagedDllName = Path.GetFileNameWithoutExtension(unmanagedDllName);
                }

                fullPath = Path.Combine(directory, "runtimes", "win" + arch, "native", $"{unmanagedDllName}.dll");
                if (!File.Exists(fullPath))
                {
                    fullPath = Path.Combine(directory, "runtimes", "win7" + arch, "native", $"{unmanagedDllName}.dll");
                }
            }

            return File.Exists(fullPath) ? fullPath : null;
        }
    }
}
