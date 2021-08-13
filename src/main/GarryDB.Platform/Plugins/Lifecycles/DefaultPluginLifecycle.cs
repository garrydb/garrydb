using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;

using Autofac;

using GarryDB.Platform.Extensions;
using GarryDB.Platform.Infrastructure;
using GarryDB.Plugins;

namespace GarryDB.Platform.Plugins.Lifecycles
{
    /// <summary>
    ///     The default lifecycle of the plugins.
    /// </summary>
    public sealed class DefaultPluginLifecycle : PluginLifecycle
    {
        private readonly Lazy<IContainer> container;
        private readonly ContainerBuilder containerBuilder;

        private readonly FileSystem fileSystem;
        private readonly IDictionary<PluginPackage, PluginLoadContext> pluginLoadContexts;
        private readonly IDictionary<PluginIdentity, int> startupOrders;

        /// <summary>
        ///     Initializes a new <see cref="DefaultPluginLifecycle" />.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        public DefaultPluginLifecycle(FileSystem fileSystem)
        {
            this.fileSystem = fileSystem;

            pluginLoadContexts = new Dictionary<PluginPackage, PluginLoadContext>();
            startupOrders = new Dictionary<PluginIdentity, int>();

            containerBuilder = new ContainerBuilder();
            container = new Lazy<IContainer>(() => containerBuilder.Build());
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
        public PluginIdentity? Load(PluginContextFactory pluginContextFactory, PluginPackage pluginPackage)
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

        /// <inheritdoc />
        public Plugin? Instantiate(PluginIdentity pluginIdentity)
        {
            Plugin? plugin = container.Value.ResolveOptionalKeyed<Plugin>(pluginIdentity);

            return plugin;
        }

        /// <inheritdoc />
        public void Configure(PluginIdentity pluginIdentity)
        {
        }

        /// <inheritdoc />
        public void Start(IReadOnlyList<PluginIdentity> pluginIdentities)
        {
        }

        /// <inheritdoc />
        public void Stop(IReadOnlyList<PluginIdentity> pluginIdentities)
        {
        }
    }
}
