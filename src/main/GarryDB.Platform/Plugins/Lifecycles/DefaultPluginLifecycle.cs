using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;

using Akka.Actor;
using Akka.Event;

using GarryDB.Platform.Actors;
using GarryDB.Platform.Extensions;
using GarryDB.Platform.Messaging;
using GarryDB.Platform.Plugins.Configuration;
using GarryDB.Platform.Plugins.PackageSources;
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
        private readonly ActorSystem actorSystem;
        private readonly IActorRef pluginsActor;
        private readonly IEnumerable<PluginPackageSource> pluginPackageSources;
        private readonly ConfigurationStorage configurationStorage;

        private readonly IDictionary<PluginPackage, PluginLoadContext> pluginLoadContexts;
        private readonly IDictionary<PluginIdentity, Lazy<Plugin>> pluginFactories;
        private readonly IDictionary<PluginIdentity, int> startupOrders;
        private readonly PluginContextFactory pluginContextFactory;

        /// <summary>
        ///     Initializes a new <see cref="DefaultPluginLifecycle" />.
        /// </summary>
        /// <param name="pluginPackageSources">The plugin package sources.</param>
        /// <param name="configurationStorage">The configuration storage.</param>
        public DefaultPluginLifecycle(IEnumerable<PluginPackageSource> pluginPackageSources, ConfigurationStorage configurationStorage)
        {
            this.pluginPackageSources = pluginPackageSources.Concat(new HardCodedPluginPackageSource(new GarryPluginPackage()));
            this.configurationStorage = configurationStorage;
            actorSystem = ActorSystem.Create("garry");
            pluginsActor = actorSystem.ActorOf(PluginsActor.Props(), "plugins");

            MonitorDeadletters();

            pluginContextFactory = new AkkaPluginContextFactory(pluginsActor);
            pluginLoadContexts = new Dictionary<PluginPackage, PluginLoadContext>();
            startupOrders = new Dictionary<PluginIdentity, int>();
            pluginFactories = new Dictionary<PluginIdentity, Lazy<Plugin>>();
        }

        /// <inheritdoc />
        public Task<IEnumerable<PluginPackage>> FindAsync()
        {
            IEnumerable<PluginPackage> packages =
                pluginPackageSources
                    .SelectMany(pluginPackageSource => pluginPackageSource.PluginPackages)
                    .ToList();
            
            DetermineDependencies(packages);

            return Task.FromResult(packages);
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
        public Task<PluginIdentity?> LoadAsync(PluginPackage pluginPackage)
        {
            PluginLoadContext pluginLoadContext = pluginLoadContexts[pluginPackage];

            PluginAssembly? pluginAssembly = pluginLoadContext.Load();
            if (pluginAssembly == null)
            {
                return Task.FromResult<PluginIdentity?>(default);
            }

            PluginIdentity pluginIdentity = pluginAssembly.PluginIdentity;
            PluginContext pluginContext = pluginContextFactory.Create(pluginIdentity);

            startupOrders[pluginIdentity] = pluginAssembly.StartupOrder;
            pluginFactories[pluginIdentity] = new Lazy<Plugin>(() => (Plugin)Activator.CreateInstance(pluginAssembly.PluginType, pluginContext)!);

            return Task.FromResult<PluginIdentity?>(pluginIdentity);
        }

        /// <inheritdoc />
        public Task<Plugin?> InstantiateAsync(PluginIdentity pluginIdentity)
        {
            Plugin plugin = pluginFactories[pluginIdentity].Value;

            pluginsActor.Tell(new PluginLoaded(pluginIdentity, plugin));

            return Task.FromResult<Plugin?>(plugin);
        }

        /// <inheritdoc />
        public Task ConfigureAsync(PluginIdentity pluginIdentity)
        {
            Plugin plugin = pluginFactories[pluginIdentity].Value;

            object? configuration = configurationStorage.FindConfiguration(pluginIdentity, plugin);

            if (configuration == null)
            {
                return Task.CompletedTask;
            }

            var destination = new Address(pluginIdentity, "configure");
            var messageEnvelope = new MessageEnvelope(GarryPlugin.PluginIdentity, destination, configuration);

            pluginsActor.Tell(messageEnvelope);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task StartAsync()
        {
            IEnumerable<PluginIdentity> startupOrder =
                startupOrders
                    .OrderBy(x => x.Value)
                    .Select(x => x.Key)
                    .ToList();

            foreach (PluginIdentity pluginIdentity in startupOrder)
            {
                Debug.WriteLine("**** Starting " + pluginIdentity);
                var destination = new Address(pluginIdentity, "start");
                var messageEnvelope = new MessageEnvelope(GarryPlugin.PluginIdentity, destination);

                if (pluginIdentity == startupOrder.Last())
                {
                    await pluginsActor.Ask(messageEnvelope).ConfigureAwait(false);
                }
                else
                {
                    pluginsActor.Tell(messageEnvelope);
                }
            }
        }

        /// <inheritdoc />
        public async Task StopAsync()
        {
            foreach (PluginIdentity pluginIdentity in startupOrders.Keys)
            {
                var destination = new Address(pluginIdentity, "stop");
                var messageEnvelope = new MessageEnvelope(GarryPlugin.PluginIdentity, destination);

                await pluginsActor.Ask(messageEnvelope).ConfigureAwait(false);
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
