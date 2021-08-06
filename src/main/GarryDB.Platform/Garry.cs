using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
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
        private IContainer? container;

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

        /// <inheritdoc />
        public void Dispose()
        {
            startupSequence.Dispose();
            container?.Dispose();
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

                IEnumerable<PluginDirectory> pluginDirectories =
                    fileSystem.GetTopLevelDirectories(pluginsDirectory)
                              .Select(directory => new PluginDirectory(fileSystem, directory))
                              .OrderBy(directory => directory, Comparer<PluginDirectory>.Create((first, second) =>
                                       {
                                           if (first.DependentAssemblies.Any(assembly =>
                                                                                 second.ProvidedAssemblies.Contains(assembly)))
                                           {
                                               return 1;
                                           }

                                           if (second.DependentAssemblies.Any(assembly =>
                                                                                 first.ProvidedAssemblies.Contains(assembly)))
                                           {
                                               return -1;
                                           }

                                           return 0;
                                       }))
                              .ToList();

                foreach (PluginDirectory pluginDirectory in pluginDirectories)
                {
                    startupSequence.Inspect(new PluginIdentity(pluginDirectory.PluginName));
                }

                var pluginAssemblies = new List<PluginAssembly>();
                var containerBuilder = new ContainerBuilder();
                    var pluginLoadContexts = new List<PluginLoadContext>();

                    foreach (PluginDirectory pluginDirectory in pluginDirectories)
                    {
                        IEnumerable<PluginLoadContext> providers =
                            pluginLoadContexts.Where(x => x.PluginDirectory.ProvidedAssemblies.Any(y => pluginDirectory
                                                         .DependentAssemblies.Contains(y)));

                        var loadContext = new PluginLoadContext(pluginDirectory,
                                                                AssemblyLoadContext.Default.AsEnumerable()
                                                                                   .Concat(providers)
                                                                                   .ToList());
                        pluginLoadContexts.Add(loadContext);
                    }

                    foreach (PluginLoadContext pluginLoadContext in pluginLoadContexts)
                    {
                        startupSequence.Load(new PluginIdentity(pluginLoadContext.Name!));

                        PluginAssembly? pluginAssembly = pluginLoadContext.Load();
                        if (pluginAssembly == null)
                        {
                            continue;
                        }

                        pluginAssemblies.Add(pluginAssembly);

                        containerBuilder.RegisterAssemblyModules(pluginLoadContext.Assemblies.ToArray());
                        containerBuilder.RegisterType(pluginAssembly.PluginType)
                                        .Keyed<Plugin>(pluginAssembly.PluginIdentity)
                                        .SingleInstance();
                    }

                container = containerBuilder.Build();

                IDictionary<PluginIdentity, Plugin> plugins =
                    pluginAssemblies
                        .OrderBy(x => x.StartupOrder)
                        .ToDictionary(x => x.PluginIdentity,
                                                  x => container.ResolveKeyed<Plugin>(x.PluginIdentity,
                                                                                          TypedParameter
                                                                                              .From<
                                                                                                  PluginContext>(new
                                                                                                  AkkaPluginContext(pluginsActor,
                                                                                                      x.PluginIdentity))));

                var garryPlugin = new GarryPlugin(new AkkaPluginContext(pluginsActor, GarryPlugin.PluginIdentity));
                plugins[GarryPlugin.PluginIdentity] = garryPlugin;

                plugins.ForEach(plugin =>
                                {
                                    pluginsActor.Tell(new PluginLoaded(plugin.Key, plugin.Value));
                                });

                plugins.Keys.ForEach(plugin =>
                                     {
                                         startupSequence.Configure(plugin);
                                         pluginsActor.Tell(new MessageEnvelope(GarryPlugin.PluginIdentity,
                                                                               new Address(plugin, "configure")));
                                     });

                plugins.Keys.ForEach(plugin =>
                                     {
                                         startupSequence.Start(plugin);
                                         pluginsActor.Tell(new MessageEnvelope(GarryPlugin.PluginIdentity,
                                                                               new Address(plugin, "start")));
                                     });

                startupSequence.Complete();

                if (plugins.Any(x => x.Key != GarryPlugin.PluginIdentity))
                {
                    garryPlugin.WaitUntilShutdownRequested();
                }

                plugins.Keys.ForEach(plugin =>
                                     {
                                         pluginsActor.Tell(new MessageEnvelope(GarryPlugin.PluginIdentity,
                                                                               new Address(plugin, "stop")));
                                     });
            }
        }

        private sealed class GarryPlugin : Plugin
        {
            public static readonly PluginIdentity PluginIdentity = new("Garry", "1.0");
            private readonly AutoResetEvent shutdownRequested;

            public GarryPlugin(PluginContext pluginContext)
                : base(pluginContext)
            {
                shutdownRequested = new AutoResetEvent(false);

                Register("shutdown", (object _) =>
                                     {
                                         shutdownRequested.Set();

                                         return Task.CompletedTask;
                                     });
            }

            public void WaitUntilShutdownRequested()
            {
                shutdownRequested.WaitOne();
            }
        }
    }
}
