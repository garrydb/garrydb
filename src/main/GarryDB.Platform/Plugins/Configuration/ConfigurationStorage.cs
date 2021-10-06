using System;
using System.Reflection;

using GarryDb.Platform.Persistence;
using GarryDb.Platform.Plugins.Extensions;
using GarryDb.Plugins;

using SQLite;

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GarryDb.Platform.Plugins.Configuration
{
    /// <summary>
    ///     Handles the loading and saving of the configuration for plugins.
    /// </summary>
    public sealed class ConfigurationStorage
    {
        private static readonly MethodInfo GetConfigurationMethod =
            typeof(ConfigurationStorage)
                .GetMethod(nameof(GetConfiguration),
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new[]
                    {
                        typeof(string)
                    },
                    null)!;

        private readonly ConnectionFactory connectionFactory;

        /// <summary>
        ///     Initializes a new <see cref="ConfigurationStorage" />.
        /// </summary>
        /// <param name="connectionFactory">The connection factory for creating database connections.</param>
        public ConfigurationStorage(ConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        /// <summary>
        ///     Find the configuration for the plugin.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="plugin">The plugin.</param>
        /// <returns>
        ///     The configuration, or <c>null</c> if the plugin is not a <see cref="ConfigurablePlugin{TConfiguration}" />.
        /// </returns>
        public object? FindConfiguration(PluginIdentity pluginIdentity, Plugin plugin)
        {
            Type? configurationType = plugin.FindConfigurationType();

            return configurationType == null
                ? null
                : GetConfigurationMethod.MakeGenericMethod(configurationType).Invoke(this, new object[] { pluginIdentity.Name });
        }

        private TConfiguration GetConfiguration<TConfiguration>(string name) where TConfiguration : new()
        {
            SQLiteConnection connection = connectionFactory.Open("garry");
            try
            {
                connection.CreateTable<ConfigurationTable>();

                ConfigurationTable record = connection.Find<ConfigurationTable>(t => t.Plugin == name);
                return record != null
                    ? JsonSerializer.Deserialize<TConfiguration>(record.Configuration) ?? new TConfiguration()
                    : new TConfiguration();
            }
            finally
            {
                connectionFactory.Close(connection);
            }
        }
    }
}
