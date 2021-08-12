using GarryDB.Platform.Persistence;
using GarryDB.Platform.Plugins.Configuration;
using GarryDB.Specs.Platform.Persistence.Builders;

namespace GarryDB.Specs.Platform.Plugins.Configuration.Builders
{
    internal sealed class ConfigurationStorageBuilder : TestDataBuilder<ConfigurationStorage>
    {
        private ConnectionFactory connectionFactory;

        protected override void OnPreBuild()
        {
            if (connectionFactory == null)
            {
                Using(new ConnectionFactoryBuilder().Build());
            }
        }

        protected override ConfigurationStorage OnBuild()
        {
            return new ConfigurationStorage(connectionFactory);
        }

        public ConfigurationStorageBuilder Using(ConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
            return this;
        }
    }
}
