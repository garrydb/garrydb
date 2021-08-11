using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;

using Autofac;

using GarryDB.Platform.Actors;
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
        private static readonly IDictionary<PluginIdentity, int> StartupOrders = new Dictionary<PluginIdentity, int>();
        private static readonly IDictionary<PluginIdentity, Plugin> Plugins = new Dictionary<PluginIdentity, Plugin>();

        public static Bootstrapper Modify(Bootstrapper bootstrapper)
        {
            return
                bootstrapper
                    .Finder(_ => pluginsDirectory => FindPlugins(bootstrapper.FileSystem, pluginsDirectory))
                    .Preparer(_ => pluginDirectories => PreparePlugins(pluginDirectories))
                    .Registrar(_ => pluginLoadContexts => RegisterPlugins(bootstrapper.PluginContextFactory, pluginLoadContexts).Concat(GarryPlugin.PluginIdentity))
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
                    })
                    .Starter(inner => pluginIdentities =>
                    {
                        inner(pluginIdentities.OrderBy(p => StartupOrders[p]).ToList());
                    });
        }

        private static IEnumerable<PluginDirectory> FindPlugins(FileSystem fileSystem, string pluginsDirectory)
        {
            return fileSystem.GetTopLevelDirectories(pluginsDirectory)
                .Select(directory => new PluginDirectory(fileSystem, directory))
                .OrderBy(pluginDirectory => pluginDirectory)
                .ToList();
        }

        private static IEnumerable<PluginLoadContext> PreparePlugins(IEnumerable<PluginDirectory> pluginDirectories)
        {
            var pluginLoadContexts = new List<PluginLoadContext>();
            foreach (PluginDirectory pluginDirectory in pluginDirectories)
            {
                IEnumerable<PluginLoadContext> providers = pluginLoadContexts.Where(x => pluginDirectory.IsDependentOn(x.PluginDirectory));

                var loadContext = new PluginLoadContext(pluginDirectory, AssemblyLoadContext.Default.AsEnumerable()
                    .Concat(providers)
                    .ToList());

                pluginLoadContexts.Add(loadContext);
            }

            return pluginLoadContexts;
        }

        private static IEnumerable<PluginIdentity> RegisterPlugins(PluginContextFactory pluginContextFactory, IEnumerable<PluginLoadContext> pluginLoadContexts)
        {
            ContainerBuilder.RegisterType<GarryPlugin>()
                .Keyed<Plugin>(GarryPlugin.PluginIdentity)
                .WithParameter(TypedParameter.From(pluginContextFactory.Create(GarryPlugin.PluginIdentity)))
                .SingleInstance();

            StartupOrders[GarryPlugin.PluginIdentity] = int.MinValue;

            foreach (PluginLoadContext pluginLoadContext in pluginLoadContexts)
            {
                PluginAssembly? pluginAssembly = pluginLoadContext.Load();
                if (pluginAssembly == null)
                {
                    yield break;
                }

                PluginIdentity pluginIdentity = pluginAssembly.PluginIdentity;
                ContainerBuilder.RegisterType(pluginAssembly.PluginType)
                    .Keyed<Plugin>(pluginIdentity)
                    .WithParameter(TypedParameter.From(pluginContextFactory.Create(pluginIdentity)))
                    .SingleInstance();

                ContainerBuilder.RegisterAssemblyModules(pluginLoadContext.Assemblies.ToArray());

                StartupOrders[pluginIdentity] = pluginAssembly.StartupOrder;

                yield return pluginIdentity;
            }
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
