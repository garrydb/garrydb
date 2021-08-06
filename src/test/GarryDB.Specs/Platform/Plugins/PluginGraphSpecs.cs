using System.Collections.Generic;
using System.Linq;

using Autofac;

using DependsOnExamplePlugin;

using ExamplePlugin.Shared;

using GarryDB.Platform.Plugins;
using GarryDB.Platform.Plugins.Inpections;
using GarryDB.Platform.Plugins.Loading;
using GarryDB.Specs.Platform.Plugins.Inspections.Builders;

using NUnit.Framework;

namespace GarryDB.Specs.Platform.Plugins
{
    public static class PluginGraphSpecs
    {
        [TestFixture]
        public class When_loading_plugins : Specification<PluginLoaderFactory>
        {
            private IList<PluginLoader> pluginLoaders;
            private IList<LoadedPlugin> plugins;

            protected override PluginLoaderFactory Given()
            {
                return new();
            }

            protected override void When(PluginLoaderFactory subject)
            {
                InspectedPlugin plugin1 = new InspectedPluginBuilder().Build();
                InspectedPlugin plugin2 =
                    new InspectedPluginBuilder().For<DependsOnExample>().References<ExampleShared>().Build();

                pluginLoaders = subject.Create(plugin1, plugin2).ToList();
                plugins = pluginLoaders.Select(x => x.Load(new ContainerBuilder())).ToList();
            }

            [Test]
            public void It_should()
            {
            }
        }
    }
}