using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.NUnit;

using GarryDB.Platform.Actors;
using GarryDB.Specs.Akka.Builders;

namespace GarryDB.Specs.Platform.Actors.Builders
{
    internal sealed class PluginsActorBuilder : TestDataBuilder<IActorRef>
    {
        private TestKit testKit;

        protected override void OnPreBuild()
        {
            if (testKit == null)
            {
                Using(new TestKitBuilder().Build());
            }
        }

        protected override IActorRef OnBuild()
        {
            return testKit.Sys.ActorOf(PluginsActor.Props().WithDispatcher(CallingThreadDispatcher.Id));
        }

        public PluginsActorBuilder Using(TestKit testKit)
        {
            this.testKit = testKit;

            return this;
        }
    }
}
