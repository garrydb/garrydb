using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;

using Akka.Actor;
using Akka.Actor.Setup;
using Akka.Event;

using GarryDB.Platform.Actors;
using GarryDB.Platform.Extensions;
using GarryDB.Platform.Infrastructure;
using GarryDB.Platform.Messaging;
using GarryDB.Platform.Persistence;
using GarryDB.Platform.Plugins;
using GarryDB.Platform.Plugins.Configuration;
using GarryDB.Platform.Startup;
using GarryDB.Plugins;

using Address = GarryDB.Platform.Messaging.Address;

namespace GarryDB.Platform
{
    /// <summary>
    ///     The Garry.
    /// </summary>
    public sealed class Garry : IDisposable
    {
        private readonly FileSystem fileSystem;
        private readonly ConnectionFactory connectionFactory;
        private readonly ActorSystem actorSystem;
        private readonly IActorRef pluginsActor;
        private readonly PluginRegistry pluginRegistry;

        /// <summary>
        ///     Initializes <see cref="Garry" />.
        /// </summary>
        /// <param name="fileSystem"></param>
        /// <param name="connectionFactory">The connectino factory.</param>
        public Garry(FileSystem fileSystem, ConnectionFactory connectionFactory)
        {
            this.fileSystem = fileSystem;
            this.connectionFactory = connectionFactory;

            actorSystem = ActorSystem.Create("garry", ActorSystemSetup.Create());
            pluginsActor = actorSystem.ActorOf(PluginsActor.Props(), "plugins");
            pluginRegistry = new PluginRegistry(new AkkaPluginContextFactory(pluginsActor));

            MonitorDeadletters();
        }

        /// <summary>
        ///     Raised when a <see cref="Plugin" /> is being loaded.
        /// </summary>
        public event EventHandler<PluginLoading>? PluginLoading;

        /// <summary>
        ///     Raised when Garry is about to start.
        /// </summary>
        public event EventHandler? Starting;

        /// <summary>
        ///     Start <see cref="Garry" /> and load the plugins from the <paramref name="pluginsDirectory" />.
        /// </summary>
        /// <param name="pluginsDirectory">The directory containing the plugins.</param>
        public void Start(string pluginsDirectory)
        {
            Load(pluginsDirectory);

            foreach (PluginIdentity pluginIdentity in pluginRegistry)
            {
                pluginsActor.Tell(new Messaging.Messages.PluginLoaded(pluginIdentity, pluginRegistry[pluginIdentity]!));
            }

            var storage = new PluginConfigurationStorage(connectionFactory, pluginRegistry);

            foreach (PluginIdentity pluginIdentity in pluginRegistry)
            {
                object? configuration = storage.FindConfiguration(pluginIdentity);

                if (configuration == null)
                {
                    continue;
                }

                Configure(pluginIdentity, configuration);
            }

            foreach (PluginIdentity pluginIdentity in pluginRegistry)
            {
                Start(pluginIdentity);
            }

            Starting?.Invoke(this, EventArgs.Empty);

            var garryPlugin = (GarryPlugin?)pluginRegistry[GarryPlugin.PluginIdentity];
            garryPlugin?.WaitUntilShutdownRequested();

            foreach (PluginIdentity pluginIdentity in pluginRegistry)
            {
                Stop(pluginIdentity);
            }
        }

        private void Load(string pluginsDirectory)
        {
            IEnumerable<PluginDirectory> pluginDirectories =
                fileSystem.GetTopLevelDirectories(pluginsDirectory)
                    .Select(directory => new PluginDirectory(fileSystem, directory))
                    .OrderBy(pluginDirectory => pluginDirectory)
                    .ToList();

            var pluginLoadContexts = new List<PluginLoadContext>();
            foreach (PluginDirectory pluginDirectory in pluginDirectories)
            {
                IEnumerable<PluginLoadContext> providers =
                    pluginLoadContexts.Where(x => pluginDirectory.IsDependentOn(x.PluginDirectory));

                var loadContext = new PluginLoadContext(pluginDirectory,
                    AssemblyLoadContext.Default.AsEnumerable()
                        .Concat(providers)
                        .ToList());
                pluginLoadContexts.Add(loadContext);

                PluginLoading?.Invoke(this,
                    new PluginLoading(PluginIdentity.Parse(loadContext.PluginDirectory.PluginName), pluginDirectories.Count()));
                pluginRegistry.Load(loadContext);
            }
        }

        private void Configure(PluginIdentity pluginIdentity, object configuration)
        {
            pluginsActor.Tell(new MessageEnvelope(GarryPlugin.PluginIdentity,
                new Address(pluginIdentity, "configure"),
                configuration));
        }

        private void Start(PluginIdentity pluginIdentity)
        {
            pluginsActor.Tell(new MessageEnvelope(GarryPlugin.PluginIdentity,
                new Address(pluginIdentity, "start")));
        }

        private void Stop(PluginIdentity pluginIdentity)
        {
            pluginsActor.Tell(new MessageEnvelope(GarryPlugin.PluginIdentity,
                new Address(pluginIdentity, "stop")));
        }

        private void MonitorDeadletters()
        {
            var deadletterWatchMonitorProps = Props.Create(() => new DeadletterMonitor());
            IActorRef deadletterWatchActorRef = actorSystem.ActorOf(deadletterWatchMonitorProps, "DeadLetterMonitoringActor");
            actorSystem.EventStream.Subscribe(deadletterWatchActorRef, typeof(DeadLetter));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            actorSystem.Dispose();
        }
    }
}
