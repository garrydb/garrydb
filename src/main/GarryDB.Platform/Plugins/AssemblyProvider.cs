using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Runtime.Loader;

using GarryDB.Platform.Extensions;
using GarryDB.Platform.Plugins.Extensions;
#pragma warning disable 1591

namespace GarryDB.Platform.Plugins
{
    public sealed class AssemblyProvider : IDisposable
    {
        private readonly MemoryCache cache;
        private readonly AssemblyLoadContext inner;

        public AssemblyProvider(AssemblyLoadContext inner)
        {
            this.inner = inner;
            cache = new MemoryCache(Guid.NewGuid().ToString());
        }

        public Assembly? Provide(AssemblyName assemblyName)
        {
            string cacheKey = $"{inner.Name}+{assemblyName.Name}.{assemblyName.Version?.Major ?? 0}";

            return cache.GetOrAdd(cacheKey, () =>
            {
                Assembly? assembly = inner.Assemblies.SingleOrDefault(x => assemblyName.IsCompatibleWith(x.GetName()));

                if (assembly != null)
                {
                    return assembly;
                }

                try
                {
                    assembly = inner.LoadFromAssemblyName(assemblyName);

                    return assembly;
                }
                catch (FileNotFoundException)
                {
                    Debug.WriteLine($"*** NOT FOUND *** - {inner.Name} / {assemblyName}");

                    return null;
                }
            });
        }

        public void Dispose()
        {
            cache.Dispose();
        }
    }
}
