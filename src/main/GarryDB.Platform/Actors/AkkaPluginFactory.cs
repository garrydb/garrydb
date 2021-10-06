using System;

using Akka.Actor;

using GarryDb.Platform.Plugins;
using GarryDb.Plugins;

namespace GarryDb.Platform.Actors
{
    /// <summary>
    ///     Creates a <see cref="Plugin" /> to use with Akka.
    /// </summary>
    internal sealed class AkkaPluginFactory : PluginFactory
    {
        private readonly IActorRef pluginsActor;

        /// <summary>
        ///     Initializes a new <see cref="AkkaPluginFactory" />.
        /// </summary>
        /// <param name="pluginsActor">The actor reference to the plugins actor.</param>
        public AkkaPluginFactory(IActorRef pluginsActor)
        {
            this.pluginsActor = pluginsActor;
        }

        public Plugin Create(PluginAssembly pluginAssembly)
        {
            PluginIdentity pluginIdentity = pluginAssembly.PluginIdentity;
            var pluginContext = new AkkaPluginContext(pluginsActor, pluginIdentity);

            return (Plugin)Activator.CreateInstance(pluginAssembly.PluginType, pluginContext)!;
        }
    }
}
