using System.Linq;

using Akka.Actor;

using GarryDB.Platform.Bootstrapping;
using GarryDB.Platform.Bootstrapping.Extensions;
using GarryDB.Platform.Infrastructure;
using GarryDB.Platform.Persistence;
using GarryDB.Platform.Plugins;
using GarryDB.Specs.Platform.Actors.Builders;
using GarryDB.Specs.Platform.Infrastructure.Builders;
using GarryDB.Specs.Platform.Persistence.Builders;
using GarryDB.Specs.Platform.Plugins.Builders;

namespace GarryDB.Specs.Platform.Bootstrapping
{
    internal sealed class BootstrapperBuilder : TestDataBuilder<Bootstrapper>
    {
        private Modifier.FindPlugins finder = _ => Enumerable.Empty<PluginPackage>();
        private Modifier.PreparePlugins preparer = _ => Enumerable.Empty<PluginLoadContext>();
        private Modifier.RegisterPlugins registrar = _ => Enumerable.Empty<PluginIdentity>();
        private Modifier.LoadPlugin loader = _ => null;
        private Modifier.ConfigurePlugin configurer = (_, _) => { };
        private Modifier.StartPlugins starter = _ => { };
        private Modifier.StopPlugins stopper = _ => { };
        private PluginContextFactory pluginContextFactory;
        private FileSystem fileSystem;
        private ConnectionFactory connectionFactory;

        protected override void OnPreBuild()
        {
            if (pluginContextFactory == null)
            {
                Using(new PluginContextFactoryBuilder().Build());
            }

            if (fileSystem == null)
            {
                Using(new FileSystemBuilder().Build());
            }

            if (connectionFactory == null)
            {
                Using(new ConnectionFactoryBuilder().Build());
            }
        }

        protected override Bootstrapper OnBuild()
        {
            return
                new Bootstrapper()
                    .Finder(_ => finder)
                    .Preparer(_ => preparer)
                    .Registrar(_ => registrar)
                    .Loader(_ => loader)
                    .Configurer(_ => configurer)
                    .Starter(_ => starter)
                    .Stopper(_ => stopper)
                    .Use(pluginContextFactory)
                    .Use(fileSystem)
                    .Use(connectionFactory);
        }

        public Bootstrapper UseDefault()
        {
            return new Bootstrapper().ApplyDefault();
        }

        public Bootstrapper UseAkka(IActorRef pluginsActor = null)
        {
            return ApplyAkkaBootstrapperModifier.Modify(new Bootstrapper(), new TestKitBuilder().Build().Sys, pluginsActor ?? new PluginsActorBuilder().Build());
        }

        public BootstrapperBuilder Finds(Modifier.FindPlugins finder)
        {
            this.finder = finder;
            return this;
        }

        public BootstrapperBuilder Prepares(Modifier.PreparePlugins preparer)
        {
            this.preparer = preparer;
            return this;
        }

        public BootstrapperBuilder Registers(Modifier.RegisterPlugins registrar)
        {
            this.registrar = registrar;
            return this;
        }

        public BootstrapperBuilder Loads(Modifier.LoadPlugin loader)
        {
            this.loader = loader;
            return this;
        }

        public BootstrapperBuilder Configures(Modifier.ConfigurePlugin configurer)
        {
            this.configurer = configurer;
            return this;
        }

        public BootstrapperBuilder Starts(Modifier.StartPlugins starter)
        {
            this.starter = starter;
            return this;
        }

        public BootstrapperBuilder Stops(Modifier.StopPlugins stopper)
        {
            this.stopper = stopper;
            return this;
        }

        public BootstrapperBuilder Using(PluginContextFactory pluginContextFactory)
        {
            this.pluginContextFactory = pluginContextFactory;
            return this;
        }

        public BootstrapperBuilder Using(FileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
            return this;
        }

        public BootstrapperBuilder Using(ConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
            return this;
        }
    }
}
