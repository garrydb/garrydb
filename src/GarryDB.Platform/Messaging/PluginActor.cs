using Akka.Actor;

using GarryDB.Platform.Plugins;
using GarryDB.Plugins;

namespace GarryDB.Platform.Messaging
{
    /// <summary>
    ///     The actor responsible for a plugins.
    /// </summary>
    public sealed class PluginActor : ReceiveActor
    {
        private readonly Plugin plugin;
        private readonly PluginIdentity pluginIdentity;

        /// <summary>
        ///     Initializes a new <see cref="PluginActor" />.
        /// </summary>
        /// <param name="pluginIdentity">The identity of the plugin.</param>
        /// <param name="plugin">The plugin.</param>
        public PluginActor(PluginIdentity pluginIdentity, Plugin plugin)
        {
            this.pluginIdentity = pluginIdentity;
            this.plugin = plugin;

            ReceiveAsync(async (MessageEnvelope envelope) =>
                         {
                             object? returnMessage = await plugin.RouteAsync(envelope.Destination.Handler, envelope.Message)
                                                                 .ConfigureAwait(false);

                             if (returnMessage != null)
                             {
                                 Sender.Tell(envelope.CreateReturnMessage(returnMessage));
                             }
                         });
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
