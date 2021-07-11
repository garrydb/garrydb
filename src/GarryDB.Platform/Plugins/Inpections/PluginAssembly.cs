using System.Reflection;

using GarryDb.Plugins;

namespace GarryDb.Platform.Plugins.Inpections
{
    /// <summary>
    ///     The assembly containing the <see cref="Plugin" />.
    /// </summary>
    public sealed class PluginAssembly : InspectedAssembly
    {
        /// <summary>
        ///     Initializes a new <see cref="PluginAssembly" />.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly" /> containing the plugin.</param>
        public PluginAssembly(Assembly assembly)
            : base(assembly)
        {
        }

        /// <summary>
        ///     Gets the identity of the plugin.
        /// </summary>
        public PluginIdentity PluginIdentity
        {
            get
            {
                AssemblyName assemblyName = Assembly.GetName();
                return new PluginIdentity(assemblyName.Name ?? "unknown", assemblyName.Version?.ToString() ?? "1.0.0.0");
            }
        }
    }
}
