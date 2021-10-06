using System;
using System.Threading.Tasks;

using Akka.Actor;

using GarryDb.Platform.Messaging;
using GarryDb.Platform.Plugins;
using GarryDb.Platform.Plugins.Configuration;
using GarryDb.Plugins;

using Address = GarryDb.Platform.Messaging.Address;

namespace GarryDb.Platform.Akka
{
    /// <summary>
    ///     Connects the plugin lifecycle with Akka.NET.
    /// </summary>
    internal sealed class AkkaPluginLifecycle : PluginLifecycle
    {
        private readonly IActorRef pluginsActor;
        private readonly ConfigurationStorage configurationStorage;
        private readonly PluginRegistry pluginRegistry;

        /// <summary>
        ///     Initializes a new <see cref="AkkaPluginLifecycle" />.
        /// </summary>
        /// <param name="configurationStorage">The configuration storage.</param>
        /// <param name="pluginRegistry">The plugin registry.</param>
        /// <param name="pluginsActor">The <see cref="IActorRef" /> for the plugins.</param>
        public AkkaPluginLifecycle(ConfigurationStorage configurationStorage, PluginRegistry pluginRegistry, IActorRef pluginsActor)
        {
            this.configurationStorage = configurationStorage;
            this.pluginRegistry = pluginRegistry;
            this.pluginsActor = pluginsActor;

            this.pluginRegistry.WhenPluginLoaded.Subscribe(identity =>
            {
                pluginsActor.Tell(new PluginLoaded(identity, this.pluginRegistry[identity]));
            });
        }

        /// <inheritdoc />
        public Task ConfigureAsync(PluginIdentity pluginIdentity)
        {
            Plugin plugin = pluginRegistry[pluginIdentity];
            object? configuration = configurationStorage.FindConfiguration(pluginIdentity, plugin);

            if (configuration == null)
            {
                return Task.CompletedTask;
            }

            var destination = new Address(pluginIdentity, "configure");
            var messageEnvelope = new MessageEnvelope(GarryPlugin.PluginIdentity, destination, configuration);

            return pluginsActor.Ask(messageEnvelope);
        }

        /// <inheritdoc />
        public Task StartAsync(PluginIdentity pluginIdentity)
        {
            var destination = new Address(pluginIdentity, "start");
            var messageEnvelope = new MessageEnvelope(GarryPlugin.PluginIdentity, destination);

            return pluginsActor.Ask(messageEnvelope);
        }

        /// <inheritdoc />
        public Task StopAsync(PluginIdentity pluginIdentity)
        {
            var destination = new Address(pluginIdentity, "stop");
            var messageEnvelope = new MessageEnvelope(GarryPlugin.PluginIdentity, destination);

            return pluginsActor.Ask(messageEnvelope);
        }
    }
}
