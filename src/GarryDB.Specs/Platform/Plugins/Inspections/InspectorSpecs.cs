using System.IO;
using System.Reflection;

using ExamplePlugin;
using ExamplePlugin.Contract;
using ExamplePlugin.Shared;

using FluentAssertions;

using GarryDb.Platform.Plugins.Inpections;
using GarryDb.Specs.Platform.Builders;
using GarryDb.Specs.Platform.Plugins.Inspections.Builders;
using GarryDb.Specs.Platform.Plugins.Inspections.Extensions;

using NUnit.Framework;

namespace GarryDb.Specs.Platform.Plugins.Inspections
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

            protected override Inspector Given()
            {
                exampleAssembly = typeof(Example).Assembly;
                contractAssembly = typeof(ExampleContract).Assembly;
                sharedAssembly = typeof(ExampleShared).Assembly;
                directory = Path.GetDirectoryName(exampleAssembly.Location);

                return
                    new InspectorBuilder()
                        .Using(new FileSystemBuilder()
                               .WithFiles(directory,
                                   Path.GetFileName(exampleAssembly.Location),
                                   Path.GetFileName(contractAssembly.Location),
                                   Path.GetFileName(sharedAssembly.Location)
                               )
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
            public void It_should_find_no_referenced_assemblies()
            {
                inspectResults.ReferencedAssemblies.Should().BeEmpty();
            }
        }
    }
}
