using System.Threading.Tasks;

using Akka.Actor;

using GarryDB.Platform.Messaging;
using GarryDB.Plugins;

using Address = GarryDB.Platform.Messaging.Address;

namespace GarryDB.Platform.Plugins
{
    /// <summary>
    ///     The plugin context that uses Akka.
    /// </summary>
    internal sealed class AkkaPluginContext : PluginContext
    {
        private readonly PluginIdentity pluginIdentity;
        private readonly IActorRef plugins;

        /// <summary>
        ///     Initializes a new <see cref="AkkaPluginContext" />.
        /// </summary>
        /// <param name="plugins">The reference to the plugins actor.</param>
        /// <param name="pluginIdentity">The identity of the plugin.</param>
        public AkkaPluginContext(IActorRef plugins, PluginIdentity pluginIdentity)
        {
            this.plugins = plugins;
            this.pluginIdentity = pluginIdentity;
        }

        /// <inheritdoc />
        public Task SendAsync(string destination, string handler, object message)
        {
            var envelope = new MessageEnvelope(pluginIdentity, new Address(new PluginIdentity(destination), handler), message);

            plugins.Tell(envelope);

            return Task.CompletedTask;
        }
    }
}
