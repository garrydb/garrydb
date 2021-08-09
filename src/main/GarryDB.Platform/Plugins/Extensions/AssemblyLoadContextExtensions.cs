using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

using GarryDB.Platform.Extensions;

namespace GarryDB.Platform.Plugins.Extensions
{
    /// <summary>
    ///     Extends <see cref="AssemblyLoadContext" />.
    /// </summary>
    internal static class AssemblyLoadContextExtensions
    {
        /// <summary>
        ///     Tries to load <paramref name="name" /> from the specified <paramref name="assemblyLoadContext" />.
        /// </summary>
        /// <param name="assemblyLoadContext">The assembly load context.</param>
        /// <param name="name">The <see cref="AssemblyName" /> of the assembly to load.</param>
        /// <param name="assembly">
        ///     The loaded <see cref="Assembly" />, or <c>null</c> if the <paramref name="assemblyLoadContext" /> can't load it.
        /// </param>
        /// <returns><c>true</c> if the loading has been successful, otherwise <c>false</c>.</returns>
        public static bool TryLoad(this AssemblyLoadContext assemblyLoadContext, AssemblyName name, out Assembly? assembly)
        {
            assembly = assemblyLoadContext.Assemblies.SingleOrDefault(x => name.IsCompatibleWith(x.GetName()));

            if (assembly != null)
            {
                return true;
            }

            try
            {
                assembly = assemblyLoadContext.LoadFromAssemblyName(name);

                return true;
            }
            catch (FileNotFoundException)
            {
                assembly = null;

                return false;
            }
        }
    }
}