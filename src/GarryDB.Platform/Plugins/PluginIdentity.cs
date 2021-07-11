using GarryDb.Plugins;

namespace GarryDb.Platform.Plugins
{
    /// <summary>
    ///     The identity of a <see cref="Plugin" />.
    /// </summary>
    public record PluginIdentity
    {
        private string version;

        /// <summary>
        ///     Initializes a new <see cref="PluginIdentity" />.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        /// <param name="version">The version of the plugin.</param>
        public PluginIdentity(string name, string version)
        {
            Name = name;
            this.version = version;
        }

        /// <summary>
        ///     Gets the name of the plugin.
        /// </summary>
        public string Name { get; }
    }
}
