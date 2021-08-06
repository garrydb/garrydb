using ExamplePlugin.Shared;

using GarryDB.Plugins;

namespace DependsOnExamplePlugin
{
    internal sealed class DependsOnExample : Plugin
    {
        public DependsOnExample(PluginContext pluginContext)
            : base(pluginContext)
        {
            Shared = new ExampleShared();
        }

        public ExampleShared Shared { get; set; }
    }
}
