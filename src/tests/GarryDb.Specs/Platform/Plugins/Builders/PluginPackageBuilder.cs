using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using GarryDb.Platform.Extensions;
using GarryDb.Platform.Plugins;
using GarryDb.Plugins;

using GarryDB.Specs;

namespace GarryDb.Specs.Platform.Plugins.Builders
{
    public sealed class PluginPackageBuilder : TestDataBuilder<PluginPackage>
    {
        private readonly IList<AssemblyName> additionalAssemblies = new List<AssemblyName>();
        private readonly Type pluginType;

        private PluginPackageBuilder(Type pluginType)
        {
            this.pluginType = pluginType;
        }

        protected override PluginPackage OnBuild()
        {
            return new PluginPackageStub(this);
        }

        public static PluginPackageBuilder ForPlugin<TPlugin>() where TPlugin : Plugin
        {
            return new PluginPackageBuilder(typeof(TPlugin));
        }

        public PluginPackageBuilder WithAdditionalAssembly<T>()
        {
            return WithAdditionalAssembly(typeof(T).Assembly);
        }

        public PluginPackageBuilder WithAdditionalAssembly(Assembly assembly)
        {
            return WithAdditionalAssembly(assembly.GetName());
        }

        public PluginPackageBuilder WithAdditionalAssembly(AssemblyName assemblyName)
        {
            additionalAssemblies.Add(assemblyName);
            return this;
        }

        private sealed class PluginPackageStub : PluginPackage
        {
            private readonly PluginPackageBuilder builder;

            public PluginPackageStub(PluginPackageBuilder builder)
                : base(builder.pluginType.Name)
            {
                this.builder = builder;
            }

            public override IEnumerable<AssemblyName> Assemblies
            {
                get { return builder.additionalAssemblies.Concat(builder.pluginType.Assembly.GetName()); }
            }

            public override Stream? ResolveAssembly(AssemblyName assemblyName)
            {
                return null;
            }

            public override Stream? ResolveAssemblySymbols(AssemblyName assemblyName)
            {
                return null;
            }

            public override string? ResolveUnmanagedDllPath(string unmanagedDllName)
            {
                return null;
            }
        }
    }
}
