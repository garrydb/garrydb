using System.Threading.Tasks;

using GarryDB.Plugins;

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
            return SendAsync("UIPlugin", "extend", new UIExtension());
        }
    }
}
