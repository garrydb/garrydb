using System;
using System.Collections.Generic;
using System.Linq;

using Akka.Actor;

using Autofac;

using GarryDB.Plugins;

using JetBrains.Annotations;

namespace GarryDB.Platform.Plugins
{
    internal sealed class PluginRegistry : IDisposable
    {
        private readonly IActorRef pluginsActor;
        private readonly ContainerBuilder containerBuilder;
        private readonly Lazy<IContainer> container;

        public PluginRegistry(IActorRef pluginsActor)
        {
            this.pluginsActor = pluginsActor;
            containerBuilder = new ContainerBuilder();
            container = new Lazy<IContainer>(() => containerBuilder.Build());

            containerBuilder.RegisterType<GarryPlugin>()
                            .As<Plugin>()
                            .WithParameter(TypedParameter.From<PluginContext>(new AkkaPluginContext(pluginsActor, GarryPlugin.PluginIdentity)))
                            .WithMetadata<RegisterdPluginMetadata>(c => c.For(m => m.StartupOrder, int.MinValue)
                                                                         .For(m => m.PluginIdentity, GarryPlugin.PluginIdentity))
                            .SingleInstance();
        }

        public void Load(PluginLoadContext pluginLoadContext)
        {
            PluginAssembly? pluginAssembly = pluginLoadContext.Load();
            if (pluginAssembly == null)
            {
                return;
            }

            containerBuilder.RegisterAssemblyModules(pluginLoadContext.Assemblies.ToArray());
            containerBuilder.RegisterType(pluginAssembly.PluginType)
                            .As<Plugin>()
                            .WithParameter(TypedParameter.From<PluginContext>(new AkkaPluginContext(pluginsActor, pluginAssembly.PluginIdentity)))
                            .WithMetadata<RegisterdPluginMetadata>(c => c.For(m => m.StartupOrder, pluginAssembly.StartupOrder)
                                                                         .For(m => m.PluginIdentity, pluginAssembly.PluginIdentity)
                                                                       )
                            .SingleInstance();
        }

        public IDictionary<PluginIdentity, Plugin> Plugins
        {
            get
            {
                return container.Value.Resolve<IEnumerable<Lazy<Plugin, RegisterdPluginMetadata>>>()
                                .OrderBy(x => x.Metadata.StartupOrder)
                                .ToDictionary(x => x.Metadata.PluginIdentity, x => x.Value);
            }
        }

        public void Dispose()
        {
            if (container.IsValueCreated)
            {
                container.Value.Dispose();
            }
        }

        [UsedImplicitly]
        private sealed class RegisterdPluginMetadata
        {
            public int StartupOrder { get; set; }
            public PluginIdentity PluginIdentity { get; set; } = null!;
        }
    }
}
