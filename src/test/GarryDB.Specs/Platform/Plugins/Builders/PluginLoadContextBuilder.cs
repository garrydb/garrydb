using System.Collections.Generic;
using System.Runtime.Loader;

using GarryDB.Platform.Plugins;

namespace GarryDB.Specs.Platform.Plugins.Builders
{
    internal sealed class PluginLoadContextBuilder : TestDataBuilder<PluginLoadContext>
    {
        private PluginDirectory pluginDirectory;
        private readonly IList<AssemblyLoadContext> providers = new List<AssemblyLoadContext>();

        protected override void OnPreBuild()
        {
            if (pluginDirectory == null)
            {
                ForDirectory(new PluginDirectoryBuilder().Build());
            }

            if (providers.Count == 0)
            {
                WithProvider(AssemblyLoadContext.Default);
            }
        }

        protected override PluginLoadContext OnBuild()
        {
            return new PluginLoadContext(pluginDirectory, providers);
        }

        public PluginLoadContextBuilder ForDirectory(PluginDirectory pluginDirectory)
        {
            this.pluginDirectory = pluginDirectory;
            return this;
        }

        public PluginLoadContextBuilder WithProvider(AssemblyLoadContext provider)
        {
            providers.Add(provider);
            return this;
        }

        public PluginLoadContextBuilder WithProviders(params AssemblyLoadContext[] providers)
        {
            foreach (AssemblyLoadContext provider in providers)
            {
                WithProvider(provider);
            }

            return this;
        }
    }
}
