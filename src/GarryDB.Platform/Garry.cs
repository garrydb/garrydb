using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Akka.Actor;
using Akka.Event;

using GarryDb.Platform.Infrastructure;

using GarryDB.Platform.Messaging;
using GarryDB.Platform.Messaging.Messages;

using GarryDb.Platform.Plugins;
using GarryDb.Platform.Plugins.Inpections;
using GarryDb.Platform.Plugins.Loading;

using Debug = System.Diagnostics.Debug;

namespace GarryDb.Platform
{
    /// <summary>
    ///     The Garry.
    /// </summary>
    public sealed class Garry
    {
        private readonly FileSystem fileSystem;
        private readonly AutoResetEvent shutdownRequested;

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
            shutdownRequested = new AutoResetEvent(false);
            this.fileSystem = fileSystem;
        }

        /// <summary>
        ///     Start <see cref="Garry" /> and load the plugins from the <paramref name="pluginsDirectory" />.
        /// </summary>
        /// <param name="pluginsDirectory">The directory containing the plugins.</param>
        public async Task StartAsync(string pluginsDirectory)
        {
            using (ActorSystem system = ActorSystem.Create(ActorPaths.Garry.Name))
            {
                var deadletterWatchMonitorProps = Props.Create(() => new DeadletterMonitor());
                IActorRef deadletterWatchActorRef = system.ActorOf(deadletterWatchMonitorProps, "DeadLetterMonitoringActor");
                system.EventStream.Subscribe(deadletterWatchActorRef, typeof(DeadLetter));

                IActorRef pluginsActor = system.ActorOf(PluginsActor.Props(), ActorPaths.Plugins.Name);

                IEnumerable<InspectedPlugin> inspectedPlugins = InspectPlugins(pluginsDirectory).ToList();

                foreach (InspectedPlugin inspectedPlugin in inspectedPlugins)
                {
                    PluginFound(this, new PluginEventArgs(inspectedPlugin.PluginIdentity));
                }

                IEnumerable<LoadedPlugin> plugins = LoadPlugins(pluginsActor, new PluginLoaderFactory(), inspectedPlugins).ToList();

                await ConfigurePlugins(plugins);
                await StartPluginsAsync(plugins);

                shutdownRequested.WaitOne(TimeSpan.FromSeconds(30));

                await StopPluginsAsync(plugins);

                Debug.WriteLine("END");
            }
        }

        private IEnumerable<InspectedPlugin> InspectPlugins(string pluginsDirectory)
        {
            var inspector = new Inspector(fileSystem);
            foreach (string directory in fileSystem.GetTopLevelDirectories(pluginsDirectory))
            {
                InspectedPlugin? inspectedPlugin = inspector.Inspect(directory);
                if (inspectedPlugin != null)
                {
                    yield return inspectedPlugin;
                }
            }
        }

        private IEnumerable<LoadedPlugin> LoadPlugins(
            IActorRef pluginsActor,
            PluginLoaderFactory pluginLoaderFactory,
            IEnumerable<InspectedPlugin> inspectedPlugins
        )
        {
            IEnumerable<PluginLoader> loaders = pluginLoaderFactory.Create(inspectedPlugins.ToArray());

            foreach (PluginLoader loader in loaders)
            {
                PluginLoading(this, new PluginEventArgs(loader.PluginIdentity));
                LoadedPlugin plugin = loader.Load();
                pluginsActor.Tell(new PluginLoaded(plugin.PluginIdentity, plugin.Plugin));

                yield return plugin;

                PluginLoaded(this, new PluginEventArgs(loader.PluginIdentity));
            }
        }

        private Task ConfigurePlugins(IEnumerable<LoadedPlugin> plugins)
        {
            foreach (LoadedPlugin loadedPlugin in plugins)
            {
                PluginConfiguring(this, new PluginEventArgs(loadedPlugin.PluginIdentity));

                PluginConfigured(this, new PluginEventArgs(loadedPlugin.PluginIdentity));
            }

            return Task.CompletedTask;
        }

        private async Task StartPluginsAsync(IEnumerable<LoadedPlugin> plugins)
        {
            foreach (LoadedPlugin loadedPlugin in plugins.OrderBy(x => x.StartupOrder))
            {
                PluginStarting(this, new PluginEventArgs(loadedPlugin.PluginIdentity));
                await loadedPlugin.Plugin.StartAsync();
                PluginStarted(this, new PluginEventArgs(loadedPlugin.PluginIdentity));
            }
        }

        private async Task StopPluginsAsync(IEnumerable<LoadedPlugin> plugins)
        {
            foreach (LoadedPlugin loadedPlugin in plugins)
            {
                PluginStopping(this, new PluginEventArgs(loadedPlugin.PluginIdentity));
                await loadedPlugin.Plugin.StopAsync();
                PluginStopped(this, new PluginEventArgs(loadedPlugin.PluginIdentity));
            }
        }
    }
}
