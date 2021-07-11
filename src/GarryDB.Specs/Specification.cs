using System;

namespace GarryDb.Specs
{
    public abstract class Specification<TSubject> : IDisposable
    {
        private TSubject subject;
        
        protected Specification()
        {
            Setup();
        }

        public void Dispose()
        {
            CleanUp();

            if (subject is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        private void Setup()
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

        protected virtual void CleanUp()
        {
        }
    }
}
