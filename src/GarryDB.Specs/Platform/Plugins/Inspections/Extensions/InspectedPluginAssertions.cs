﻿using System.Linq;
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
        private static readonly PropertyInfo assemblyProperty =
            typeof(InspectedAssembly).GetProperty("Assembly",
                                                  BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty);

        public InspectedPluginAssertions(InspectedPlugin subject)
        {
            Subject = subject;
        }

        protected override string Identifier
        {
            get { return "inspected-plugin"; }
        }

        public AndConstraint<InspectedPluginAssertions> HavePluginAssembly(Assembly assembly,
                                                                           string because = "",
                                                                           params object[] becauseArgs
        )
        {
            return HavePluginAssembly(assembly.GetName(), because, becauseArgs);
        }

        public AndConstraint<InspectedPluginAssertions> HavePluginAssembly(AssemblyName assemblyName,
                                                                           string because = "",
                                                                           params object[] becauseArgs
        )
        {
            var assembly = (Assembly)assemblyProperty.GetValue(Subject.PluginAssembly);

            Execute.Assertion.BecauseOf(because, becauseArgs)
                   .ForCondition(assembly != null && assembly.GetName().IsCompatibleWith(assemblyName))
                   .FailWith("Expected {context:inspected-plugin} to have {0} as the plugin assembly{reason}", assemblyName);

            return new AndConstraint<InspectedPluginAssertions>(this);
        }

        public AndConstraint<InspectedPluginAssertions> Provide(Assembly assembly,
                                                                string because = "",
                                                                params object[] becauseArgs
        )
        {
            return Provide(assembly.GetName(), because, becauseArgs);
        }

        public AndConstraint<InspectedPluginAssertions> Provide(AssemblyName assemblyName,
                                                                string because = "",
                                                                params object[] becauseArgs
        )
        {
            Execute.Assertion.BecauseOf(because, becauseArgs)
                   .Given(() => Subject.ProvidedAssemblies.Select(providedAssembly =>
                                                                      (Assembly)assemblyProperty.GetValue(providedAssembly)))
                   .ForCondition(assemblies => assemblies.Any(assembly => assembly.GetName().IsCompatibleWith(assemblyName)))
                   .FailWith("Expected {context:inspected-plugin} to provide {0}{reason}", assemblyName);

            return new AndConstraint<InspectedPluginAssertions>(this);
        }

        public AndConstraint<InspectedPluginAssertions> Reference(Assembly assembly,
                                                                  string because = "",
                                                                  params object[] becauseArgs
        )
        {
            return Reference(assembly.GetName(), because, becauseArgs);
        }

        public AndConstraint<InspectedPluginAssertions> Reference(AssemblyName assemblyName,
                                                                  string because = "",
                                                                  params object[] becauseArgs
        )
        {
            Execute.Assertion.BecauseOf(because, becauseArgs)
                   .Given(() => Subject.ReferencedAssemblies.Select(referencedAssembly =>
                                                                        (Assembly)assemblyProperty.GetValue(referencedAssembly)))
                   .ForCondition(assemblies => assemblies.Any(assembly => assembly.GetName().IsCompatibleWith(assemblyName)))
                   .FailWith("Expected {context:inspected-plugin} to reference {0}{reason}", assemblyName);

            return new AndConstraint<InspectedPluginAssertions>(this);
        }
    }
}
