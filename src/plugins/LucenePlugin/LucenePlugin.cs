using GarryDb.Plugins;

namespace LucenePlugin
{
    public sealed class LuceneConfiguration
    {
        public string SomeData { get; set; } 
    }

    public sealed class LucenePlugin : ConfigurablePlugin<LuceneConfiguration>
    {
        public LucenePlugin(PluginContext pluginContext)
            : base(pluginContext)
        {
        }
    }
}
