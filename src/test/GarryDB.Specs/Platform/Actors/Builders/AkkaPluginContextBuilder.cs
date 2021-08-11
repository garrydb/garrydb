using Akka.Actor;

using GarryDB.Platform.Actors;
using GarryDB.Platform.Plugins;
using GarryDB.Specs.Platform.Plugins.Builders;

namespace GarryDB.Specs.Platform.Actors.Builders
{
    internal sealed class AkkaPluginContextBuilder : TestDataBuilder<AkkaPluginContext>
    {
        private PluginIdentity pluginIdentity;
        private IActorRef plugins;

        protected override void OnPreBuild()
        {
            if (plugins == null)
            {
                WithPluginsActor(new PluginsActorBuilder().Build());
            }

            if (pluginIdentity == null)
            {
                ForPlugin(new PluginIdentityBuilder().Build());
            }
        }

        protected override AkkaPluginContext OnBuild()
        {
            return new AkkaPluginContext(plugins, pluginIdentity);
        }

        public AkkaPluginContextBuilder WithPluginsActor(IActorRef plugins)
        {
            this.plugins = plugins;

            return this;
        }

        public AkkaPluginContextBuilder ForPlugin(PluginIdentity pluginIdentity)
        {
            this.pluginIdentity = pluginIdentity;

            return this;
        }
    }
}
