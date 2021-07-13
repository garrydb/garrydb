using Akka.Actor;

using GarryDb.Platform.Plugins;
using GarryDb.Plugins;

namespace GarryDB.Platform.Messaging
{
    /// <summary>
    ///     The actor responsible for a plugins.
    /// </summary>
    public sealed class PluginActor : ReceiveActor
    {
        private readonly Plugin plugin;
        private readonly PluginIdentity pluginIdentity;

        private PluginActor(PluginIdentity pluginIdentity, Plugin plugin)
        {
            this.pluginIdentity = pluginIdentity;
            this.plugin = plugin;
        }

        /// <summary>
        ///     Create the <see cref="PluginActor" />.
        /// </summary>
        /// <param name="pluginIdentity">The identity of the plugin.</param>
        /// <param name="plugin">The plugin.</param>
        /// <returns>The configuration object for creating <see cref="PluginActor" />.</returns>
        public static Props Props(PluginIdentity pluginIdentity, Plugin plugin)
        {
            return Akka.Actor.Props.Create(() => new PluginActor(pluginIdentity, plugin));
        }
    }
}
