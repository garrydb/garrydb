using System.Collections.Generic;

using Akka.Actor;
using Akka.Event;

using GarryDB.Platform.Actors;
using GarryDB.Platform.Messaging;
using GarryDB.Plugins;

using Address = GarryDB.Platform.Messaging.Address;
#pragma warning disable 1591

namespace GarryDB.Platform.Plugins.Lifecycles
{
    public sealed class ApplyAkkaPluginLifecycle : PluginLifecycle
    {
        private readonly ActorSystem actorSystem;
        private readonly IActorRef pluginsActor;
        private readonly PluginContextFactory pluginContextFactory;

        public ApplyAkkaPluginLifecycle(PluginLifecycle next)
            : base(next)
        {
            actorSystem = ActorSystem.Create("garry");
            pluginsActor = actorSystem.ActorOf(PluginsActor.Props(), "plugins");
            pluginContextFactory = new AkkaPluginContextFactory(pluginsActor);

            MonitorDeadletters();
        }

        public override PluginIdentity? Register(PluginContextFactory pluginContextFactory, PluginPackage pluginPackage)
        {
            return Next.Register(this.pluginContextFactory, pluginPackage);
        }

        public override Plugin? Load(PluginIdentity pluginIdentity)
        {
            Plugin? plugin = Next.Load(pluginIdentity);

            if (plugin != null)
            {
                pluginsActor.Tell(new PluginLoaded(pluginIdentity, plugin));
            }

            return plugin;
        }

        public override object? Configure(PluginIdentity pluginIdentity)
        {
            object? configuration = Next.Configure(pluginIdentity);

            if (configuration == null)
            {
                return configuration;
            }

            var destination = new Address(pluginIdentity, "configure");
            var messageEnvelope = new MessageEnvelope(GarryPlugin.PluginIdentity, destination, configuration);
            pluginsActor.Tell(messageEnvelope);

            return configuration;
        }

        public override void Start(IEnumerable<PluginIdentity> pluginIdentities)
        {
            Next.Start(pluginIdentities);
            
            foreach (PluginIdentity pluginIdentity in pluginIdentities)
            {
                var destination = new Address(pluginIdentity, "start");
                var messageEnvelope = new MessageEnvelope(GarryPlugin.PluginIdentity, destination);
                pluginsActor.Tell(messageEnvelope);
            }
        }

        public override void Stop(IEnumerable<PluginIdentity> pluginIdentities)
        {
            Next.Stop(pluginIdentities);
            
            foreach (PluginIdentity pluginIdentity in pluginIdentities)
            {
                var destination = new Address(pluginIdentity, "stop");
                var messageEnvelope = new MessageEnvelope(GarryPlugin.PluginIdentity, destination);
                pluginsActor.Ask(messageEnvelope).GetAwaiter().GetResult();;
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
