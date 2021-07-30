using Akka.Actor;

using GarryDB.Platform.Plugins;
using GarryDB.Specs.Platform.Messaging.Builders;

namespace GarryDB.Specs.Platform.Plugins.Builders
{
    public sealed class AkkaPluginContextBuilder : TestDataBuilder<AkkaPluginContext>
    {
        private IActorRef plugins;
        private PluginIdentity pluginIdentity;

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
