using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;

using Akka.Actor;
using Akka.Event;

using GarryDB.Platform.Databases;
using GarryDB.Platform.Extensions;
using GarryDB.Platform.Infrastructure;
using GarryDB.Platform.Messaging;
using GarryDB.Platform.Messaging.Messages;
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
        private readonly StartupSequence startupSequence;

        /// <summary>
        ///     Initializes <see cref="Garry" />.
        /// </summary>
        /// <param name="fileSystem"></param>
        public Garry(FileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
            startupSequence = new StartupSequence();
        }

        /// <summary>
        ///     Gets the <see cref="IObservable{T}">IObservable&lt;ProgressUpdated&gt;</see> to be notified
        ///     when the startup progress is changed.
        /// </summary>
        public IObservable<StartupProgressUpdated> WhenProgressUpdated
        {
            get { return startupSequence; }
        }

        /// <summary>
        ///     Start <see cref="Garry" /> and load the plugins from the <paramref name="pluginsDirectory" />.
        /// </summary>
        /// <param name="pluginsDirectory">The directory containing the plugins.</param>
        public async Task StartAsync(string pluginsDirectory)
        {
            await Task.CompletedTask;
            using (var system = ActorSystem.Create("garry"))
            {
                var deadletterWatchMonitorProps = Props.Create(() => new DeadletterMonitor());
                IActorRef deadletterWatchActorRef = system.ActorOf(deadletterWatchMonitorProps, "DeadLetterMonitoringActor");
                system.EventStream.Subscribe(deadletterWatchActorRef, typeof(DeadLetter));

                IActorRef pluginsActor = system.ActorOf(PluginsActor.Props(), "plugins");

                List<PluginLoadContext> pluginLoadContexts = CreatePluginLoadContexts(pluginsDirectory);
                var pluginRegistry = new PluginRegistry(pluginsActor);

                pluginLoadContexts.ForEach(pluginLoadContext =>
                                           {
                                               startupSequence.Load(PluginIdentity.Parse(pluginLoadContext.PluginDirectory.PluginName));
                                               pluginRegistry.Load(pluginLoadContext);
                                           });

                IDictionary<PluginIdentity, Plugin> plugins = pluginRegistry.Plugins;

                plugins.ForEach(plugin =>
                                {
                                    pluginsActor.Tell(new PluginLoaded(plugin.Key, plugin.Value));
                                });

                var storage = new PluginConfigurationStorage(new PersistentSqLiteConnectionFactory(fileSystem, Path.Combine(Environment.CurrentDirectory, "data")), pluginRegistry);
                plugins.ForEach(plugin =>
                                     {
                                         startupSequence.Configure(plugin.Key);
                                         object? configuration = storage.FindConfiguration(plugin.Key);
                                         pluginsActor.Tell(new MessageEnvelope(GarryPlugin.PluginIdentity,
                                                                               new Address(plugin.Key, "configure"),
                                                                               configuration ?? new object()));
                                     });

                plugins.ForEach(plugin =>
                                     {
                                         startupSequence.Start(plugin.Key);
                                         pluginsActor.Tell(new MessageEnvelope(GarryPlugin.PluginIdentity,
                                                                               new Address(plugin.Key, "start")));
                                     });

                startupSequence.Complete();

                if (plugins.Any(x => x.Key != GarryPlugin.PluginIdentity))
                {
                    var garryPlugin = (GarryPlugin)plugins[GarryPlugin.PluginIdentity];
                    garryPlugin.WaitUntilShutdownRequested();
                }
                
                plugins.Keys.ForEach(plugin =>
                                     {
                                         pluginsActor.Tell(new MessageEnvelope(GarryPlugin.PluginIdentity,
                                                                               new Address(plugin, "stop")));
                                     });
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            startupSequence.Dispose();
        }

        private List<PluginLoadContext> CreatePluginLoadContexts(string pluginsDirectory)
        {
            IEnumerable<PluginDirectory> pluginDirectories =
                fileSystem.GetTopLevelDirectories(pluginsDirectory)
                          .Select(directory => new PluginDirectory(fileSystem, directory))
                          .OrderBy(pluginDirectory => pluginDirectory)
                          .ToList();

            foreach (PluginDirectory pluginDirectory in pluginDirectories)
            {
                startupSequence.Inspect(PluginIdentity.Parse(pluginDirectory.PluginName));
            }

            var pluginLoadContexts = new List<PluginLoadContext>();

            foreach (PluginDirectory pluginDirectory in pluginDirectories)
            {
                IEnumerable<PluginLoadContext> providers = pluginLoadContexts.Where(x => pluginDirectory.IsDependentOn(x.PluginDirectory));

                var loadContext = new PluginLoadContext(pluginDirectory,
                                                        AssemblyLoadContext.Default.AsEnumerable()
                                                                           .Concat(providers)
                                                                           .ToList());
                pluginLoadContexts.Add(loadContext);
            }

            return pluginLoadContexts;
        }
    }
}
