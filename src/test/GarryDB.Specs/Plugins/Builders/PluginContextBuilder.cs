using System.Threading.Tasks;

using GarryDB.Plugins;

namespace GarryDB.Specs.Plugins.Builders
{
    internal sealed class PluginContextBuilder : TestDataBuilder<PluginContext>
    {
        private string name;

        protected override void OnPreBuild()
        {
            if (name == null)
            {
                Named(name);
            }
        }

        protected override PluginContext OnBuild()
        {
            return new PluginContextStub(this);
        }

        public PluginContextBuilder Named(string name)
        {
            this.name = name;
            return this;
        }

        private sealed class PluginContextStub : PluginContext
        {
            private readonly PluginContextBuilder builder;

            public PluginContextStub(PluginContextBuilder builder)
            {
                this.builder = builder;
            }

            public string Name { get; }

            public Task SendAsync(string destination, string handler, object message)
            {
                return Task.CompletedTask;
            }
        }
    }
}
