using System;

using GarryDB.Plugins;

namespace GarryDB.Platform.Extensions
{
    /// <summary>
    ///     Extends <see cref="Type" />.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        ///     Determines whether <paramref name="type" /> is a plugin.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if it is a plugin, otherwise <c>false</c>.</returns>
        public static bool IsPluginType(this Type? type)
        {
            if (type == null || type.IsInterface || type.IsAbstract)
            {
                return false;
            }

            while (type != null && !type.IsInterface)
            {
                if (type.FullName == typeof(Plugin).FullName)
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }
    }
}
