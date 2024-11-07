﻿using MySql.Data.MySqlClient;
using StackExchange.Redis;

namespace Server.Database
{
    public static class DatabaseManager
    {
        public static MySqlConnectionStringBuilder ConnectionOption { get; set; } = new();

        public static MySqlConnection GetConnection()
        {
            var conn = new MySqlConnection(ConnectionOption.ConnectionString);
            return conn;
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
