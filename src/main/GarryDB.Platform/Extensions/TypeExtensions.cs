using System;
using System.Linq;

using GarryDB.Plugins;

namespace GarryDB.Platform.Extensions
{
    /// <summary>
    ///     Extends <see cref="Type" />.
    /// </summary>
    internal static class TypeExtensions
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

        /// <summary>
        ///     Find the type of the configuration class for <see cref="ConfigurablePlugin{TConfiguration}" />.
        /// </summary>
        /// <param name="pluginType">The plugin type.</param>
        /// <returns>
        ///     The type of the configuration class if the plugin is a <see cref="ConfigurablePlugin{TConfiguration}" />.
        /// </returns>
        public static Type? FindConfigurationType(this Type? pluginType)
        {
            if (pluginType == null)
            {
                return null;
            }

            if (pluginType.IsGenericType && pluginType.GetGenericTypeDefinition() == typeof(ConfigurablePlugin<>))
            {
                return pluginType.GetGenericArguments().Single();
            }

            return pluginType.BaseType.FindConfigurationType();
        }
    }
}
