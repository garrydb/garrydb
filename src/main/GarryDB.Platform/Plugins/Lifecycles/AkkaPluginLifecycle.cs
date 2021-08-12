using System.Collections.Generic;

using Akka.Actor;
using Akka.Event;

using GarryDB.Platform.Actors;
using GarryDB.Platform.Messaging;
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

        /// <summary>
        ///     Initializes a new <see cref="AkkaPluginLifecycle" />.
        /// </summary>
        /// <param name="next"></param>
        public AkkaPluginLifecycle(PluginLifecycle next)
        {
            this.next = next;
            actorSystem = ActorSystem.Create("garry");
            pluginsActor = actorSystem.ActorOf(PluginsActor.Props(), "plugins");
            pluginContextFactory = new AkkaPluginContextFactory(pluginsActor);

            MonitorDeadletters();
        }

        /// <inheritdoc />
        public PluginIdentity? Register(PluginContextFactory pluginContextFactory, PluginPackage pluginPackage)
        {
            return next.Register(this.pluginContextFactory, pluginPackage);
        }

        /// <inheritdoc />
        public Plugin? Load(PluginIdentity pluginIdentity)
        {
            Plugin? plugin = next.Load(pluginIdentity);

            if (plugin != null)
            {
                pluginsActor.Tell(new PluginLoaded(pluginIdentity, plugin));
            }

            return plugin;
        }

        /// <inheritdoc />
        public object? Configure(PluginIdentity pluginIdentity)
        {
            object? configuration = next.Configure(pluginIdentity);

            if (configuration == null)
            {
                return configuration;
            }

            var destination = new Address(pluginIdentity, "configure");
            var messageEnvelope = new MessageEnvelope(GarryPlugin.PluginIdentity, destination, configuration);
            pluginsActor.Tell(messageEnvelope);

            return configuration;
        }

        /// <inheritdoc />
        public void Start(IEnumerable<PluginIdentity> pluginIdentities)
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
        public void Stop(IEnumerable<PluginIdentity> pluginIdentities)
        {
            next.Stop(pluginIdentities);
            
            foreach (PluginIdentity pluginIdentity in pluginIdentities)
            {
                var destination = new Address(pluginIdentity, "stop");
                var messageEnvelope = new MessageEnvelope(GarryPlugin.PluginIdentity, destination);
                pluginsActor.Ask(messageEnvelope).GetAwaiter().GetResult();;
            }

            actorSystem.Dispose();
        }

        /// <inheritdoc />
        public IEnumerable<PluginPackage> Find(string pluginsDirectory)
        {
            return next.Find(pluginsDirectory);
        }

        /// <inheritdoc />
        public void DetermineDependencies(IEnumerable<PluginPackage> pluginPackages)
        {
            next.DetermineDependencies(pluginPackages);
        }

        private void MonitorDeadletters()
        {
            var deadletterWatchMonitorProps = Props.Create(() => new DeadletterMonitor());
            IActorRef deadletterWatchActorRef = actorSystem.ActorOf(deadletterWatchMonitorProps, "DeadLetterMonitoringActor");
            actorSystem.EventStream.Subscribe(deadletterWatchActorRef, typeof(DeadLetter));
        }
    }
}
