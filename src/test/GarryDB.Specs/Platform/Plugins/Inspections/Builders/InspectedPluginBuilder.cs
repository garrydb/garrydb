using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using ExamplePlugin;
using ExamplePlugin.Contract;
using ExamplePlugin.Shared;

using GarryDB.Platform.Plugins.Inpections;
using GarryDB.Plugins;

namespace GarryDB.Specs.Platform.Plugins.Inspections.Builders
{
    public sealed class InspectedPluginBuilder : TestDataBuilder<InspectedPlugin>
    {
        private readonly IList<Assembly> provided = new List<Assembly>();
        private readonly IList<Assembly> references = new List<Assembly>();
        private Assembly pluginAssembly;

        protected override void OnPreBuild()
        {
            if (pluginAssembly == null)
            {
                For<Example>().Provides<ExampleShared>().Provides<ExampleContract>();
            }
        }

        protected override InspectedPlugin OnBuild()
        {
            return new(new PluginAssembly(pluginAssembly), provided.Select(assembly => new ProvidedAssembly(assembly)),
                       references.Select(assembly => new ReferencedAssembly(assembly)));
        }

        public InspectedPluginBuilder For<TPlugin>() where TPlugin : Plugin
        {
            return For(typeof(TPlugin).Assembly);
        }

        public InspectedPluginBuilder For(Assembly assembly)
        {
            pluginAssembly = assembly;

            return this;
        }

        public InspectedPluginBuilder Provides<T>()
        {
            return Provides(typeof(T).Assembly);
        }

        public InspectedPluginBuilder Provides(Assembly assembly)
        {
            provided.Add(assembly);

            return this;
        }

        public InspectedPluginBuilder References<T>()
        {
            return References(typeof(T).Assembly);
        }

        public InspectedPluginBuilder References(Assembly assembly)
        {
            references.Add(assembly);

            return this;
        }
    }
}