using System.Reflection;

using GarryDb.Plugins;

namespace GarryDb.Platform.Plugins.Inpections
{
    /// <summary>
    ///     The assembly that is referenced by the <see cref="Plugin" />.
    /// </summary>
    public sealed class ReferencedAssembly : InspectedAssembly
    {
        /// <summary>
        ///     Initializes a new <see cref="ReferencedAssembly" />.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly" /> the plugin references.</param>
        public ReferencedAssembly(Assembly assembly)
            : base(assembly)
        {
        }
    }
}
