using System.Linq;

using Akka.TestKit.NUnit;

using GarryDB.Specs.NUnit.Extensions;

using NUnit.Framework.Internal;

namespace GarryDB.Specs.Akka.Builders
{
    public sealed class TestKitBuilder : TestDataBuilder<TestKit>
    {
        private const string TestKitKey = "testkit";

        protected override TestKit OnBuild()
        {
            TestKit existing = TestExecutionContext.CurrentContext.CurrentTest.Properties[TestKitKey].OfType<TestKit>()
                .SingleOrDefault();

            if (existing != null)
            {
                return existing;
            }

            var testKit = new TestKit(@"akka.scheduler.implementation = ""Akka.TestKit.TestScheduler, Akka.TestKit""");

            TestExecutionContext.CurrentContext.CurrentTest.Properties[TestKitKey].Add(testKit);
            testKit.InitializeActorSystemOnSetUp();

            return testKit;
        }

        protected override void OnPostBuild(TestKit subject)
        {
            TestExecutionContext.CurrentContext.DisposeAfterTestRun(subject);
        }
    }
}
