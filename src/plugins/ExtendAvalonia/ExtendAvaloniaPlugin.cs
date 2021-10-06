using System.Threading.Tasks;

using GarryDb.Plugins;

namespace ExtendAvalonia
{
    public sealed class ExtendAvaloniaPlugin : Plugin
    {
        public ExtendAvaloniaPlugin(PluginContext pluginContext)
            : base(pluginContext)
        {
        }

        protected override Task StartAsync()
        {
            return SendAsync("UI", "extend", new UIExtension());
        }
    }
}
