using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.NUnit;

using GarryDB.Platform.Actors;
using GarryDB.Plugins;
using GarryDB.Specs.Akka.Builders;
using GarryDB.Specs.Plugins.Builders;

namespace GarryDB.Specs.Platform.Actors.Builders
{
    internal sealed class PluginActorBuilder : TestDataBuilder<IActorRef>
    {
        private Plugin plugin;
        private TestKit testKit;

        protected override void OnPreBuild()
        {
            if (plugin == null)
            {
                For(new PluginBuilder().Build());
            }

            if (testKit == null)
            {
                Using(new TestKitBuilder().Build());
            }
        }

        protected override IActorRef OnBuild()
        {
            return testKit.Sys.ActorOf(PluginActor.Props(plugin).WithDispatcher(CallingThreadDispatcher.Id));
        }

        public PluginActorBuilder For(Plugin plugin)
        {
            this.plugin = plugin;

            return this;
        }

        public PluginActorBuilder Using(TestKit testKit)
        {
            this.testKit = testKit;

            return this;
        }
    }
}
