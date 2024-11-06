using MySql.Data.MySqlClient;
using System.Text;
using System.Text.Json;
using Dapper;
using Server.Database;

namespace Server.Main
{
    public class Program
    {
        private static void Main(string[] args)
        {
            SetupDatabase("config.json", false);

            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
        }

        private static void SetupDatabase(string configurationFilePath, bool createDummyData)
        {
            DatabaseManager.Initialize();

            string config = string.Join("\r\n", File.ReadAllLines(configurationFilePath, Encoding.UTF8));
            JsonElement root = JsonDocument.Parse(config).RootElement;

            string database = root.GetProperty("database").ToString();
            string username = root.GetProperty("user").ToString();
            string password = root.GetProperty("password").ToString();

            // config.json을 통한 MySQL 접속
            DatabaseManager.Connection = new MySqlConnection($"Server=localhost;" +
                                             $"Database={database};" +
                                             $"Uid={username};" +
                                             $"Pwd={password};");

            // 테이블 데이터 생성
            if (createDummyData)
            {
                DatabaseManager.Connection.Execute("TRUNCATE TABLE World");
                DatabaseManager.Connection.Execute("TRUNCATE TABLE CachedWorld");

                Random random = new Random();
                World world = new World();
                CachedWorld cachedWorld = new CachedWorld();
                for (int i = 0; i < 10_000; ++i)
                {
                    world.id = cachedWorld.id = i + 1;
                    world.randomNumber = cachedWorld.randomNumber = random.Next(0, 10_000) + 1;
         
                    DatabaseManager.Connection.Execute("INSERT INTO World VALUES(@Id, @RandomNumber)", world);
                    DatabaseManager.Connection.Execute("INSERT INTO CachedWorld VALUES(@Id, @RandomNumber)", cachedWorld);
                }
            }

            // liquibase.properties 파일이 없을 경우 새로 생성
            if (!File.Exists("liquibase.properties"))
            {
                string[] propertiesTemplate = File.ReadAllLines("Templates/liquibasePropertiesTemplate.txt",
                                                                Encoding.UTF8);

                using (StreamWriter writer = File.CreateText("liquibase.properties"))
                {
                    foreach (string line in propertiesTemplate)
                    {
                        string liquibaseProperties = string.Format(line, database, username, password);
                        writer.WriteLine(liquibaseProperties);
                    }
                }
            }
        }
    }
}