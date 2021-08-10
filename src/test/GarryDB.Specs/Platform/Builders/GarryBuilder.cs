using GarryDB.Platform;
using GarryDB.Platform.Infrastructure;
using GarryDB.Platform.Persistence;
using GarryDB.Specs.Platform.Infrastructure.Builders;
using GarryDB.Specs.Platform.Persistence.Builders;

namespace GarryDB.Specs.Platform.Builders
{
    internal sealed class GarryBuilder : TestDataBuilder<Garry>
    {
        private FileSystem fileSystem;
        private ConnectionFactory connectionFactory;

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
        }

        protected override Garry OnBuild()
        {
            return new Garry(fileSystem, connectionFactory);
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
    }
}
