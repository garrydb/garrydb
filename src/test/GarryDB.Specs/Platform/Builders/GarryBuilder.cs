using GarryDB.Platform;
using GarryDB.Platform.Infrastructure;
using GarryDB.Platform.Plugins;
using GarryDB.Platform.Plugins.Configuration;
using GarryDB.Platform.Plugins.Lifecycles;
using GarryDB.Specs.Platform.Infrastructure.Builders;
using GarryDB.Specs.Platform.Plugins.Builders;
using GarryDB.Specs.Platform.Plugins.Configuration.Builders;

namespace GarryDB.Specs.Platform.Builders
{
    internal sealed class GarryBuilder : TestDataBuilder<Garry>
    {
        private FileSystem fileSystem;
        private PluginContextFactory pluginContextFactory;
        private ConfigurationStorage configurationStorage;

        protected override void OnPreBuild()
        {
            if (fileSystem == null)
            {
                Using(new FileSystemBuilder().Build());
            }

            if (pluginContextFactory == null)
            {
                Using(new PluginContextFactoryBuilder().Build());
            }

            if (configurationStorage == null)
            {
                Using(new ConfigurationStorageBuilder().Build());
            }
        }

        protected override Garry OnBuild()
        {
            return new Garry(new DefaultPluginLifecycle(fileSystem, configurationStorage));
        }

        public GarryBuilder Using(FileSystem fileSystem)
        {
            this.fileSystem = fileSystem;

            return this;
        }

        public GarryBuilder Using(PluginContextFactory pluginContextFactory)
        {
            this.pluginContextFactory = pluginContextFactory;

            return this;
        }

        public GarryBuilder Using(ConfigurationStorage configurationStorage)
        {
            this.configurationStorage = configurationStorage;

            return this;
        }
    }
}
