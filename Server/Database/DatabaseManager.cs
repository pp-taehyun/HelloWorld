using MySql.Data.MySqlClient;
using StackExchange.Redis;

namespace Server.Database
{
    public static class DatabaseManager
    {
        public static MySqlConnection Connection { get; set; } = new();
        public static IDatabase? RedisDatabase { get; private set; }

        public static void Initialize()
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            RedisDatabase = redis.GetDatabase();
        }
    }
}
