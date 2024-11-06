using MySql.Data.MySqlClient;
using StackExchange.Redis;

namespace Server.Database
{
    public static class DatabaseManager
    {
        public static MySqlConnection Connection { get; set; } = new();

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
