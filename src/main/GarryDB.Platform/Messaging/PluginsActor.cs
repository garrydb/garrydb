using System.Collections.Concurrent;

using Akka.Actor;

using GarryDB.Platform.Messaging.Messages;
using GarryDB.Platform.Plugins;

namespace GarryDB.Platform.Messaging
{
    /// <summary>
    ///     The actor responsible for all plugins.
    /// </summary>
    internal sealed class PluginsActor : ReceiveActor
    {
        /// <summary>
        ///     Initializes a new <see cref="PluginsActor" />.
        /// </summary>
        public PluginsActor()
        {
            var plugins = new ConcurrentDictionary<PluginIdentity, IActorRef>();

            Receive((PluginLoaded message) =>
                    {
                        IActorRef pluginActorRef = Context.ActorOf(PluginActor.Props(message.Plugin), message.PluginIdentity.Name);
                        plugins[message.PluginIdentity] = pluginActorRef;
                    });

            Receive((MessageEnvelope envelope) =>
                    {
                        IActorRef pluginActorRef = plugins[envelope.Destination.PluginIdentity];
                        pluginActorRef.Forward(envelope);
                    });
        }

        /// <summary>
        ///     Create the <see cref="PluginsActor" />.
        /// </summary>
        /// <returns>The configuration object for creating <see cref="PluginsActor" />.</returns>
        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new PluginsActor());
        }
    }
}
