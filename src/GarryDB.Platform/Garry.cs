using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Akka.Actor;
using Akka.Event;

using GarryDb.Platform.Infrastructure;
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
        /// 
        /// </summary>
        public event EventHandler<PluginEventArgs> PluginFound = delegate { };

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<PluginEventArgs> PluginLoading = delegate { };

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<PluginEventArgs> PluginLoaded = delegate { };

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<PluginEventArgs> PluginStarting = delegate { };

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<PluginEventArgs> PluginStarted = delegate { };

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<PluginEventArgs> PluginStopping = delegate { };

        /// <summary>
        /// 
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

                IEnumerable<InspectedPlugin> inspectedPlugins = InspectPlugins(pluginsDirectory).ToList();

                foreach (InspectedPlugin inspectedPlugin in inspectedPlugins)
                {
                    PluginFound(this, new PluginEventArgs(inspectedPlugin.PluginIdentity));
                }

                IEnumerable<LoadedPlugin> plugins = LoadPlugins(new PluginLoaderFactory(), inspectedPlugins).ToList();

                system.RegisterOnTermination(() => Debug.WriteLine("TERMINATION"));
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

        private IEnumerable<LoadedPlugin> LoadPlugins(PluginLoaderFactory pluginLoaderFactory, IEnumerable<InspectedPlugin> inspectedPlugins)
        {
            IEnumerable<PluginLoader> loaders = pluginLoaderFactory.Create(inspectedPlugins.ToArray());

            foreach (PluginLoader loader in loaders)
            {
                PluginLoading(this, new PluginEventArgs(loader.PluginIdentity));
                yield return loader.Load();

                PluginLoaded(this, new PluginEventArgs(loader.PluginIdentity));
            }
        }

        private async Task StartPluginsAsync(IEnumerable<LoadedPlugin> plugins)
        {
            foreach (LoadedPlugin loadedPlugin in plugins)
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
