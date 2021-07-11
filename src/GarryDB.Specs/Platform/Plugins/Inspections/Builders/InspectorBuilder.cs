using GarryDb.Platform.Infrastructure;
using GarryDb.Platform.Plugins.Inpections;
using GarryDb.Specs.Platform.Builders;

namespace GarryDb.Specs.Platform.Plugins.Inspections.Builders
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
            return new Inspector(fileSystem);
        }

        public InspectorBuilder Using(FileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
            return this;
        }
    }
}
