using GarryDb.Platform.Plugins.Inpections;

namespace GarryDb.Specs.Platform.Plugins.Inspections.Extensions
{
    public static class InspectedPluginExtensions
    {
        public static InspectedPluginAssertions Should(this InspectedPlugin subject)
        {
            return new InspectedPluginAssertions(subject);
        }
    }
}
