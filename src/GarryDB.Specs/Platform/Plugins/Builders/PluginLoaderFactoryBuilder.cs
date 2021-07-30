using GarryDB.Platform.Plugins;

namespace GarryDB.Specs.Platform.Plugins.Builders
{
    public sealed class PluginLoaderFactoryBuilder : TestDataBuilder<PluginLoaderFactory>
    {
        protected override PluginLoaderFactory OnBuild()
        {
            return new PluginLoaderFactory();
        }
    }
}
