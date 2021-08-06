using System.Threading.Tasks;

namespace GarryDB.Specs
{
    internal abstract class AsyncSpecification<TSubject> : Specification<TSubject>
    {
        protected override TSubject Given()
        {
            return AsyncPump.Run(() => GivenAsync());
        }

        protected virtual Task<TSubject> GivenAsync()
        {
            return Task.FromResult(default(TSubject));
        }

        protected override void When(TSubject subject)
        {
            AsyncPump.Run(() => WhenAsync(subject));
        }

        protected virtual Task WhenAsync(TSubject subject)
        {
            return Task.CompletedTask;
        }

        protected override void CleanUp()
        {
            AsyncPump.Run(() => CleanUpAsync());
        }

        protected virtual Task CleanUpAsync()
        {
            return Task.CompletedTask;
        }
    }
}
