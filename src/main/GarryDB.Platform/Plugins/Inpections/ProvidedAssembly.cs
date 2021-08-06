using System.Reflection;

using GarryDB.Plugins;

namespace GarryDB.Platform.Plugins.Inpections
{
    /// <summary>
    ///     The assembly that the <see cref="Plugin" /> is providing to the rest of the system.
    /// </summary>
    public sealed class ProvidedAssembly : InspectedAssembly
    {
        /// <summary>
        ///     Initializes a new <see cref="ProvidedAssembly" />.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly" /> the plugin provides.</param>
        public ProvidedAssembly(Assembly assembly)
            : base(assembly)
        {
        }
    }
}
