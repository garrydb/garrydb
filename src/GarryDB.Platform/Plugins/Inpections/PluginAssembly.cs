using System.Linq;
using System.Reflection;

using GarryDB.Platform.Extensions;
using GarryDB.Plugins;

namespace GarryDB.Platform.Plugins.Inpections
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
            PluginType = assembly.DefinedTypes.Single(type => type.IsPluginType()).FullName!;
            StartupOrder = assembly.GetCustomAttributesData()
                                   .Where(x => x.AttributeType.Name == nameof(StartupOrderAttribute))
                                   .Select(x => (int?)x.ConstructorArguments.Single().Value ?? 0)
                                   .SingleOrDefault();
        }

        /// <summary>
        ///     Gets the startup order.
        /// </summary>
        public int StartupOrder { get; }

        /// <summary>
        ///     Gets the full name of the plugin type.
        /// </summary>
        public string PluginType { get; }

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
