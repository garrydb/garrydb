using System.Reflection;

using DependsOnExamplePlugin;

using GarryDb.Platform.Plugins.Inpections;
using GarryDb.Specs;

namespace GarryDB.Specs.Platform.Plugins.Inspections.Builders
{
    public sealed class ReferencedAssemblyBuilder : TestDataBuilder<ReferencedAssembly>
    {
        private Assembly assembly;

        protected override void OnPreBuild()
        {
            if (assembly == null)
            {
                ForAssembly(typeof(DependsOnExample).Assembly);
            }
        }

        protected override ReferencedAssembly OnBuild()
        {
            return new ReferencedAssembly(assembly);
        }

        public ReferencedAssemblyBuilder ForAssembly(Assembly assembly)
        {
            this.assembly = assembly;
            return this;
        }
    }
}
