using GarryDB.Platform.Persistence;

using SQLite;

namespace GarryDB.Specs.Platform.Persistence.Builders
{
    public sealed class ConnectionFactoryBuilder : TestDataBuilder<ConnectionFactory>
    {
        protected override ConnectionFactory OnBuild()
        {
            return new ConnectionFactoryStub();
        }

        private sealed class ConnectionFactoryStub : ConnectionFactory
        {
            private int connectionCount;
            private SQLiteConnection connection;

            public SQLiteConnection Open(string databaseName)
            {
                if (connectionCount == 0)
                {
                    connection = new SQLiteConnection(":memory:");
                }

                connectionCount++;

                return connection;
            }

            public void Close(SQLiteConnection connection)
            {
                connectionCount--;

                if (connectionCount == 0)
                {
                    connection.Close();
                }
            }
        }
    }
}
