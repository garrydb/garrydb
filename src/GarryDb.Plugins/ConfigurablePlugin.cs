using System.Threading.Tasks;

namespace GarryDb.Plugins
{
    /// <summary>
    ///     A plugin that can be configured
    /// </summary>
    public abstract class ConfigurablePlugin : ConfigurablePlugin<object>
    {
        /// <inheritdoc />
        protected sealed override void Configure(object configuration)
        {
            Configure();
        }

        /// <summary>
        ///     Called when the plugin is being configured.
        /// </summary>
        protected virtual void Configure()
        {
        }

        /// <inheritdoc />
        protected sealed override Task ConfigureAsync(object configuration)
        {
            return ConfigureAsync();
        }

        /// <summary>
        ///     Called when the plugin is being configured.
        /// </summary>
        protected virtual Task ConfigureAsync()
        {
            Configure();
            return Task.CompletedTask;
        }
    }
}
