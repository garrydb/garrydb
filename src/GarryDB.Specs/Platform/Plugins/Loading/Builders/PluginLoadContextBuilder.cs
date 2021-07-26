using GarryDb.Platform.Plugins.Inpections;
using GarryDb.Platform.Plugins.Loading;
using GarryDb.Specs;
using GarryDb.Specs.Platform.Plugins.Inspections.Builders;

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
