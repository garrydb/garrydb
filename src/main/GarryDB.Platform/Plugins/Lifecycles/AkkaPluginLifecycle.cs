using System;
using System.Threading.Tasks;

using Akka.Actor;
using Akka.Event;

using GarryDb.Platform.Actors;
using GarryDb.Platform.Messaging;
using GarryDb.Platform.Plugins.Configuration;
using GarryDb.Plugins;

using Address = GarryDb.Platform.Messaging.Address;

namespace GarryDb.Platform.Plugins.Lifecycles
{
    /// <summary>
    ///     
    /// </summary>
    public sealed class AkkaPluginLifecycle : PluginLifecycle
    {
        private readonly ActorSystem actorSystem;
        private readonly IActorRef pluginsActor;
        private readonly ConfigurationStorage configurationStorage;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurationStorage">The configuration storage.</param>
        public AkkaPluginLifecycle(ConfigurationStorage configurationStorage)
        {
            actorSystem = ActorSystem.Create("garry");
            pluginsActor = actorSystem.ActorOf(PluginsActor.Props(), "plugins");

            this.configurationStorage = configurationStorage;

            PluginRegistry = new PluginRegistry(new AkkaPluginFactory(pluginsActor));

            PluginRegistry.WhenPluginLoaded.Subscribe(identity =>
            {
                pluginsActor.Tell(new PluginLoaded(identity, PluginRegistry[identity]));
            });

            MonitorDeadletters();
        }

        /// <summary>
        ///     Gets the <see cref="Plugins.PluginRegistry" />.
        /// </summary>
        public PluginRegistry PluginRegistry { get; }

        /// <inheritdoc />
        public Task ConfigureAsync(PluginIdentity pluginIdentity)
        {
            Plugin plugin = PluginRegistry[pluginIdentity];
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

        private void MonitorDeadletters()
        {
            var deadletterWatchMonitorProps = Props.Create(() => new DeadletterMonitor());
            IActorRef deadletterWatchActorRef = actorSystem.ActorOf(deadletterWatchMonitorProps, "DeadLetterMonitoringActor");
            actorSystem.EventStream.Subscribe(deadletterWatchActorRef, typeof(DeadLetter));
        }
    }
}
