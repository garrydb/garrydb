using GarryDB.Platform.Plugins;
using GarryDB.Platform.Plugins.Loading;
using GarryDB.Specs.Builders.Randomized;
using GarryDB.Specs.Platform.Plugins.Builders;

namespace GarryDB.Specs.Platform.Plugins.Loading.Builders
{
    public sealed class LoadedPluginBuilder : TestDataBuilder<LoadedPlugin>
    {
        private PluginIdentity pluginIdentity;
        private int startupOrder;

        protected override void OnPreBuild()
        {
            if (pluginIdentity == null)
            {
                ForPlugin(new PluginIdentityBuilder().Build());
            }

            if (startupOrder == 0)
            {
                WithStartupOrder(new RandomIntegerBuilder().Build());
            }
        }

        protected override LoadedPlugin OnBuild()
        {
            return new(pluginIdentity, startupOrder);
        }

        public LoadedPluginBuilder ForPlugin(PluginIdentity pluginIdentity)
        {
            this.pluginIdentity = pluginIdentity;

            return this;
        }

        public LoadedPluginBuilder WithStartupOrder(int startupOrder)
        {
            this.startupOrder = startupOrder;

            return this;
        }
    }
}
