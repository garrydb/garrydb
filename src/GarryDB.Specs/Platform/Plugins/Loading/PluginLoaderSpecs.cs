using System.Runtime.Loader;

using ExamplePlugin;

using FluentAssertions;

using GarryDb.Platform.Plugins.Inpections;
using GarryDb.Platform.Plugins.Loading;
using GarryDb.Specs.Platform.Plugins.Inspections.Builders;

using NUnit.Framework;

namespace GarryDb.Specs.Platform.Plugins.Loading
{
    public static class PluginLoaderSpecs
    {
        [TestFixture]
        public class When_loading_the_example_plugin : Specification<PluginLoader>
        {
            private LoadedPlugin plugin;

            protected override PluginLoader Given()
            {
                InspectedPlugin inspectedPlugin = new InspectedPluginBuilder().Build();
                return new PluginLoader(inspectedPlugin, AssemblyLoadContext.Default);
            }

            protected override void When(PluginLoader subject)
            {
                plugin = subject.Load();
            }

            [Test]
            public void It_should_instantiate_the_plugin()
            {
                plugin.Should().NotBeNull();
                plugin.Plugin.GetType().FullName.Should().Be(typeof(Example).FullName);
            }
        }
    }
}
