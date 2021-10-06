using System.Reflection;

namespace GarryDb.Platform.Extensions
{
    /// <summary>
    ///     Extends <see cref="AssemblyName" />.
    /// </summary>
    internal static class AssemblyNameExtensions
    {
        /// <summary>
        ///     Determines whether <paramref name="compatibleWith" /> can be used instead of <paramref name="assemblyName" />.
        /// </summary>
        /// <param name="assemblyName">The <see cref="AssemblyName" /> to check.</param>
        /// <param name="compatibleWith">The <see cref="AssemblyName" /> of check compatibility with.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="assemblyName" /> is compatible with <paramref name="compatibleWith" />,
        ///     otherwise <c>false</c>.
        /// </returns>
        public static bool IsCompatibleWith(this AssemblyName assemblyName, AssemblyName compatibleWith)
        {
            if (assemblyName.Name != compatibleWith.Name)
            {
                return false;
            }

            if (assemblyName.Version == null || compatibleWith.Version == null)
            {
                return true;
            }

            return assemblyName.Version.Major == compatibleWith.Version.Major;
        }
    }
}
