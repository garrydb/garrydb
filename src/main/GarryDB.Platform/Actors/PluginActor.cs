using Akka.Actor;

using GarryDB.Platform.Messaging;
using GarryDB.Plugins;

namespace GarryDB.Platform.Actors
{
    /// <summary>
    ///     The actor responsible for a plugins.
    /// </summary>
    internal sealed class PluginActor : ReceiveActor
    {
        /// <summary>
        ///     Initializes a new <see cref="PluginActor" />.
        /// </summary>
        /// <param name="plugin">The plugin.</param>
        // ReSharper disable once MemberCanBePrivate.Global
        public PluginActor(Plugin plugin)
        {
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
        /// <param name="plugin">The plugin.</param>
        /// <returns>The configuration object for creating <see cref="PluginActor" />.</returns>
        public static Props Props(Plugin plugin)
        {
            return Akka.Actor.Props.Create(() => new PluginActor(plugin));
        }
    }
}
