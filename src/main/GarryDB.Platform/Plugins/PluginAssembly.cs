using System;
using System.Linq;
using System.Reflection;

using GarryDb.Platform.Extensions;
using GarryDb.Plugins;

namespace GarryDb.Platform.Plugins
{
    /// <summary>
    ///     The assembly containing the <see cref="Plugin" />.
    /// </summary>
    public sealed class PluginAssembly
    {
        private readonly Assembly assembly;

        /// <summary>
        ///     Initializes a new <see cref="PluginAssembly" />.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly" /> containing the plugin.</param>
        public PluginAssembly(Assembly assembly)
        {
            this.assembly = assembly;

            PluginType = assembly.DefinedTypes.Single(type => type.IsPluginType());
            StartupOrder = assembly.GetCustomAttribute<StartupOrderAttribute>()?.Order ?? 0;
        }

        /// <summary>
        ///     Gets the startup order.
        /// </summary>
        public int StartupOrder { get; }

        /// <summary>
        ///     Gets the full name of the plugin type.
        /// </summary>
        public Type PluginType { get; }

        /// <summary>
        ///     Gets the identity of the plugin.
        /// </summary>
        public PluginIdentity PluginIdentity
        {
            get
            {
                AssemblyName assemblyName = assembly.GetName();

                string name = PluginType.Name.RemoveSuffix("Plugin", true);

                return PluginIdentity.Parse($"{name}:{assemblyName.Version!.Major}.{assemblyName.Version.Minor}");
            }
        }
    }
}
