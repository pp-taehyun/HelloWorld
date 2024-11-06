using Dapper;
using Microsoft.AspNetCore.Mvc;
using Server.Database;
using System.Text.Json;
using System.Text;
using System.Linq;
using StackExchange.Redis;
using MySql.Data.MySqlClient;

namespace Server.Controller
{
    [Route("cached-queries")]
    [ApiController]
    public class CachedQueryController : ControllerBase
    {
        private static Random Random { get; } = new();

        [HttpGet]
        public async Task<IActionResult> ExecuteCachedMultipleQueries(int count)
        {
            count = Math.Clamp(count, 1, 500);

            var worldList = new List<CachedWorld>();
            var builder = new StringBuilder();
            IDatabase redisDatabase = DatabaseManager.RedisDatabase;

            var selectQuery = "SELECT * FROM World WHERE Id = {0};";

            for (var i = 0; i < count; i++)
            {
                string? cachedValue = redisDatabase.StringGet($"world:{i + 1}");
                if (cachedValue is not null)
                {
                    worldList.Add(new CachedWorld
                    {
                        id = i + 1,
                        randomNumber = Int32.Parse(cachedValue)
                    });
                }
                else
                {
                    builder.Append(string.Format(selectQuery, Random.Next(0, 10_000) + 1));
                }
            }

            using (MySqlConnection conn = DatabaseManager.GetConnection())
            {
                using SqlMapper.GridReader multi = await conn.QueryMultipleAsync(builder.ToString());
                while (!multi.IsConsumed)
                {
                    CachedWorld world = multi.Read<CachedWorld>().Single();

                    var cacheKey = $"world:{world.id}";
                    redisDatabase.StringSet(cacheKey, world.randomNumber);
                    redisDatabase.KeyExpire(cacheKey, TimeSpan.FromMinutes(3));

                    worldList.Add(world);
                }
            }

            if (worldList.Count != count)
                throw new Exception("Does not match requested count.");

            string json = JsonSerializer.Serialize(worldList);

            Response.ContentType = "application/json";
            Response.ContentLength = json.Length;

            return Content(json);
        }
    }
}
