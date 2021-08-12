using System.Reflection;

using GarryDB.Platform.Plugins;

namespace GarryDB.Specs.Platform.Plugins.Builders
{
    internal sealed class PluginAssemblyBuilder : TestDataBuilder<PluginAssembly>
    {
        private Assembly assembly;

        protected override void OnPreBuild()
        {
            if (assembly == null)
            {
                For(Assembly.GetExecutingAssembly());
            }
        }

        protected override PluginAssembly OnBuild()
        {
            return new PluginAssembly(assembly);
        }

        public PluginAssemblyBuilder For(Assembly assembly)
        {
            this.assembly = assembly;
            return this;
        }
    }
}
