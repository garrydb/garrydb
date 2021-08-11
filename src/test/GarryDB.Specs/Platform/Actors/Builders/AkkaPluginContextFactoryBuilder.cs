using Akka.Actor;

using GarryDB.Platform.Actors;

namespace GarryDB.Specs.Platform.Actors.Builders
{
    internal sealed class AkkaPluginContextFactoryBuilder : TestDataBuilder<AkkaPluginContextFactory>
    {
        private IActorRef pluginsActor;

        protected override void OnPreBuild()
        {
            if (pluginsActor == null)
            {
                Using(new PluginsActorBuilder().Build());
            }
        }

        protected override AkkaPluginContextFactory OnBuild()
        {
            return new AkkaPluginContextFactory(pluginsActor);
        }

        public AkkaPluginContextFactoryBuilder Using(IActorRef pluginsActor)
        {
            this.pluginsActor = pluginsActor;
            return this;
        }
    }
}
