﻿using System.Reflection;

using ExamplePlugin;

using GarryDb.Platform.Plugins.Inpections;
using GarryDb.Specs;

namespace GarryDB.Specs.Platform.Plugins.Inspections.Builders
{
    public sealed class PluginAssemblyBuilder : TestDataBuilder<PluginAssembly>
    {
        private Assembly assembly;

        protected override void OnPreBuild()
        {
            if (assembly == null)
            {
                ForAssembly(typeof(Example).Assembly);
            }
        }

        protected override PluginAssembly OnBuild()
        {
            return new PluginAssembly(assembly);
        }

        public PluginAssemblyBuilder ForAssembly(Assembly assembly)
        {
            this.assembly = assembly;
            return this;
        }
    }
}
