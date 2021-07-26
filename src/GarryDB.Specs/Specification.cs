using System;

using GarryDB.Specs.NUnit.Extensions;

using NUnit.Framework;
using NUnit.Framework.Internal;

namespace GarryDb.Specs
{
    public abstract class Specification<TSubject>
    {
        private TSubject subject;
        
        protected Specification()
        {
            Setup();
        }

        [SetUp]
        public void Setup()
        {
            subject = Given();
            When(subject);
        }

        protected virtual TSubject Given()
        {
            return default;
        }

        protected virtual void When(TSubject subject)
        {
        }

        [TearDown]
        public void TearDown()
        {
            CleanUp();

            TestExecutionContext.CurrentContext.DisposeAll();

            if (subject is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        protected virtual void CleanUp()
        {
        }
    }
}
