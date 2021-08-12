using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;

using Autofac;

using GarryDB.Platform.Extensions;
using GarryDB.Platform.Infrastructure;
using GarryDB.Plugins;

#pragma warning disable 1591

namespace GarryDB.Platform.Plugins
{
    public abstract class PluginLifecycle
    {
        protected PluginLifecycle(PluginLifecycle next)
        {
            Next = next;
        }

        protected PluginLifecycle Next { get; }

        public abstract IEnumerable<PluginPackage> Find(string pluginsDirectory);
        public abstract IEnumerable<PluginLoadContext> Prepare(IEnumerable<PluginPackage> pluginPackages);
        public abstract IEnumerable<PluginIdentity> Register(IEnumerable<PluginLoadContext> pluginLoadContexts);
        public abstract IDictionary<PluginIdentity, Plugin> Load(IEnumerable<PluginIdentity> pluginIdentities);
        public abstract void Configure(IEnumerable<PluginIdentity> pluginIdentities);
        public abstract void Start(IEnumerable<PluginIdentity> pluginIdentities);
        public abstract void Stop(IEnumerable<PluginIdentity> pluginIdentities);
    }

    internal sealed class DefaultPluginLifecycle : PluginLifecycle
    {
        private readonly Lazy<IContainer> Container;
        private readonly ContainerBuilder containerBuilder;

        private readonly FileSystem fileSystem;
        private readonly IDictionary<PluginIdentity, int> startupOrders;
        private readonly PluginContextFactory pluginContextFactory;

        public DefaultPluginLifecycle(PluginLifecycle next, FileSystem fileSystem, PluginContextFactory pluginContextFactory)
            : base(next)
        {
            this.fileSystem = fileSystem;
            this.pluginContextFactory = pluginContextFactory;

            containerBuilder = new ContainerBuilder();
            Container = new Lazy<IContainer>(() => containerBuilder.Build());
            startupOrders = new Dictionary<PluginIdentity, int>();
        }

        public override IEnumerable<PluginPackage> Find(string pluginsDirectory)
        {
            return fileSystem.GetTopLevelDirectories(pluginsDirectory)
                .Select(directory => new PluginDirectory(fileSystem, directory))
                .OrderBy(pluginPackage => pluginPackage)
                .ToList();
        }

        public override IEnumerable<PluginLoadContext> Prepare(IEnumerable<PluginPackage> pluginPackages)
        {
            var pluginLoadContexts = new Dictionary<PluginPackage, PluginLoadContext>();
            foreach (PluginPackage pluginPackage in pluginPackages)
            {
                IEnumerable<PluginLoadContext> providers = pluginLoadContexts.Where(x => pluginPackage.IsDependentOn(x.Key)).Select(x => x.Value);

                var loadContext = new PluginLoadContext(pluginPackage, AssemblyLoadContext.Default.AsEnumerable()
                    .Concat(providers)
                    .ToList());

                pluginLoadContexts[pluginPackage] = loadContext;
            }

            return pluginLoadContexts.Values;
        }

        public override IEnumerable<PluginIdentity> Register(IEnumerable<PluginLoadContext> pluginLoadContexts)
        {
            containerBuilder.RegisterType<GarryPlugin>()
                .Keyed<Plugin>(GarryPlugin.PluginIdentity)
                .WithParameter(TypedParameter.From(pluginContextFactory.Create(GarryPlugin.PluginIdentity)))
                .SingleInstance();

            startupOrders[GarryPlugin.PluginIdentity] = int.MinValue;

            foreach (PluginLoadContext pluginLoadContext in pluginLoadContexts)
            {
                PluginAssembly? pluginAssembly = pluginLoadContext.Load();
                if (pluginAssembly == null)
                {
                    yield break;
                }

                PluginIdentity pluginIdentity = pluginAssembly.PluginIdentity;
                containerBuilder.RegisterType(pluginAssembly.PluginType)
                    .Keyed<Plugin>(pluginIdentity)
                    .WithParameter(TypedParameter.From(pluginContextFactory.Create(pluginIdentity)))
                    .SingleInstance();

                containerBuilder.RegisterAssemblyModules(pluginLoadContext.Assemblies.ToArray());

                startupOrders[pluginIdentity] = pluginAssembly.StartupOrder;

                yield return pluginIdentity;
            }
        }

        public override IDictionary<PluginIdentity, Plugin> Load(IEnumerable<PluginIdentity> pluginIdentities)
        {
            var plugins = new Dictionary<PluginIdentity, Plugin>();

            foreach (PluginIdentity pluginIdentity in pluginIdentities)
            {
                Plugin? plugin = Container.Value.ResolveOptionalKeyed<Plugin>(pluginIdentity);

                if (plugin != null)
                {
                    plugins[pluginIdentity] = plugin;
                }
            }

            return plugins;
        }

        public override void Configure(IEnumerable<PluginIdentity> pluginIdentities)
        {
            throw new System.NotImplementedException();
        }

        public override void Start(IEnumerable<PluginIdentity> pluginIdentities)
        {
            throw new System.NotImplementedException();
        }

        public override void Stop(IEnumerable<PluginIdentity> pluginIdentities)
        {
            throw new System.NotImplementedException();
        }
    }
}
