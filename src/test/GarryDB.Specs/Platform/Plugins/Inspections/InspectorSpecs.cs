using System.IO;
using System.Reflection;

using ExamplePlugin;
using ExamplePlugin.Contract;
using ExamplePlugin.Shared;

using GarryDB.Platform.Plugins.Inpections;
using GarryDB.Plugins;
using GarryDB.Specs.Platform.Infrastructure.Builders;
using GarryDB.Specs.Platform.Plugins.Inspections.Builders;
using GarryDB.Specs.Platform.Plugins.Inspections.Extensions;

using NUnit.Framework;

namespace GarryDB.Specs.Platform.Plugins.Inspections
{
    public static class InspectorSpecs
    {
        [TestFixture]
        public class When_inspecting_the_example_plugin : AsyncSpecification<Inspector>
        {
            private string directory;
            private InspectedPlugin inspectResults;
            private Assembly exampleAssembly;
            private Assembly contractAssembly;
            private Assembly sharedAssembly;
            private Assembly sharedPluginsAssembly;

            protected override Inspector Given()
            {
                exampleAssembly = typeof(Example).Assembly;
                contractAssembly = typeof(ExampleContract).Assembly;
                sharedAssembly = typeof(ExampleShared).Assembly;
                sharedPluginsAssembly = typeof(Plugin).Assembly;

                directory = Path.GetDirectoryName(exampleAssembly.Location);

                return new InspectorBuilder()
                       .Using(new FileSystemBuilder()
                              .WithFiles(directory,
                                         Path.GetFileName(exampleAssembly.Location),
                                         Path.GetFileName(contractAssembly.Location),
                                         Path.GetFileName(sharedAssembly.Location),
                                         Path.GetFileName(sharedPluginsAssembly.Location))
                              .Build())
                       .Build();
            }

            protected override void When(Inspector subject)
            {
                inspectResults = subject.Inspect(directory);
            }

            [Test]
            public void It_should_find_the_plugin_assembly()
            {
                inspectResults.Should().HavePluginAssembly(exampleAssembly);
            }

            [Test]
            public void It_should_find_the_contracts_assemblies()
            {
                inspectResults.Should().Provide(contractAssembly);
                inspectResults.Should().Provide(sharedAssembly);
            }

            [Test]
            public void It_should_find_the_reference_to_the_shared_plugins_assembly()
            {
                inspectResults.Should().Reference(sharedPluginsAssembly);
            }
        }
    }
}
