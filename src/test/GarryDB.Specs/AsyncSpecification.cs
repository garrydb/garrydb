using System.Threading.Tasks;

namespace GarryDB.Specs
{
    internal abstract class AsyncSpecification : Specification
    {
        protected override void Given()
        {
            GivenAsync().GetAwaiter().GetResult();
        }

        protected virtual Task GivenAsync()
        {
            return Task.CompletedTask;
        }

        protected override void When()
        {
            WhenAsync().GetAwaiter().GetResult();
        }

        protected virtual Task WhenAsync()
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
