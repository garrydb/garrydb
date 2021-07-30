using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Akka.Actor;
using Akka.Event;

using Autofac;

using GarryDB.Platform.Extensions;
using GarryDB.Platform.Infrastructure;

using GarryDB.Platform.Messaging;
using GarryDB.Platform.Messaging.Messages;

using GarryDB.Platform.Plugins;
using GarryDB.Platform.Plugins.Inpections;
using GarryDB.Platform.Plugins.Loading;

using GarryDB.Platform.Startup;
using GarryDB.Plugins;

using Address = GarryDB.Platform.Messaging.Address;

namespace GarryDB.Platform
{
    /// <summary>
    ///     The Garry.
    /// </summary>
    public sealed class Garry
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
            var shutdownRequested = new AutoResetEvent(false);
            using (ActorSystem system = ActorSystem.Create("garry"))
            {
                var deadletterWatchMonitorProps = Props.Create(() => new DeadletterMonitor());
                IActorRef deadletterWatchActorRef = system.ActorOf(deadletterWatchMonitorProps, "DeadLetterMonitoringActor");
                system.EventStream.Subscribe(deadletterWatchActorRef, typeof(DeadLetter));

                IActorRef pluginsActor = system.ActorOf(PluginsActor.Props(), "plugins");
                
                var containerBuilder = new ContainerBuilder();
                IEnumerable<LoadedPlugin> loadedPlugins = LoadPlugins(pluginsDirectory, containerBuilder).ToList();

                await using (IContainer container = containerBuilder.Build())
                {
                    IDictionary<PluginIdentity, Plugin> plugins =
                        loadedPlugins
                            .OrderBy(plugin => plugin.StartupOrder)
                            .ToDictionary(
                                plugin => plugin.PluginIdentity,
                                plugin =>
                                    container.ResolveKeyed<Plugin>(
                                        plugin.PluginIdentity,
                                        TypedParameter.From<PluginContext>(new AkkaPluginContext(pluginsActor, plugin.PluginIdentity))
                                    )
                            );

                    plugins[GarryPlugin.PluginIdentity] = new GarryPlugin(shutdownRequested, new AkkaPluginContext(pluginsActor, GarryPlugin.PluginIdentity));

                    plugins.ForEach(plugin =>
                    {
                        pluginsActor.Tell(new PluginLoaded(plugin.Key, plugin.Value));
                    });

                    plugins.Keys.ForEach(plugin =>
                    {
                        startupSequence.Configure(plugin);
                        pluginsActor.Tell(new MessageEnvelope(GarryPlugin.PluginIdentity, new Address(plugin, "configure"), new object()));
                    });

                    plugins.Keys.ForEach(plugin =>
                    {
                        startupSequence.Configure(plugin);
                        pluginsActor.Tell(new MessageEnvelope(GarryPlugin.PluginIdentity, new Address(plugin, "start"), new object()));
                    });

                    startupSequence.Complete();

                    shutdownRequested.WaitOne();

                    plugins.Keys.ForEach(plugin =>
                    {
                        startupSequence.Configure(plugin);
                        pluginsActor.Tell(new MessageEnvelope(GarryPlugin.PluginIdentity, new Address(plugin, "stop"), new object()));
                    });
                }
            }
        }

        private IEnumerable<LoadedPlugin> LoadPlugins(string pluginsDirectory, ContainerBuilder containerBuilder)
        {
            var pluginLoaderFactory = new PluginLoaderFactory();
            IEnumerable<InspectedPlugin> inspectedPlugins = InspectPlugins(pluginsDirectory).ToList();
            IEnumerable<PluginLoader> loaders = pluginLoaderFactory.Create(inspectedPlugins.ToArray());

            foreach (PluginLoader loader in loaders)
            {
                yield return LoadPlugin(loader, containerBuilder);
            }
        }

        private IEnumerable<InspectedPlugin> InspectPlugins(string pluginsDirectory)
        {
            var inspector = new Inspector(fileSystem);

            IEnumerable<InspectedPlugin> inspectedPlugins =
                fileSystem.GetTopLevelDirectories(pluginsDirectory)
                    .Select(directory => inspector.Inspect(directory))
                    .Where(plugin => plugin != null)
                    .Select(plugin => plugin!)
                    .ToList();
            
            foreach (InspectedPlugin inspectedPlugin in inspectedPlugins)
            {
                startupSequence.Inspect(inspectedPlugin.PluginIdentity);
                yield return inspectedPlugin;
            }
        }

        private LoadedPlugin LoadPlugin(PluginLoader loader, ContainerBuilder containerBuilder)
        {
            startupSequence.Load(loader.PluginIdentity);
            LoadedPlugin plugin = loader.Load(containerBuilder);

            return plugin;
        }

        private sealed class GarryPlugin : Plugin
        {
            public static readonly PluginIdentity PluginIdentity = new PluginIdentity("Garry", "1.0");

            public GarryPlugin(EventWaitHandle shutdownRequested, PluginContext pluginContext)
                : base(pluginContext)
            {
                Register("shutdown", (object _) =>
                {
                    shutdownRequested.Set();
                    return Task.CompletedTask;
                });
            }
        }
    }
}
