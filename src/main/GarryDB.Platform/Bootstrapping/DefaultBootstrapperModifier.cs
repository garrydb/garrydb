using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;

using Autofac;

using GarryDB.Platform.Extensions;
using GarryDB.Platform.Infrastructure;
using GarryDB.Platform.Plugins;
using GarryDB.Platform.Plugins.Configuration;
using GarryDB.Plugins;

namespace GarryDB.Platform.Bootstrapping
{
    internal static class DefaultBootstrapperModifier
    {
        private static readonly ContainerBuilder ContainerBuilder = new ContainerBuilder();
        private static readonly Lazy<IContainer> Container = new Lazy<IContainer>(() => ContainerBuilder.Build());
        private static readonly IDictionary<PluginIdentity, Plugin> Plugins = new Dictionary<PluginIdentity, Plugin>();

        public static Bootstrapper Modify(Bootstrapper bootstrapper)
        {
            return
                bootstrapper
                    .Finder(_ => pluginsDirectory => FindPlugins(bootstrapper.FileSystem, pluginsDirectory))
                    .Preparer(_ => pluginDirectories => PreparePlugins(pluginDirectories))
                    .Registrar(_ => pluginLoadContexts => RegisterPlugins(bootstrapper.PluginContextFactory, pluginLoadContexts))
                    .Loader(_ => pluginIdentities => LoadPlugins(pluginIdentities))
                    .Configurer(inner => (pluginIdentity, _) =>
                    {
                        var configurationStorage = new ConfigurationStorage(bootstrapper.ConnectionFactory);
                        object? configuration = configurationStorage.FindConfiguration(pluginIdentity, Plugins[pluginIdentity]);

                        if (configuration == null)
                        {
                            return;
                        }

                        inner(pluginIdentity, configuration);
                    });
        }

        private static IEnumerable<PluginPackage> FindPlugins(FileSystem fileSystem, string pluginsDirectory)
        {
            return fileSystem.GetTopLevelDirectories(pluginsDirectory)
                .Select(directory => new PluginDirectory(fileSystem, directory))
                .OrderBy(pluginDirectory => pluginDirectory)
                .ToList();
        }

        private static IEnumerable<PluginLoadContext> PreparePlugins(IEnumerable<PluginPackage> pluginPackages)
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

        private static IEnumerable<PluginIdentity> RegisterPlugins(PluginContextFactory pluginContextFactory, IEnumerable<PluginLoadContext> pluginLoadContexts)
        {
            ContainerBuilder.RegisterType<GarryPlugin>()
                .Keyed<Plugin>(GarryPlugin.PluginIdentity)
                .WithParameter(TypedParameter.From(pluginContextFactory.Create(GarryPlugin.PluginIdentity)))
                .SingleInstance();

            var startupOrders = new Dictionary<PluginIdentity, int>
            {
                [GarryPlugin.PluginIdentity] = int.MinValue
            };

            foreach (PluginLoadContext pluginLoadContext in pluginLoadContexts)
            {
                PluginAssembly? pluginAssembly = pluginLoadContext.Load();
                if (pluginAssembly == null)
                {
                    continue;
                }

                PluginIdentity pluginIdentity = pluginAssembly.PluginIdentity;
                ContainerBuilder.RegisterType(pluginAssembly.PluginType)
                    .Keyed<Plugin>(pluginIdentity)
                    .WithParameter(TypedParameter.From(pluginContextFactory.Create(pluginIdentity)))
                    .SingleInstance();

                ContainerBuilder.RegisterAssemblyModules(pluginLoadContext.Assemblies.ToArray());

                startupOrders[pluginIdentity] = pluginAssembly.StartupOrder;
            }

            return startupOrders.OrderBy(x => x.Value).Select(x => x.Key);
        }

        private static Plugin? LoadPlugins(PluginIdentity pluginIdentity)
        {
            Plugin? plugin = Container.Value.ResolveOptionalKeyed<Plugin>(pluginIdentity);

            if (plugin != null)
            {
                Plugins[pluginIdentity] = plugin;
            }

            return plugin;
        }
    }
}
