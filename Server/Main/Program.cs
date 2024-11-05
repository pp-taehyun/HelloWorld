using MySql.Data.MySqlClient;
using System.Text;
using System.Text.Json;
using Dapper;

namespace Server.Main
{
    public class Program
    {
        public static MySqlConnection? Connection { get; private set; }

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
            string config = string.Join("\r\n", File.ReadAllLines(configurationFilePath, Encoding.UTF8));
            JsonElement root = JsonDocument.Parse(config).RootElement;

            string database = root.GetProperty("database").ToString();
            string username = root.GetProperty("user").ToString();
            string password = root.GetProperty("password").ToString();

            // config.json�� ���� MySQL ����
            Connection = new MySqlConnection($"Server=localhost;" +
                                             $"Database={database};" +
                                             $"Uid={username};" +
                                             $"Pwd={password};");

            // ���̺� ������ ����
            if (createDummyData)
            {
                Random random = new Random();
                for (int i = 0; i < 10_000; ++i)
                {
                    Database.World world = new Database.World()
                    {
                        Id = i + 1,
                        RandomNumber = random.Next(0, 10_000) + 1
                    };
                    Connection!.Execute("INSERT INTO World VALUES(@Id, @RandomNumber)", world);
                }
            }

            // liquibase.properties ������ ���� ��� ���� ����
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