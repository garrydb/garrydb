using Akka.Actor;
using Akka.Event;

using GarryDB.Platform.Actors;
using GarryDB.Platform.Messaging;
using GarryDB.Platform.Messaging.Messages;
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
            IActorRef? pluginsActor = actorSystem.ActorOf(PluginsActor.Props(), "plugins");

            MonitorDeadletters(actorSystem);

            return
                bootstrapper
                    .Configurer(inner => (pluginIdentity, configuration) =>
                    {
                        inner(pluginIdentity, configuration);
                        if (configuration != null)
                        {
                            SendMessage(pluginsActor, pluginIdentity, "configure", configuration);
                        }
                    })
                    .Starter(inner => pluginIdentities =>
                    {
                        inner(pluginIdentities);

                        foreach (PluginIdentity pluginIdentity in pluginIdentities)
                        {
                            SendMessage(pluginsActor, pluginIdentity, "start");
                        }
                    })
                    .Stopper(inner => pluginIdentities =>
                    {
                        inner(pluginIdentities);

                        foreach (PluginIdentity pluginIdentity in pluginIdentities)
                        {
                            SendMessage(pluginsActor, pluginIdentity, "stop");
                        }
                    })
                    .Loader(inner => pluginIdentity =>
                    {
                        Plugin? plugin = inner(pluginIdentity);

                        if (plugin != null)
                        {
                            pluginsActor.Tell(new PluginLoaded(pluginIdentity, plugin));
                        }

                        return plugin;
                    })
                    .Replace(_ => new AkkaPluginContextFactory(pluginsActor));
        }

        private static void SendMessage(IActorRef pluginsActor, PluginIdentity pluginIdentity, string handler, object? message = null)
        {
            var destination = new Address(pluginIdentity, handler);
            var messageEnvelope = new MessageEnvelope(GarryPlugin.PluginIdentity, destination, message ?? new object());
            pluginsActor.Tell(messageEnvelope);
        }

        private static void MonitorDeadletters(ActorSystem actorSystem)
        {
            var deadletterWatchMonitorProps = Props.Create(() => new DeadletterMonitor());
            IActorRef deadletterWatchActorRef = actorSystem.ActorOf(deadletterWatchMonitorProps, "DeadLetterMonitoringActor");
            actorSystem.EventStream.Subscribe(deadletterWatchActorRef, typeof(DeadLetter));
        }
    }
}
