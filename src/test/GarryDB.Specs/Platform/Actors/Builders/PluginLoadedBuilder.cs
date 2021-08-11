using GarryDB.Platform.Actors;
using GarryDB.Platform.Plugins;
using GarryDB.Plugins;
using GarryDB.Specs.Platform.Plugins.Builders;
using GarryDB.Specs.Plugins.Builders;

namespace GarryDB.Specs.Platform.Actors.Builders
{
    internal sealed class PluginLoadedBuilder : TestDataBuilder<PluginLoaded>
    {
        private Plugin plugin;
        private PluginIdentity pluginIdentity;

        protected override void OnPreBuild()
        {
            if (pluginIdentity == null && plugin == null)
            {
                For(new PluginIdentityBuilder().Build(), new PluginBuilder().Build());
            }
        }

        protected override PluginLoaded OnBuild()
        {
            return new PluginLoaded(pluginIdentity, plugin);
        }

        public PluginLoadedBuilder For(PluginIdentity pluginIdentity, Plugin plugin)
        {
            this.pluginIdentity = pluginIdentity;
            this.plugin = plugin;

            return this;
        }
    }
}
