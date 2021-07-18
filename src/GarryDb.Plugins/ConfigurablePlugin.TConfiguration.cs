using System.Threading.Tasks;

namespace GarryDb.Plugins
{
    /// <summary>
    ///     A plugin that can be configured
    /// </summary>
    public abstract class ConfigurablePlugin<TConfiguration> : Plugin
    {
        /// <summary>
        ///     Initializes a new <see cref="ConfigurablePlugin" />.
        /// </summary>
        protected ConfigurablePlugin()
        {
            Register("configure", (TConfiguration configuration) => ConfigureAsync(configuration));
        }

        /// <summary>
        ///     Called when the plugin is being configured.
        /// </summary>
        protected virtual void Configure(TConfiguration configuration)
        {
        }

        /// <summary>
        ///     Called when the plugin is being configured.
        /// </summary>
        protected virtual Task ConfigureAsync(TConfiguration configuration)
        {
            Configure(configuration);
            return Task.CompletedTask;
        }
    }
}
