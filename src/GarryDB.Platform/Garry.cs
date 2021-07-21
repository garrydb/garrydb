using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Akka.Actor;
using Akka.Event;

using Autofac;

using GarryDb.Platform.Infrastructure;

using GarryDB.Platform.Messaging;
using GarryDB.Platform.Messaging.Messages;

using GarryDb.Platform.Plugins;

using GarryDB.Platform.Plugins;

using GarryDb.Platform.Plugins.Inpections;
using GarryDb.Platform.Plugins.Loading;
using GarryDb.Plugins;

using Debug = System.Diagnostics.Debug;

namespace GarryDb.Platform
{
    /// <summary>
    ///     The Garry.
    /// </summary>
    public sealed class Garry
    {
        private readonly FileSystem fileSystem;

        /// <summary>
        ///     Raised when a plugin has been found.
        /// </summary>
        public event EventHandler<PluginEventArgs> PluginFound = delegate { };

        /// <summary>
        ///     Raised when a plugin is being loaded.
        /// </summary>
        public event EventHandler<PluginEventArgs> PluginLoading = delegate { };

        /// <summary>
        ///     Raised when a plugin has been loaded.
        /// </summary>
        public event EventHandler<PluginEventArgs> PluginLoaded = delegate { };

        /// <summary>
        ///     Raised when a plugin is being started.
        /// </summary>
        public event EventHandler<PluginEventArgs> PluginStarting = delegate { };

        /// <summary>
        ///     Raised when a plugin has been started.
        /// </summary>
        public event EventHandler<PluginEventArgs> PluginStarted = delegate { };

        /// <summary>
        ///     Raised when a plugin is being configured.
        /// </summary>
        public event EventHandler<PluginEventArgs> PluginConfiguring = delegate { };

        /// <summary>
        ///     Raised when a plugin has been configured.
        /// </summary>
        public event EventHandler<PluginEventArgs> PluginConfigured = delegate { };

        /// <summary>
        ///     Raised when a plugin is being stopped.
        /// </summary>
        public event EventHandler<PluginEventArgs> PluginStopping = delegate { };

        /// <summary>
        ///     Raised when a plugin has been stopped.
        /// </summary>
        public event EventHandler<PluginEventArgs> PluginStopped = delegate { };

        /// <summary>
        ///     Initializes <see cref="Garry" />.
        /// </summary>
        /// <param name="fileSystem"></param>
        public Garry(FileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        /// <summary>
        ///     Start <see cref="Garry" /> and load the plugins from the <paramref name="pluginsDirectory" />.
        /// </summary>
        /// <param name="pluginsDirectory">The directory containing the plugins.</param>
        public async Task StartAsync(string pluginsDirectory)
        {
            var shutdownRequested = new AutoResetEvent(false);
            using (ActorSystem system = ActorSystem.Create(ActorPaths.Garry.Name))
            {
                var deadletterWatchMonitorProps = Props.Create(() => new DeadletterMonitor());
                IActorRef deadletterWatchActorRef = system.ActorOf(deadletterWatchMonitorProps, "DeadLetterMonitoringActor");
                system.EventStream.Subscribe(deadletterWatchActorRef, typeof(DeadLetter));

                IActorRef pluginsActor = system.ActorOf(PluginsActor.Props(), ActorPaths.Plugins.Name);
                
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

                    CreateActors(pluginsActor, plugins);
                    await ConfigurePlugins(plugins).ConfigureAwait(false);
                    await StartPluginsAsync(plugins).ConfigureAwait(false);

                    shutdownRequested.WaitOne();

                    await StopPluginsAsync(plugins).ConfigureAwait(false);
                }

                Debug.WriteLine("END");
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
                PluginFound(this, new PluginEventArgs(inspectedPlugin.PluginIdentity));
                yield return inspectedPlugin;
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

        private LoadedPlugin LoadPlugin(PluginLoader loader, ContainerBuilder containerBuilder)
        {
            PluginLoading(this, new PluginEventArgs(loader.PluginIdentity));
            LoadedPlugin plugin = loader.Load(containerBuilder);
            PluginLoaded(this, new PluginEventArgs(loader.PluginIdentity));

            return plugin;
        }

        private void CreateActors(IActorRef pluginsActor, IDictionary<PluginIdentity, Plugin> plugins)
        {
            foreach ((PluginIdentity identity, Plugin plugin) in plugins)
            {
                pluginsActor.Tell(new PluginLoaded(identity, plugin));
            }
        }

        private async Task ConfigurePlugins(IDictionary<PluginIdentity, Plugin> plugins)
        {
            foreach ((PluginIdentity identity, Plugin plugin) in plugins)
            {
                PluginConfiguring(this, new PluginEventArgs(identity));
                await plugin.RouteAsync("configure", new object()).ConfigureAwait(false);
                PluginConfigured(this, new PluginEventArgs(identity));
            }
        }

        private async Task StartPluginsAsync(IDictionary<PluginIdentity, Plugin> plugins)
        {
            foreach ((PluginIdentity identity, Plugin plugin) in plugins)
            {
                PluginStarting(this, new PluginEventArgs(identity));
                await plugin.RouteAsync("start", new object()).ConfigureAwait(false);
                PluginStarted(this, new PluginEventArgs(identity));
            }
        }

        private async Task StopPluginsAsync(IDictionary<PluginIdentity, Plugin> plugins)
        {
            foreach ((PluginIdentity identity, Plugin plugin) in plugins)
            {
                PluginStopping(this, new PluginEventArgs(identity));
                await plugin.RouteAsync("stop", new object()).ConfigureAwait(false);
                PluginStopped(this, new PluginEventArgs(identity));
            }
        }

        private sealed class GarryPlugin : Plugin
        {
            public static readonly PluginIdentity PluginIdentity = new PluginIdentity("garry", "1.0");

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
