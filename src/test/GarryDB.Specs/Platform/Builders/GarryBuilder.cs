using GarryDB.Platform;
using GarryDB.Platform.Infrastructure;
using GarryDB.Platform.Persistence;
using GarryDB.Platform.Plugins;
using GarryDB.Platform.Plugins.Configuration;
using GarryDB.Platform.Plugins.Lifecycles;
using GarryDB.Specs.Platform.Infrastructure.Builders;
using GarryDB.Specs.Platform.Persistence.Builders;
using GarryDB.Specs.Platform.Plugins.Builders;

namespace GarryDB.Specs.Platform.Builders
{
    internal sealed class GarryBuilder : TestDataBuilder<Garry>
    {
        private FileSystem fileSystem;
        private ConnectionFactory connectionFactory;
        private PluginContextFactory pluginContextFactory;

        protected override void OnPreBuild()
        {
            if (fileSystem == null)
            {
                Using(new FileSystemBuilder().Build());
            }

            if (connectionFactory == null)
            {
                Using(new ConnectionFactoryBuilder().Build());
            }

            if (pluginContextFactory == null)
            {
                Using(new PluginContextFactoryBuilder().Build());
            }
        }

        protected override Garry OnBuild()
        {
            return new Garry(new DefaultPluginLifecycle(fileSystem, new ConfigurationStorage(connectionFactory)));
        }

        public GarryBuilder Using(FileSystem fileSystem)
        {
            this.fileSystem = fileSystem;

            return this;
        }

        public GarryBuilder Using(ConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;

            return this;
        }

        public GarryBuilder Using(PluginContextFactory pluginContextFactory)
        {
            this.pluginContextFactory = pluginContextFactory;

            return this;
        }
    }
}
