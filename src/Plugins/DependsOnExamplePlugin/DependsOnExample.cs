using ExamplePlugin.Shared;

using GarryDb.Plugins;

namespace DependsOnExamplePlugin
{
    public class DependsOnExample : Plugin
    {
        public DependsOnExample()
        {
            Shared = new ExampleShared();
        }

        public ExampleShared Shared { get; set; }
    }
}
