using GarryDB.Platform.Messaging;

namespace GarryDB.Specs.Platform.Messaging.Builders
{
    public sealed class DeadletterMonitorBuilder : TestDataBuilder<DeadletterMonitor>
    {
        protected override DeadletterMonitor OnBuild()
        {
            return new();
        }
    }
}
