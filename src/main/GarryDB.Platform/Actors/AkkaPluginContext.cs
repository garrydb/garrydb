using System.Threading.Tasks;

using Akka.Actor;

using GarryDB.Platform.Messaging;
using GarryDB.Platform.Plugins;
using GarryDB.Plugins;

using Address = GarryDB.Platform.Messaging.Address;

namespace GarryDB.Platform.Actors
{
    /// <summary>
    ///     The plugin context that uses Akka.
    /// </summary>
    internal sealed class AkkaPluginContext : PluginContext
    {
        private readonly PluginIdentity pluginIdentity;
        private readonly IActorRef pluginsActor;

        /// <summary>
        ///     Initializes a new <see cref="AkkaPluginContext" />.
        /// </summary>
        /// <param name="pluginsActor">The reference to the plugins actor.</param>
        /// <param name="pluginIdentity">The identity of the plugin.</param>
        public AkkaPluginContext(IActorRef pluginsActor, PluginIdentity pluginIdentity)
        {
            this.pluginsActor = pluginsActor;
            this.pluginIdentity = pluginIdentity;
        }
        
        /// <inheritdoc />
        public string Name
        {
            get { return pluginIdentity.Name; }
        }

        /// <inheritdoc />
        public Task SendAsync(string destination, string handler, object message)
        {
            var envelope = new MessageEnvelope(pluginIdentity, new Address(PluginIdentity.Parse(destination), handler), message);

            pluginsActor.Tell(envelope);

            return Task.CompletedTask;
        }
    }
}
