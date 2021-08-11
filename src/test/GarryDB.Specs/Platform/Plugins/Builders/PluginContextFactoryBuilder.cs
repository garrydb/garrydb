using GarryDB.Platform.Plugins;
using GarryDB.Plugins;
using GarryDB.Specs.Plugins.Builders;

namespace GarryDB.Specs.Platform.Plugins.Builders
{
    internal sealed class PluginContextFactoryBuilder : TestDataBuilder<PluginContextFactory>
    {
        protected override PluginContextFactory OnBuild()
        {
            return new PluginContextFactoryStub();
        }

        private sealed class PluginContextFactoryStub : PluginContextFactory
        {
            public PluginContext Create(PluginIdentity pluginIdentity)
            {
                return new PluginContextBuilder().Named(pluginIdentity.Name).Build();
            }
        }
    }
}
