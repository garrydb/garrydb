using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace GarryDB.Platform.Plugins.Loading.Extensions
{
    /// <summary>
    ///     Extends <see cref="AssemblyLoadContext" />.
    /// </summary>
    public static class AssemblyLoadContextExtensions
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
