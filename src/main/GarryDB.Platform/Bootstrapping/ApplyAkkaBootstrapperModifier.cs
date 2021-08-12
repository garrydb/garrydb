using Akka.Actor;
using Akka.Event;

using GarryDB.Platform.Actors;
using GarryDB.Platform.Messaging;
using GarryDB.Platform.Plugins;
using GarryDB.Plugins;

using Address = GarryDB.Platform.Messaging.Address;

namespace GarryDB.Platform.Bootstrapping
{
    internal static class ApplyAkkaBootstrapperModifier
    {
        public static Bootstrapper Modify(Bootstrapper bootstrapper)
        {
            var actorSystem = ActorSystem.Create("garry");
            IActorRef pluginsActor = actorSystem.ActorOf(PluginsActor.Props(), "plugins");

            MonitorDeadletters(actorSystem);

            return Modify(bootstrapper, actorSystem, pluginsActor);
        }

        public static Bootstrapper Modify(Bootstrapper bootstrapper, ActorSystem actorSystem, IActorRef pluginsActor)
        {
            return bootstrapper
                .Loader(inner => pluginIdentity =>
                {
                    Plugin? plugin = inner(pluginIdentity);

                    if (plugin != null)
                    {
                        pluginsActor.Tell(new PluginLoaded(pluginIdentity, plugin));
                    }

                    return plugin;
                })
                .Configurer(inner => (pluginIdentity, configuration) =>
                {
                    inner(pluginIdentity, configuration);
                    if (configuration == null)
                    {
                        return;
                    }

                    var destination = new Address(pluginIdentity, "configure");
                    var messageEnvelope = new MessageEnvelope(GarryPlugin.PluginIdentity, destination, configuration);
                    pluginsActor.Tell(messageEnvelope);
                })
                .Starter(inner => pluginIdentities =>
                {
                    inner(pluginIdentities);

                    foreach (PluginIdentity pluginIdentity in pluginIdentities)
                    {
                        var destination = new Address(pluginIdentity, "start");
                        var messageEnvelope = new MessageEnvelope(GarryPlugin.PluginIdentity, destination);
                        pluginsActor.Tell(messageEnvelope);
                    }
                })
                .Stopper(inner => pluginIdentities =>
                {
                    inner(pluginIdentities);

                    foreach (PluginIdentity pluginIdentity in pluginIdentities)
                    {
                        var destination = new Address(pluginIdentity, "stop");
                        var messageEnvelope = new MessageEnvelope(GarryPlugin.PluginIdentity, destination);
                            
                        pluginsActor.Ask(messageEnvelope).GetAwaiter().GetResult();
                    }

                    actorSystem.Dispose();
                })
                .Use(new AkkaPluginContextFactory(pluginsActor));
        }

        private static void MonitorDeadletters(ActorSystem actorSystem)
        {
            var deadletterWatchMonitorProps = Props.Create(() => new DeadletterMonitor());
            IActorRef deadletterWatchActorRef = actorSystem.ActorOf(deadletterWatchMonitorProps, "DeadLetterMonitoringActor");
            actorSystem.EventStream.Subscribe(deadletterWatchActorRef, typeof(DeadLetter));
        }
    }
}
