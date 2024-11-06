using MySql.Data.MySqlClient;
using StackExchange.Redis;

namespace Server.Database
{
    public static class DatabaseManager
    {
        public static string ConnectionString { get; set; } = "";

        public static MySqlConnection Connection
        {
            get
            {
                return new MySqlConnection(ConnectionString);
            }
        }

        public static IDatabase RedisDatabase
        {
            get
            {
                return redisConnection.GetDatabase();
            }
        }

        private static ConnectionMultiplexer redisConnection => ConnectionMultiplexer.Connect("localhost");
    }
}
