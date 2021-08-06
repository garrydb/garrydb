using System.Reflection;

using ExamplePlugin.Contract;

using GarryDB.Platform.Plugins.Inpections;

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
            return new(assembly);
        }

        public ProvidedAssemblyBuilder ForAssembly(Assembly assembly)
        {
            this.assembly = assembly;

            return this;
        }
    }
}
