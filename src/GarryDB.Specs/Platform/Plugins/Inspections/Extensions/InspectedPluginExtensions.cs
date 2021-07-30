using GarryDB.Platform.Plugins.Inpections;

namespace GarryDB.Specs.Platform.Plugins.Inspections.Extensions
{
    public static class InspectedPluginExtensions
    {
        public static InspectedPluginAssertions Should(this InspectedPlugin subject)
        {
            return new(subject);
        }
    }
}
