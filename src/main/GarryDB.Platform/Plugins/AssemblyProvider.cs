using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Runtime.Loader;

using GarryDb.Platform.Extensions;
using GarryDb.Platform.Plugins.Extensions;

namespace GarryDb.Platform.Plugins
{
    /// <summary>
    ///     Provides an <see cref="Assembly" /> to a different <see cref="AssemblyLoadContext" />.
    /// </summary>
    public sealed class AssemblyProvider : IDisposable
    {
        private readonly MemoryCache cache;
        private readonly AssemblyLoadContext assemblyLoadContext;

        /// <summary>
        ///     Initializes a new <see cref="AssemblyProvider" />.
        /// </summary>
        /// <param name="assemblyLoadContext">The assembly load context for loading assemblies.</param>
        public AssemblyProvider(AssemblyLoadContext assemblyLoadContext)
        {
            this.assemblyLoadContext = assemblyLoadContext;
            cache = new MemoryCache(Guid.NewGuid().ToString());
        }

        /// <summary>
        ///     Provide the <see cref="Assembly" />.
        /// </summary>
        /// <param name="assemblyName">The assembly name.</param>
        /// <returns>The <see cref="Assembly" />, or <c>null</c> if this provider can't provide it.</returns>
        public Assembly? Provide(AssemblyName assemblyName)
        {
            string cacheKey = $"{assemblyLoadContext.Name}+{assemblyName.Name}.{assemblyName.Version?.Major ?? 0}";

            return cache.GetOrAdd(cacheKey, () =>
            {
                Assembly? assembly = assemblyLoadContext.Assemblies.SingleOrDefault(x => assemblyName.IsCompatibleWith(x.GetName()));

                return assembly;
            });
        }

        /// <inheritdoc />
        public void Dispose()
        {
            cache.Dispose();
        }
    }
}
