using ExamplePlugin.Shared;

using GarryDb.Plugins;

namespace DependsOnExamplePlugin
{
    public class DependsOnExample : Plugin
    {
        public DependsOnExample(PluginContext pluginContext)
            : base(pluginContext)
        {
            Shared = new ExampleShared();
        }

        public ExampleShared Shared { get; set; }
    }
}
