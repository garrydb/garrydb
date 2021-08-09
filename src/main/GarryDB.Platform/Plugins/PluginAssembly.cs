using System;
using System.Linq;
using System.Reflection;

using GarryDB.Platform.Extensions;
using GarryDB.Plugins;

namespace GarryDB.Platform.Plugins
{
    /// <summary>
    ///     The assembly containing the <see cref="Plugin" />.
    /// </summary>
    internal sealed class PluginAssembly
    {
        private readonly Assembly assembly;

        /// <summary>
        ///     Initializes a new <see cref="PluginAssembly" />.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly" /> containing the plugin.</param>
        public PluginAssembly(Assembly assembly)
        {
            this.assembly = assembly;
            TypeInfo? singleOrDefault = assembly.DefinedTypes.SingleOrDefault(type => type.IsPluginType());

            PluginType = singleOrDefault!;
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

                return PluginIdentity.Parse($"{assemblyName.Name}:{assemblyName.Version}");
            }
        }
    }
}
