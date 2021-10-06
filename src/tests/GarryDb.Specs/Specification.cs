using GarryDB.Specs.NUnit.Extensions;

using NUnit.Framework;
using NUnit.Framework.Internal;

namespace GarryDB.Specs
{
    internal abstract class Specification
    {
        protected Specification()
        {
            Setup();
        }

        [SetUp]
        public void Setup()
        {
            Given();
            When();
        }

        protected virtual void Given()
        {
        }

        protected virtual void When()
        {
        }

        [TearDown]
        public void TearDown()
        {
            CleanUp();

            TestExecutionContext.CurrentContext.DisposeAll();
        }

        protected virtual void CleanUp()
        {
        }
    }
}
