using GarryDB.Platform.Actors;

namespace GarryDB.Specs.Platform.Actors.Builders
{
    internal sealed class DeadletterMonitorBuilder : TestDataBuilder<DeadletterMonitor>
    {
        protected override DeadletterMonitor OnBuild()
        {
            return new DeadletterMonitor();
        }
    }
}
