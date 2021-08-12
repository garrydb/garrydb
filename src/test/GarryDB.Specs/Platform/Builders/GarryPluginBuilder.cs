using GarryDB.Platform;
using GarryDB.Plugins;
using GarryDB.Specs.Plugins.Builders;

namespace GarryDB.Specs.Platform.Builders
{
    internal sealed class GarryPluginBuilder : TestDataBuilder<GarryPlugin>
    {
        private PluginContext pluginContext;

        protected override void OnPreBuild()
        {
            if (pluginContext == null)
            {
                Using(new PluginContextBuilder().Named(GarryPlugin.PluginIdentity.Name).Build());
            }
        }

        protected override GarryPlugin OnBuild()
        {
            return new GarryPlugin(pluginContext);
        }

        public GarryPluginBuilder Using(PluginContext pluginContext)
        {
            this.pluginContext = pluginContext;

            return this;
        }
    }
}
