using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using GarryDB.Platform.Infrastructure;

namespace GarryDB.Specs.Platform.Infrastructure.Builders
{
    public sealed class FileSystemBuilder : TestDataBuilder<FileSystem>
    {
        private readonly IDictionary<string, string[]> files = new Dictionary<string, string[]>();

        protected override FileSystem OnBuild()
        {
            return new FileSystemStub(this);
        }

        protected override void OnPostBuild(FileSystem subject)
        {
            string runtimeDirectory = RuntimeEnvironment.GetRuntimeDirectory();
            WithFiles(runtimeDirectory, Directory.EnumerateFiles(runtimeDirectory, "*.dll").ToArray());
        }

        public FileSystemBuilder WithFiles(string directory, params string[] files)
        {
            this.files[directory.ToLowerInvariant()] = files;
            return this;
        }

        private class FileSystemStub : FileSystem
        {
            private readonly FileSystemBuilder builder;

            public FileSystemStub(FileSystemBuilder builder)
            {
                this.builder = builder;
            }

            public IEnumerable<string> GetFiles(string directory, string pattern = "*.*")
            {
                if (builder.files.ContainsKey(directory.ToLowerInvariant()))
                {
                    return builder.files[directory.ToLowerInvariant()].Select(file => Path.Combine(directory, file)).ToList();
                }

                return Enumerable.Empty<string>();
            }

            public IEnumerable<string> GetTopLevelDirectories(string directory)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
