using System;

using GarryDB.Platform.Extensions;
using GarryDB.Plugins;

namespace GarryDB.Platform.Plugins.Extensions
{
    internal static class PluginExtensions
    {
        public static Type? FindConfigurationType(this Plugin plugin)
        {
            return plugin.GetType().FindConfigurationType();
        }
    }
}
