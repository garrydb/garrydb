using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;

using Autofac;

using GarryDB.Platform.Extensions;
using GarryDB.Platform.Infrastructure;
using GarryDB.Platform.Plugins.Configuration;
using GarryDB.Plugins;
#pragma warning disable 1591

namespace GarryDB.Platform.Plugins.Lifecycles
{
    public sealed class DefaultPluginLifecycle : PluginLifecycle
    {
        private readonly Lazy<IContainer> container;
        private readonly ContainerBuilder containerBuilder;

        private readonly FileSystem fileSystem;
        private readonly ConfigurationStorage configurationStorage;
        private readonly IDictionary<PluginPackage, PluginLoadContext> pluginLoadContexts;
        private readonly IDictionary<PluginIdentity, int> startupOrders;

        public DefaultPluginLifecycle(FileSystem fileSystem, ConfigurationStorage configurationStorage)
            : base(null!)
        {
            this.fileSystem = fileSystem;
            this.configurationStorage = configurationStorage;

            pluginLoadContexts = new Dictionary<PluginPackage, PluginLoadContext>();
            startupOrders = new Dictionary<PluginIdentity, int>();

            containerBuilder = new ContainerBuilder();
            container = new Lazy<IContainer>(() => containerBuilder.Build());
        }

        public override IEnumerable<PluginPackage> Find(string pluginsDirectory)
        {
            // var x = 
            // new SortedSet<PluginPackage>(fileSystem.GetTopLevelDirectories(pluginsDirectory)
            //     .Select(directory => (PluginPackage)new PluginDirectory(fileSystem, directory))
            //     .Concat(new GarryPluginPackage()));


            var x = 
                fileSystem.GetTopLevelDirectories(pluginsDirectory)
                    .Select(directory => (PluginPackage)new PluginDirectory(fileSystem, directory))
                    .Concat(new GarryPluginPackage())
                    .OrderBy(x => x)
                    .ToList();

            return x;
        }

        public override void Prepare(IEnumerable<PluginPackage> pluginPackages)
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

        public override PluginIdentity? Register(PluginContextFactory pluginContextFactory, PluginPackage pluginPackage)
        {
            PluginLoadContext pluginLoadContext = pluginLoadContexts[pluginPackage];

            PluginAssembly? pluginAssembly = pluginLoadContext.Load();
            if (pluginAssembly == null)
            {
                return null;
            }

            PluginIdentity pluginIdentity = pluginAssembly.PluginIdentity;

            containerBuilder.RegisterType(pluginAssembly.PluginType)
                .Keyed<Plugin>(pluginIdentity)
                .WithParameter(TypedParameter.From(pluginContextFactory.Create(pluginIdentity)))
                .SingleInstance();

            containerBuilder.RegisterAssemblyModules(pluginLoadContext.Assemblies.ToArray());

            startupOrders[pluginIdentity] = pluginAssembly.StartupOrder;

            return pluginIdentity;
        }

        public override Plugin? Load(PluginIdentity pluginIdentity)
        {
            Plugin? plugin = container.Value.ResolveOptionalKeyed<Plugin>(pluginIdentity);

            return plugin;
        }

        public override object? Configure(PluginIdentity pluginIdentity)
        {
            Plugin? plugin = Load(pluginIdentity);
            if (plugin == null)
            {
                return null;
            }

            object? configuration = configurationStorage.FindConfiguration(pluginIdentity, plugin);

            return configuration;
        }

        public override void Start(IEnumerable<PluginIdentity> pluginIdentities)
        {
        }

        public override void Stop(IEnumerable<PluginIdentity> pluginIdentities)
        {
        }
    }
}
