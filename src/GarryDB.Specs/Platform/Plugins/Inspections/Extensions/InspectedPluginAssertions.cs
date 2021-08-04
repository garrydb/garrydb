using System.Linq;
using System.Reflection;

using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

using GarryDB.Platform.Extensions;
using GarryDB.Platform.Plugins.Inpections;

namespace GarryDB.Specs.Platform.Plugins.Inspections.Extensions
{
    public class InspectedPluginAssertions : ReferenceTypeAssertions<InspectedPlugin, InspectedPluginAssertions>
    {
        private static readonly PropertyInfo AssemblyProperty =
            typeof(InspectedAssembly).GetProperty("Assembly",
                                                  BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty);

        public InspectedPluginAssertions(InspectedPlugin subject)
            : base(subject)
        {
        }

        protected override string Identifier
        {
            get { return "inspectedPlugin"; }
        }

        [CustomAssertion]
        public AndConstraint<InspectedPluginAssertions> HavePluginAssembly(Assembly assembly,
                                                                           string because = "",
                                                                           params object[] becauseArgs
        )
        {
            return HavePluginAssembly(assembly.GetName(), because, becauseArgs);
        }

        [CustomAssertion]
        public AndConstraint<InspectedPluginAssertions> HavePluginAssembly(AssemblyName assemblyName,
                                                                           string because = "",
                                                                           params object[] becauseArgs
        )
        {
            var assembly = (Assembly)AssemblyProperty.GetValue(Subject.PluginAssembly);

            Execute.Assertion.BecauseOf(because, becauseArgs)
                   .ForCondition(assembly != null && assembly.GetName().IsCompatibleWith(assemblyName))
                   .FailWith("Expected {context:inspectedPlugin} to have {0} as the plugin assembly{reason}", assemblyName);

            return new AndConstraint<InspectedPluginAssertions>(this);
        }

        [CustomAssertion]
        public AndConstraint<InspectedPluginAssertions> Provide(Assembly assembly,
                                                                string because = "",
                                                                params object[] becauseArgs
        )
        {
            return Provide(assembly.GetName(), because, becauseArgs);
        }

        [CustomAssertion]
        public AndConstraint<InspectedPluginAssertions> Provide(AssemblyName assemblyName,
                                                                string because = "",
                                                                params object[] becauseArgs
        )
        {
            Execute.Assertion.BecauseOf(because, becauseArgs)
                   .Given(() => Subject.ProvidedAssemblies.Select(providedAssembly =>
                                                                      (Assembly)AssemblyProperty.GetValue(providedAssembly)))
                   .ForCondition(assemblies => assemblies.Any(assembly => assembly.GetName().IsCompatibleWith(assemblyName)))
                   .FailWith("Expected {context:inspectedPlugin} to provide {0}{reason}", assemblyName);

            return new AndConstraint<InspectedPluginAssertions>(this);
        }

        [CustomAssertion]
        public AndConstraint<InspectedPluginAssertions> Reference(Assembly assembly,
                                                                  string because = "",
                                                                  params object[] becauseArgs
        )
        {
            return Reference(assembly.GetName(), because, becauseArgs);
        }

        [CustomAssertion]
        public AndConstraint<InspectedPluginAssertions> Reference(AssemblyName assemblyName,
                                                                  string because = "",
                                                                  params object[] becauseArgs
        )
        {
            Execute.Assertion.BecauseOf(because, becauseArgs)
                   .Given(() => Subject.ReferencedAssemblies.Select(referencedAssembly => (Assembly)AssemblyProperty.GetValue(referencedAssembly)))
                   .ForCondition(assemblies => assemblies.Any(assembly => assembly.GetName().IsCompatibleWith(assemblyName)))
                   .FailWith("Expected {context:inspectedPlugin} to reference {0}{reason}", assemblyName);

            return new AndConstraint<InspectedPluginAssertions>(this);
        }
    }
}
