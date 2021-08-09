using SQLite;

namespace GarryDB.Platform.Databases
{
    internal interface ConnectionFactory
    {
        SQLiteConnection Open();
        void Close(SQLiteConnection connection);
    }
}
