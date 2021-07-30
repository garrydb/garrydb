using GarryDB.Platform;
using GarryDB.Platform.Infrastructure;
using GarryDB.Specs.Platform.Infrastructure.Builders;

namespace GarryDB.Specs.Platform.Builders
{
    public sealed class GarryBuilder : TestDataBuilder<Garry>
    {
        private FileSystem fileSystem;

        protected override void OnPreBuild()
        {
            if (fileSystem == null)
            {
                Using(new FileSystemBuilder().Build());
            }
        }

        protected override Garry OnBuild()
        {
            return new Garry(fileSystem);
        }

        public GarryBuilder Using(FileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
            return this;
        }
    }
}
