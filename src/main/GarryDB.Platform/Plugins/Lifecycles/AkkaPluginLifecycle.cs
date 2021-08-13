using System.Collections.Generic;

using Akka.Actor;
using Akka.Event;

using GarryDB.Platform.Actors;
using GarryDB.Platform.Messaging;
using GarryDB.Platform.Plugins.Configuration;
using GarryDB.Plugins;

using Address = GarryDB.Platform.Messaging.Address;

namespace GarryDB.Platform.Plugins.Lifecycles
{
    /// <summary>
    ///     Adds Akka to the <see cref="PluginLifecycle" />.
    /// </summary>
    public sealed class AkkaPluginLifecycle : PluginLifecycle
    {
        private readonly PluginLifecycle next;
        private readonly ActorSystem actorSystem;
        private readonly IActorRef pluginsActor;
        private readonly PluginContextFactory pluginContextFactory;
        private readonly ConfigurationStorage configurationStorage;

        /// <summary>
        ///     Initializes a new <see cref="AkkaPluginLifecycle" />.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="configurationStorage">The configuration storage.</param>
        public AkkaPluginLifecycle(PluginLifecycle next, ConfigurationStorage configurationStorage)
        {
            this.next = next;
            this.configurationStorage = configurationStorage;
            actorSystem = ActorSystem.Create("garry");
            pluginsActor = actorSystem.ActorOf(PluginsActor.Props(), "plugins");
            pluginContextFactory = new AkkaPluginContextFactory(pluginsActor);

            MonitorDeadletters();
        }

        /// <inheritdoc />
        public IEnumerable<PluginPackage> Find(string pluginsDirectory)
        {
            return next.Find(pluginsDirectory);
        }

        /// <inheritdoc />
        public PluginIdentity? Load(PluginContextFactory pluginContextFactory, PluginPackage pluginPackage)
        {
            return next.Load(this.pluginContextFactory, pluginPackage);
        }

        /// <inheritdoc />
        public Plugin? Instantiate(PluginIdentity pluginIdentity)
        {
            Plugin? plugin = next.Instantiate(pluginIdentity);

            if (plugin != null)
            {
                pluginsActor.Tell(new PluginLoaded(pluginIdentity, plugin));
            }

            return plugin;
        }

        /// <inheritdoc />
        public void Configure(PluginIdentity pluginIdentity)
        {
            Plugin? plugin =  next.Instantiate(pluginIdentity);
            if (plugin == null)
            {
                return;
            }

            object? configuration = configurationStorage.FindConfiguration(pluginIdentity, plugin);

            if (configuration == null)
            {
                return;
            }

            var destination = new Address(pluginIdentity, "configure");
            var messageEnvelope = new MessageEnvelope(GarryPlugin.PluginIdentity, destination, configuration);
            pluginsActor.Tell(messageEnvelope);
        }

        /// <inheritdoc />
        public void Start(IReadOnlyList<PluginIdentity> pluginIdentities)
        {
            next.Start(pluginIdentities);
            
            foreach (PluginIdentity pluginIdentity in pluginIdentities)
            {
                var destination = new Address(pluginIdentity, "start");
                var messageEnvelope = new MessageEnvelope(GarryPlugin.PluginIdentity, destination);
                pluginsActor.Tell(messageEnvelope);
            }
        }

        /// <inheritdoc />
        public void Stop(IReadOnlyList<PluginIdentity> pluginIdentities)
        {
            next.Stop(pluginIdentities);
            
            foreach (PluginIdentity pluginIdentity in pluginIdentities)
            {
                var destination = new Address(pluginIdentity, "stop");
                var messageEnvelope = new MessageEnvelope(GarryPlugin.PluginIdentity, destination);
                pluginsActor.Ask(messageEnvelope).GetAwaiter().GetResult();
            }

            actorSystem.Dispose();
        }

        private void MonitorDeadletters()
        {
            var deadletterWatchMonitorProps = Props.Create(() => new DeadletterMonitor());
            IActorRef deadletterWatchActorRef = actorSystem.ActorOf(deadletterWatchMonitorProps, "DeadLetterMonitoringActor");
            actorSystem.EventStream.Subscribe(deadletterWatchActorRef, typeof(DeadLetter));
        }
    }
}
