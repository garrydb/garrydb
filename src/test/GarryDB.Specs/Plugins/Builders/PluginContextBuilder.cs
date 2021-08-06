using System.Threading.Tasks;

using GarryDB.Plugins;

namespace GarryDB.Specs.Plugins.Builders
{
    public sealed class PluginContextBuilder : TestDataBuilder<PluginContext>
    {
        protected override PluginContext OnBuild()
        {
            return new PluginContextStub(this);
        }

        private sealed class PluginContextStub : PluginContext
        {
            private readonly PluginContextBuilder builder;

            public PluginContextStub(PluginContextBuilder builder)
            {
                this.builder = builder;
            }

            public Task SendAsync(string destination, string handler, object message)
            {
                return Task.CompletedTask;
            }
        }
    }
}
