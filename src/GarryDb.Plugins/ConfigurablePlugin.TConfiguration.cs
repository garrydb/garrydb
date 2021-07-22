using System.Threading.Tasks;

namespace GarryDb.Plugins
{
    /// <summary>
    ///     A plugin that can be configured
    /// </summary>
    public abstract class ConfigurablePlugin<TConfiguration> : Plugin where TConfiguration : new()
    {
        /// <summary>
        ///     Initializes a new <see cref="ConfigurablePlugin" />.
        /// </summary>
        /// <param name="pluginContext">The plugin context.</param>
        protected ConfigurablePlugin(PluginContext pluginContext)
            : base(pluginContext)
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
