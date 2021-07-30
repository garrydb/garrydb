using System.Reflection;

using GarryDB.Platform.Extensions;

namespace GarryDB.Platform.Plugins.Inpections
{
    /// <summary>
    ///     The assembly that has been inspected by a <see cref="Inspector" />.
    /// </summary>
    public abstract class InspectedAssembly
    {
        /// <summary>
        ///     Initializes a new <see cref="InspectedAssembly" />.
        /// </summary>
        /// <param name="assembly">The assembly that has been inspected.</param>
        protected InspectedAssembly(Assembly assembly)
        {
            Assembly = assembly;
        }

        /// <summary>
        ///     Gets the <see cref="System.Reflection.AssemblyName" />.
        /// </summary>
        public AssemblyName AssemblyName
        {
            get { return Assembly.GetName(); }
        }

        /// <summary>
        ///     Gets the <see cref="System.Reflection.Assembly" />.
        /// </summary>
        protected Assembly Assembly { get; }

        /// <summary>
        ///     Gets the path to the assembly.
        /// </summary>
        public string Path
        {
            get { return Assembly.Location; }
        }

        /// <summary>
        ///     Determines whether this assembly can be used instead of <paramref name="assemblyName" />.
        /// </summary>
        /// <param name="assemblyName">The <see cref="AssemblyName" /> to check.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="assemblyName" /> is compatible with this assembly, otherwise <c>false</c>.
        /// </returns>
        public bool IsCompatibleWith(AssemblyName assemblyName)
        {
            return AssemblyName.IsCompatibleWith(assemblyName);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return System.IO.Path.GetFileName(Path);
        }
    }
}
