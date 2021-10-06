using System.Collections.Generic;

using GarryDb.Platform.Extensions;

namespace GarryDb.Platform.Plugins.PackageSources
{
    /// <summary>
    ///     A <see cref="PluginPackageSource" /> where the <see cref="PluginPackage" /> is known at compile time.
    /// </summary>
    public sealed class HardCodedPluginPackageSource : PluginPackageSource
    {
        /// <summary>
        ///     Initializes a new <see cref="HardCodedPluginPackageSource" />.
        /// </summary>
        /// <param name="pluginPackage">The plugin package.</param>
        public HardCodedPluginPackageSource(PluginPackage pluginPackage)
        {
            PluginPackages = pluginPackage.AsEnumerable();
        }

        /// <inheritdoc />
        public IEnumerable<PluginPackage> PluginPackages { get; }
    }
}
