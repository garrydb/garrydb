using SQLite;

namespace GarryDB.Platform.Plugins.Configuration
{
    /// <summary>
    ///     The table with the configuration of plugins.
    /// </summary>
    [Table("configurations")]
    public class ConfigurationTable
    {
        /// <summary>
        ///     Gets and sets the id of the record.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        ///     Gets and sets the name of the plugin.
        /// </summary>
        [Column("plugin"), NotNull]
        public string Plugin { get; set; } = null!;

        /// <summary>
        ///     Gets and sets the configuration as JSON.
        /// </summary>
        [Column("configuration"), NotNull]
        public string Configuration { get; set; } = null!;
    }
}
