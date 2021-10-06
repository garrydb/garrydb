using System.Collections.Generic;

namespace GarryDb.Platform.Plugins
{
    /// <summary>
    ///     A source of <see cref="PluginPackage" />s.
    /// </summary>
    public interface PluginPackageSource
    {
        /// <summary>
        ///     Gets the <see cref="PluginPackage" />s that are found in this source.
        /// </summary>
        IEnumerable<PluginPackage> PluginPackages { get; }
    }
}
