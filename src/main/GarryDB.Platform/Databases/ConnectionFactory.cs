using SQLite;

namespace GarryDB.Platform.Databases
{
    internal interface ConnectionFactory
    {
        SQLiteConnection Open(string databaseName);
        void Close(SQLiteConnection connection);
    }
}
