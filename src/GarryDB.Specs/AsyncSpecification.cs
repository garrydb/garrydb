using System.Threading.Tasks;

namespace GarryDB.Specs
{
    public abstract class AsyncSpecification<TSubject> : Specification<TSubject>
    {
        protected override TSubject Given()
        {
            return GivenAsync().GetAwaiter().GetResult();
        }

        protected virtual Task<TSubject> GivenAsync()
        {
            return Task.FromResult<TSubject>(default);
        }

        protected override void When(TSubject subject)
        {
            WhenAsync(subject).GetAwaiter().GetResult();
        }

        protected virtual Task WhenAsync(TSubject subject)
        {
            return Task.CompletedTask;
        }

        protected override void CleanUp()
        {
            CleanUpAsync().GetAwaiter().GetResult();
        }

        protected virtual Task CleanUpAsync()
        {
            return Task.CompletedTask;
        }
    }
}
