using System.Reflection;

using ExamplePlugin.Contract;

using GarryDb.Platform.Plugins.Inpections;
using GarryDb.Specs;

namespace GarryDB.Specs.Platform.Plugins.Inspections.Builders
{
    public sealed class ProvidedAssemblyBuilder : TestDataBuilder<ProvidedAssembly>
    {
        private Assembly assembly;

        protected override void OnPreBuild()
        {
            if (assembly == null)
            {
                ForAssembly(typeof(ExampleContract).Assembly);
            }
        }

        protected override ProvidedAssembly OnBuild()
        {
            return new ProvidedAssembly(assembly);
        }

        public ProvidedAssemblyBuilder ForAssembly(Assembly assembly)
        {
            this.assembly = assembly;
            return this;
        }
    }
}
