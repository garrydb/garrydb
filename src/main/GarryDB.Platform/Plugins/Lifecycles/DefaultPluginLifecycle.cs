using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;

using Akka.Actor;
using Akka.Event;

using GarryDB.Platform.Actors;
using GarryDB.Platform.Extensions;
using GarryDB.Platform.Infrastructure;
using GarryDB.Platform.Messaging;
using GarryDB.Platform.Plugins.Configuration;
using GarryDB.Plugins;

using Address = GarryDB.Platform.Messaging.Address;
using Debug = System.Diagnostics.Debug;

namespace GarryDB.Platform.Plugins.Lifecycles
{
    /// <summary>
    ///     The default lifecycle of the plugins.
    /// </summary>
    public sealed class DefaultPluginLifecycle : PluginLifecycle
    {
        private readonly FileSystem fileSystem;
        private readonly ActorSystem actorSystem;
        private readonly IActorRef pluginsActor;
        private readonly ConfigurationStorage configurationStorage;

        private readonly IDictionary<PluginPackage, PluginLoadContext> pluginLoadContexts;
        private readonly IDictionary<PluginIdentity, Func<Plugin>> pluginFactories;
        private readonly IDictionary<PluginIdentity, int> startupOrders;
        private readonly PluginContextFactory pluginContextFactory;

        /// <summary>
        ///     Initializes a new <see cref="DefaultPluginLifecycle" />.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="configurationStorage">The configuration storage.</param>
        public DefaultPluginLifecycle(FileSystem fileSystem, ConfigurationStorage configurationStorage)
        {
            this.fileSystem = fileSystem;
            this.configurationStorage = configurationStorage;
            actorSystem = ActorSystem.Create("garry");
            pluginsActor = actorSystem.ActorOf(PluginsActor.Props(), "plugins");

            MonitorDeadletters();

            pluginContextFactory = new AkkaPluginContextFactory(pluginsActor);
            pluginLoadContexts = new Dictionary<PluginPackage, PluginLoadContext>();
            startupOrders = new Dictionary<PluginIdentity, int>();
            pluginFactories = new Dictionary<PluginIdentity, Func<Plugin>>();
        }

        /// <inheritdoc />
        public IEnumerable<PluginPackage> Find(string pluginsDirectory)
        {
            var packages =
                fileSystem.GetTopLevelDirectories(pluginsDirectory)
                    .Select(directory => (PluginPackage)new PluginDirectory(fileSystem, directory))
                    .Concat(new GarryPluginPackage())
                    .OrderBy(x => x)
                    .ToList();

            DetermineDependencies(packages);

            return packages;
        }

        private void DetermineDependencies(IEnumerable<PluginPackage> pluginPackages)
        {
            IDictionary<AssemblyLoadContext, AssemblyProvider> assemblyProviders =
                new Dictionary<AssemblyLoadContext, AssemblyProvider>
                {
                    { AssemblyLoadContext.Default, new AssemblyProvider(AssemblyLoadContext.Default) }
                };

            foreach (PluginPackage pluginPackage in pluginPackages)
            {
                IEnumerable<AssemblyLoadContext> providers =
                    AssemblyLoadContext.Default.AsEnumerable()
                        .Concat(
                            pluginLoadContexts
                                .Where(x => pluginPackage.IsDependentOn(x.Key))
                                .Select(x => x.Value)
                        .Select(provider => provider))
                        .ToList();

                var loadContext = new PluginLoadContext(pluginPackage, providers.Select(x => assemblyProviders[x]).ToList());

                pluginLoadContexts[pluginPackage] = loadContext;
                assemblyProviders[loadContext] = new AssemblyProvider(loadContext);
            }
        }

        /// <inheritdoc />
        public PluginIdentity? Load(PluginPackage pluginPackage)
        {
            PluginLoadContext pluginLoadContext = pluginLoadContexts[pluginPackage];

            PluginAssembly? pluginAssembly = pluginLoadContext.Load();
            if (pluginAssembly == null)
            {
                return null;
            }

            PluginIdentity pluginIdentity = pluginAssembly.PluginIdentity;
            PluginContext pluginContext = pluginContextFactory.Create(pluginIdentity);

            startupOrders[pluginIdentity] = pluginAssembly.StartupOrder;
            pluginFactories[pluginIdentity] = () => (Plugin)Activator.CreateInstance(pluginAssembly.PluginType, pluginContext)!;

            return pluginIdentity;
        }

        /// <inheritdoc />
        public Plugin Instantiate(PluginIdentity pluginIdentity)
        {
            Plugin plugin = pluginFactories[pluginIdentity]();

            pluginsActor.Tell(new PluginLoaded(pluginIdentity, plugin));

            return plugin;
        }

        /// <inheritdoc />
        public void Configure(PluginIdentity pluginIdentity)
        {
            Plugin plugin = pluginFactories[pluginIdentity]();

            object? configuration = configurationStorage.FindConfiguration(pluginIdentity, plugin);

            if (configuration == null)
            {
                return;
            }

            var destination = new Address(pluginIdentity, "configure");
            var messageEnvelope = new MessageEnvelope(GarryPlugin.PluginIdentity, destination, configuration);
            pluginsActor.Tell(messageEnvelope);
        }

        /// <inheritdoc />
        public void Start(IReadOnlyList<PluginIdentity> pluginIdentities)
        {
            foreach (PluginIdentity pluginIdentity in startupOrders.OrderBy(x => x.Value).Select(x => x.Key))
            {
                Debug.WriteLine("**** Starting " + pluginIdentity);
                var destination = new Address(pluginIdentity, "start");
                var messageEnvelope = new MessageEnvelope(GarryPlugin.PluginIdentity, destination);
                pluginsActor.Ask(messageEnvelope).GetAwaiter().GetResult();
            }
        }

        /// <inheritdoc />
        public void Stop(IReadOnlyList<PluginIdentity> pluginIdentities)
        {
            foreach (PluginIdentity pluginIdentity in pluginIdentities)
            {
                var destination = new Address(pluginIdentity, "stop");
                var messageEnvelope = new MessageEnvelope(GarryPlugin.PluginIdentity, destination);
                pluginsActor.Ask(messageEnvelope).GetAwaiter().GetResult();
            }

            actorSystem.Dispose();
        }
   
        private void MonitorDeadletters()
        {
            var deadletterWatchMonitorProps = Props.Create(() => new DeadletterMonitor());
            IActorRef deadletterWatchActorRef = actorSystem.ActorOf(deadletterWatchMonitorProps, "DeadLetterMonitoringActor");
            actorSystem.EventStream.Subscribe(deadletterWatchActorRef, typeof(DeadLetter));
        }
    }
}
