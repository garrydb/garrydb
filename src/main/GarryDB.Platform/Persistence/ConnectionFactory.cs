using SQLite;

namespace GarryDB.Platform.Persistence
{
    /// <summary>
    ///     Opens and closes connections to a SQLite database.
    /// </summary>
    public interface ConnectionFactory
    {
        /// <summary>
        ///     Open a connection to the database named <paramref name="databaseName" />.
        /// </summary>
        /// <param name="databaseName">The name of the database.</param>
        /// <returns>A connectino to the database.</returns>
        SQLiteConnection Open(string databaseName);

        /// <summary>
        ///     Close the <paramref name="connection" />.
        /// </summary>
        /// <param name="connection">The connection to close.</param>
        void Close(SQLiteConnection connection);
    }
}
