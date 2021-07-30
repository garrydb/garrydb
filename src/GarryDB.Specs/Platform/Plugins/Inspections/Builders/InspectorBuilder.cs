using GarryDB.Platform.Infrastructure;
using GarryDB.Platform.Plugins.Inpections;
using GarryDB.Specs.Platform.Infrastructure.Builders;

namespace GarryDB.Specs.Platform.Plugins.Inspections.Builders
{
    public sealed class InspectorBuilder : TestDataBuilder<Inspector>
    {
        private FileSystem fileSystem;

        protected override void OnPreBuild()
        {
            if (fileSystem == null)
            {
                Using(new FileSystemBuilder().Build());
            }
        }

        protected override Inspector OnBuild()
        {
            return new(fileSystem);
        }

        public InspectorBuilder Using(FileSystem fileSystem)
        {
            this.fileSystem = fileSystem;

            return this;
        }
    }
}
