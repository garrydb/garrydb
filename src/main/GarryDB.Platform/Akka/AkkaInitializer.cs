using Akka.Actor;
using Akka.Event;

using GarryDb.Platform.Plugins;
using GarryDb.Platform.Plugins.Configuration;

namespace GarryDb.Platform.Akka
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AkkaInitializer
    {
        private readonly ActorSystem actorSystem;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurationStorage"></param>
        public AkkaInitializer(ConfigurationStorage configurationStorage)
        {
            actorSystem = ActorSystem.Create("garry");
            IActorRef pluginsActor = actorSystem.ActorOf(PluginsActor.Props(), "plugins");

            PluginRegistry = new PluginRegistry(new AkkaPluginFactory(pluginsActor));
            PluginLifecycle = new AkkaPluginLifecycle(configurationStorage, PluginRegistry, pluginsActor);
  
            MonitorDeadletters();
        }

        /// <summary>
        /// 
        /// </summary>
        public PluginRegistry PluginRegistry { get; }

        /// <summary>
        /// 
        /// </summary>
        public PluginLifecycle PluginLifecycle { get; }
        
        private void MonitorDeadletters()
        {
            var deadletterWatchMonitorProps = Props.Create(() => new DeadletterMonitor());
            IActorRef deadletterWatchActorRef = actorSystem.ActorOf(deadletterWatchMonitorProps, "DeadLetterMonitoringActor");
            actorSystem.EventStream.Subscribe(deadletterWatchActorRef, typeof(DeadLetter));
        }
    }
}
