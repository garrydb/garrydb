using Akka.Actor;

using GarryDB.Platform.Messaging.Messages;

namespace GarryDB.Platform.Messaging
{
    /// <summary>
    ///     The actor responsible for all plugins.
    /// </summary>
    public sealed class PluginsActor : ReceiveActor
    {
        private PluginsActor()
        {
            Receive((PluginLoaded message) =>
            {
                Context.ActorOf(PluginActor.Props(message.PluginIdentity, message.Plugin), message.PluginIdentity.Name);
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
