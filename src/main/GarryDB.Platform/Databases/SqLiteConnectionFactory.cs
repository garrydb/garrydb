using System.IO;

using GarryDB.Platform.Infrastructure;

using SQLite;

namespace GarryDB.Platform.Databases
{
    internal sealed class SqLiteConnectionFactory : ConnectionFactory
    {
        private readonly FileSystem fileSystem;
        private readonly string databasePath;

        public SqLiteConnectionFactory(FileSystem fileSystem, string databasePath)
        {
            this.fileSystem = fileSystem;
            this.databasePath = databasePath;
        }

        public SQLiteConnection Open(string databaseName)
        {
            if (!fileSystem.Exists(databasePath))
            {
                fileSystem.CreateDirectory(databasePath);
            }
            
            return new SQLiteConnection(Path.Combine(databasePath, $"{databaseName}.db"));
        }

        public void Close(SQLiteConnection connection)
        {
            connection.Close();
        }
    }
}
