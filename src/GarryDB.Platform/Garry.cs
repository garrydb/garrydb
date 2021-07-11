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
        private readonly AutoResetEvent shutdownRequested;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileSystem"></param>
        public Garry(FileSystem fileSystem)
        {
            shutdownRequested = new AutoResetEvent(false);
            this.fileSystem = fileSystem;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginsDirectory"></param>
        public async Task StartAsync(string pluginsDirectory)
        {
            using (ActorSystem system = ActorSystem.Create(ActorPaths.Garry.Name))
            {
                var deadletterWatchMonitorProps = Props.Create(() => new DeadletterMonitor());
                IActorRef deadletterWatchActorRef = system.ActorOf(deadletterWatchMonitorProps, "DeadLetterMonitoringActor");
                system.EventStream.Subscribe(deadletterWatchActorRef, typeof(DeadLetter));

                IEnumerable<InspectedPlugin> inspectedPlugins = InspectPlugins(pluginsDirectory).ToList();
                
                IEnumerable<PluginLoader> loaders = new PluginLoaderFactory().Create(inspectedPlugins.ToArray()).ToList();
                IEnumerable<Plugin> plugins = loaders
                    .Select(pluginLoader => pluginLoader.Load())
                    .ToList();

                foreach (PluginLoader loader in loaders)
                {
                    loader.LoadDependencies();
                }

                system.RegisterOnTermination(() => Debug.WriteLine("TERMINATION"));

                foreach (Plugin plugin in plugins)
                {
                    await plugin.StartAsync();
                }

                shutdownRequested.WaitOne(TimeSpan.FromSeconds(30));

                foreach (Plugin plugin in plugins)
                {
                    await plugin.StopAsync();
                }

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
    }

    /// <summary>
    /// 
    /// </summary>
    public class DeadletterMonitor : ReceiveActor
    {
        /// <summary>
        /// 
        /// </summary>
        public DeadletterMonitor()
        {
            Receive<DeadLetter>(dl => HandleDeadletter(dl));
        }

        private void HandleDeadletter(DeadLetter dl)
        {
            Debug.WriteLine($"DeadLetter captured: {dl.Message}, sender: {dl.Sender}, recipient: {dl.Recipient}");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ActorPaths
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly ActorMetadata Garry = new ActorMetadata("garry");

        /// <summary>
        /// 
        /// </summary>
        public static readonly ActorMetadata Plugins = new ActorMetadata("plugins");
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class ActorMetadata
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        public ActorMetadata(string name, ActorMetadata? parent = null)
        {
            Name = name;
            string parentPath = (parent == null) ? "/user" : parent.Path;
            Path = $"{parentPath}/{name}";
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Path { get; }
    }
}
