using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Autofac;

using GarryDB.Platform.Actors;
using GarryDB.Plugins;

using JetBrains.Annotations;

namespace GarryDB.Platform.Plugins
{
    /// <summary>
    ///     A registry containing all loaded <see cref="Plugin" />s.
    /// </summary>
    internal sealed class PluginRegistry : IEnumerable<PluginIdentity>, IDisposable
    {
        private readonly PluginContextFactory pluginContextFactory;
        private readonly IList<RegisteredPluginMetadata> registeredPlugins;

        private readonly ContainerBuilder containerBuilder;
        private readonly Lazy<IContainer> container;

        /// <summary>
        ///     Intializes a new <see cref="PluginRegistry" />.
        /// </summary>
        /// <param name="pluginContextFactory">The factory for creating a <see cref="PluginContext" />.</param>
        public PluginRegistry(PluginContextFactory pluginContextFactory)
        {
            this.pluginContextFactory = pluginContextFactory;
            registeredPlugins = new List<RegisteredPluginMetadata>();

            containerBuilder = new ContainerBuilder();
            container = new Lazy<IContainer>(() => containerBuilder.Build());

            containerBuilder.RegisterType<GarryPlugin>()
                .Keyed<Plugin>(GarryPlugin.PluginIdentity)
                .WithParameter(TypedParameter.From(pluginContextFactory.Create(GarryPlugin.PluginIdentity)))
                .SingleInstance();

            registeredPlugins.Add(new RegisteredPluginMetadata(int.MinValue, GarryPlugin.PluginIdentity));
        }

        /// <summary>
        ///     Load the plugin from the <paramref name="pluginLoadContext" />.
        /// </summary>
        /// <param name="pluginLoadContext">The plugin load context.</param>
        public void Load(PluginLoadContext pluginLoadContext)
        {
            PluginAssembly? pluginAssembly = pluginLoadContext.Load();
            if (pluginAssembly == null)
            {
                return;
            }

            containerBuilder.RegisterAssemblyModules(pluginLoadContext.Assemblies.ToArray());
            containerBuilder.RegisterType(pluginAssembly.PluginType)
                .Keyed<Plugin>(pluginAssembly.PluginIdentity)
                .WithParameter(TypedParameter.From(pluginContextFactory.Create(pluginAssembly.PluginIdentity)))
                .SingleInstance();

            registeredPlugins.Add(new RegisteredPluginMetadata(pluginAssembly.StartupOrder, pluginAssembly.PluginIdentity));
        }

        /// <summary>
        ///     Gets the <see cref="Plugin" /> based on the identity.
        /// </summary>
        /// <param name="pluginIdentity">The identity of the plugin.</param>
        /// <returns>The plugin, or <c>null</c> if the plugin can't be found.</returns>
        public Plugin? this[PluginIdentity pluginIdentity]
        {
            get { return container.Value.ResolveOptionalKeyed<Plugin>(pluginIdentity); }
        }

        public IEnumerator<PluginIdentity> GetEnumerator()
        {
            return registeredPlugins.OrderBy(x => x.StartupOrder).Select(x => x.PluginIdentity).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (container.IsValueCreated)
            {
                container.Value.Dispose();
            }
        }

        [UsedImplicitly]
        private sealed class RegisteredPluginMetadata
        {
            public RegisteredPluginMetadata(int startupOrder, PluginIdentity pluginIdentity)
            {
                StartupOrder = startupOrder;
                PluginIdentity = pluginIdentity;
            }

            public int StartupOrder { get; }
            public PluginIdentity PluginIdentity { get; }
        }
    }
}
