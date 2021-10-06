using System.Collections.Concurrent;

using Akka.Actor;

using GarryDb.Platform.Messaging;
using GarryDb.Platform.Plugins;

namespace GarryDb.Platform.Actors
{
    /// <summary>
    ///     The actor responsible for all plugins.
    /// </summary>
    internal sealed class PluginsActor : ReceiveActor
    {
        /// <summary>
        ///     Initializes a new <see cref="PluginsActor" />.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
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
                PluginIdentity pluginIdentity = envelope.Destination.PluginIdentity;

                if (plugins.TryGetValue(pluginIdentity, out IActorRef? pluginActorRef))
                {
                    pluginActorRef.Forward(envelope);
                }
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
