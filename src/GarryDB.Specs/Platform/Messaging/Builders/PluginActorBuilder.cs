using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.NUnit;

using GarryDB.Platform.Messaging;
using GarryDB.Platform.Plugins;
using GarryDB.Plugins;
using GarryDB.Specs.Akka.Builders;
using GarryDB.Specs.Platform.Plugins.Builders;

using GarryDB.Specs.Plugins.Builders;

namespace GarryDB.Specs.Platform.Messaging.Builders
{
    public sealed class PluginActorBuilder : TestDataBuilder<IActorRef>
    {
        private PluginIdentity pluginIdentity;
        private Plugin plugin;
        private TestKit testKit;

        protected override void OnPreBuild()
        {
            if (pluginIdentity == null && plugin == null)
            {
                For(new PluginIdentityBuilder().Build(), new PluginBuilder().Build());
            }

            if (testKit == null)
            {
                Using(new TestKitBuilder().Build());
            }
        }

        protected override IActorRef OnBuild()
        {
            return testKit.Sys.ActorOf(PluginActor.Props(pluginIdentity, plugin).WithDispatcher(CallingThreadDispatcher.Id));
        }

        public PluginActorBuilder For(PluginIdentity pluginIdentity, Plugin plugin)
        {
            this.pluginIdentity = pluginIdentity;
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
