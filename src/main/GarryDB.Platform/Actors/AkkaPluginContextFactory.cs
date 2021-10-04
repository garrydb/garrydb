using Akka.Actor;

using GarryDB.Platform.Plugins;
using GarryDB.Plugins;

namespace GarryDB.Platform.Actors
{
    /// <summary>
    ///     Creates a <see cref="PluginContext" /> to use with Akka.
    /// </summary>
    internal sealed class AkkaPluginContextFactory : PluginContextFactory
    {
        private readonly IActorRef pluginsActor;

        /// <summary>
        ///     Initializes a new <see cref="AkkaPluginContextFactory" />.
        /// </summary>
        /// <param name="pluginsActor">The actor reference to the plugins actor.</param>
        public AkkaPluginContextFactory(IActorRef pluginsActor)
        {
            this.pluginsActor = pluginsActor;
        }

        public PluginContext Create(PluginIdentity pluginIdentity)
        {
            return new AkkaPluginContext(pluginsActor, pluginIdentity);
        }
    }
}
