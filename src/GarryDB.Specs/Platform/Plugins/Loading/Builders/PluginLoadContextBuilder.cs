using GarryDB.Platform.Plugins.Inpections;
using GarryDB.Platform.Plugins.Loading;
using GarryDB.Specs.Platform.Plugins.Inspections.Builders;

namespace GarryDB.Specs.Platform.Plugins.Loading.Builders
{
    public sealed class PluginLoadContextBuilder : TestDataBuilder<PluginLoadContext>
    {
        private InspectedPlugin inspectedPlugin;

        protected override void OnPreBuild()
        {
            if (inspectedPlugin == null)
            {
                ForPlugin(new InspectedPluginBuilder().Build());
            }
        }

        protected override PluginLoadContext OnBuild()
        {
            return new PluginLoadContext(inspectedPlugin);
        }

        public PluginLoadContextBuilder ForPlugin(InspectedPlugin inspectedPlugin)
        {
            this.inspectedPlugin = inspectedPlugin;
            return this;
        }
    }
}
