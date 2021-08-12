using GarryDB.Platform.Infrastructure;
using GarryDB.Platform.Plugins;
using GarryDB.Specs.Builders.Randomized;
using GarryDB.Specs.Platform.Infrastructure.Builders;

namespace GarryDB.Specs.Platform.Plugins.Builders
{
    internal sealed class PluginDirectoryBuilder : TestDataBuilder<PluginDirectory>
    {
        private FileSystem fileSystem;
        private string directory;

        protected override void OnPreBuild()
        {
            if (fileSystem == null)
            {
                Using(new FileSystemBuilder().Build());
            }

            if (directory == null)
            {
                InDirectory(new RandomStringBuilder().Build());
            }
        }

        protected override PluginDirectory OnBuild()
        {
            return new PluginDirectory(fileSystem, directory);
        }

        public PluginDirectoryBuilder Using(FileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
            return this;
        }

        public PluginDirectoryBuilder InDirectory(string directory)
        {
            this.directory = directory;
            return this;
        }
    }
}
