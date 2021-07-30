using GarryDB.Platform.Plugins.Inpections;
using GarryDB.Platform.Plugins.Loading;
using GarryDB.Specs.Platform.Plugins.Inspections.Builders;

namespace GarryDB.Specs.Platform.Plugins.Loading.Builders
{
    public sealed class PluginLoaderBuilder : TestDataBuilder<PluginLoader>
    {
        private InspectedPlugin inspectedPlugin;

        protected override void OnPreBuild()
        {
            if (inspectedPlugin == null)
            {
                ThatLoadsPlugin(new InspectedPluginBuilder().Build());
            }
        }

        protected override PluginLoader OnBuild()
        {
            return new PluginLoader(inspectedPlugin);
        }

        public PluginLoaderBuilder ThatLoadsPlugin(InspectedPlugin inspectedPlugin)
        {
            this.inspectedPlugin = inspectedPlugin;
            return this;
        }
    }
}
