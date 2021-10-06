using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

using GarryDb.Platform.Extensions;

namespace GarryDb.Platform.Plugins
{
    /// <summary>
    ///     Provides an <see cref="Assembly" /> to a different <see cref="AssemblyLoadContext" />.
    /// </summary>
    internal sealed class AssemblyProvider
    {
        private readonly AssemblyLoadContext assemblyLoadContext;

        /// <summary>
        ///     Initializes a new <see cref="AssemblyProvider" />.
        /// </summary>
        /// <param name="assemblyLoadContext">The assembly load context for loading assemblies.</param>
        public AssemblyProvider(AssemblyLoadContext assemblyLoadContext)
        {
            this.assemblyLoadContext = assemblyLoadContext;
        }

        /// <summary>
        ///     Provide the <see cref="Assembly" />.
        /// </summary>
        /// <param name="assemblyName">The assembly name.</param>
        /// <returns>The <see cref="Assembly" />, or <c>null</c> if this provider can't provide it.</returns>
        public Assembly? Provide(AssemblyName assemblyName)
        {
            Assembly? assembly = assemblyLoadContext.Assemblies.SingleOrDefault(x => assemblyName.IsCompatibleWith(x.GetName()));

            return assembly;
        }
    }
}
