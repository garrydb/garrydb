using System.Diagnostics.CodeAnalysis;
using System.IO;

using GarryDB.Platform.Infrastructure;

using SQLite;

namespace GarryDB.Platform.Persistence
{
    /// <summary>
    ///     Creates <see cref="SQLiteConnection" />s where the database is stored on disk.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class PersistentSqLiteConnectionFactory : ConnectionFactory
    {
        private readonly FileSystem fileSystem;
        private readonly string databasePath;

        /// <summary>
        ///     Initializes a new <see cref="PersistentSqLiteConnectionFactory" />.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="databasePath">The path where the databases should be stored.</param>
        public PersistentSqLiteConnectionFactory(FileSystem fileSystem, string databasePath)
        {
            this.fileSystem = fileSystem;
            this.databasePath = databasePath;
        }

        /// <inheritdoc />
        public SQLiteConnection Open(string databaseName)
        {
            if (!fileSystem.Exists(databasePath))
            {
                fileSystem.CreateDirectory(databasePath);
            }

            return new SQLiteConnection(Path.Combine(databasePath, $"{databaseName}.db"));
        }

        /// <inheritdoc />
        public void Close(SQLiteConnection connection)
        {
            connection.Close();
        }
    }
}
