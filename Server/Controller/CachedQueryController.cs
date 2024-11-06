using Dapper;
using Microsoft.AspNetCore.Mvc;
using Server.Database;
using System.Text.Json;
using System.Text;
using System.Linq;
using StackExchange.Redis;

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

            var worldList = new List<World>();
            var builder = new StringBuilder();
            IDatabase redisDatabase = DatabaseManager.RedisDatabase;

            var selectQuery = "SELECT * FROM World WHERE Id = {0};";

            for (var i = 0; i < count; i++)
            {
                string? cachedValue = redisDatabase.StringGet((i + 1).ToString());
                if (cachedValue is not null)
                {
                    worldList.Add(new World
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

            using SqlMapper.GridReader multi = await DatabaseManager.Connection.QueryMultipleAsync(builder.ToString());
            while (!multi.IsConsumed)
            {
                World world = multi.Read<World>().Single();
                redisDatabase.StringSet(world.id.ToString(), world.randomNumber);

                worldList.Add(world);
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
