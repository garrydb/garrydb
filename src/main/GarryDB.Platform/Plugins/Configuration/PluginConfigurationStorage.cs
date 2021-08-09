using System;
using System.Reflection;

using GarryDB.Platform.Databases;
using GarryDB.Platform.Plugins.Extensions;
using GarryDB.Plugins;

using SQLite;

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GarryDB.Platform.Plugins.Configuration
{
    internal sealed class PluginConfigurationStorage
    {
        private static readonly MethodInfo GetConfigurationMethod;

        private readonly ConnectionFactory connectionFactory;
        private readonly PluginRegistry pluginRegistry;

        static PluginConfigurationStorage()
        {
            GetConfigurationMethod =
                typeof(PluginConfigurationStorage)
                    .GetMethod(nameof(GetConfiguration),
                               BindingFlags.Instance | BindingFlags.NonPublic,
                               null,
                               new[]
                               {
                                   typeof(PluginIdentity)
                               },
                               null)!;
        }

        public PluginConfigurationStorage(ConnectionFactory connectionFactory, PluginRegistry pluginRegistry)
        {
            this.connectionFactory = connectionFactory;
            this.pluginRegistry = pluginRegistry;
        }

        public object? FindConfiguration(PluginIdentity pluginIdentity)
        {
            Plugin plugin = pluginRegistry.Plugins[pluginIdentity];
            Type? configurationType = plugin.FindConfigurationType();

            if (configurationType == null)
            {
                return null;
            }

            return GetConfigurationMethod.MakeGenericMethod(configurationType).Invoke(this, new object[] { pluginIdentity });
        }

        private TConfiguration GetConfiguration<TConfiguration>(PluginIdentity pluginIdentity) where TConfiguration : new()
        {
            SQLiteConnection connection = connectionFactory.Open();
            try
            {
                connection.CreateTable<ConfigurationTable>();

                ConfigurationTable? record = connection.Find<ConfigurationTable>(t => t.Plugin == pluginIdentity.Name);
                if (record == null)
                {
                    var configuration = new TConfiguration();
                    connection.Insert(new ConfigurationTable
                                      {
                                          Plugin = pluginIdentity.Name, Configuration = JsonSerializer.Serialize(configuration)
                                      });

                    return new TConfiguration();
                }

                return JsonSerializer.Deserialize<TConfiguration>(record.Configuration) ?? new TConfiguration();
            }
            finally
            {
                connectionFactory.Close(connection);
            }
        }
    }
}
