using Dapper;
using Microsoft.AspNetCore.Mvc;
using Server.Database;
using System.Text.Json;
using System.Text;

namespace Server.Controller
{
    [Route("cached-queries")]
    [ApiController]
    public class CachedQueryController : ControllerBase
    {
        private static Random Random { get; } = new Random();

        [HttpGet]
        public async Task<IActionResult> ExecuteCachedMultipleQueries(int count)
        {
            List<World> worldList = new();
            StringBuilder builder = new();

            var selectQuery = "SELECT * FROM World WHERE Id = {0};";

            for (int i = 0; i < Math.Clamp(count, 1, 500); i++)
            {
                builder.Append(string.Format(selectQuery, Random.Next(0, 10_000) + 1));
            }

            using SqlMapper.GridReader multi = await DatabaseManager.Connection.QueryMultipleAsync(builder.ToString());
            while (!multi.IsConsumed)
            {
                worldList.Add(multi.Read<World>().Single());
            }

            string json = JsonSerializer.Serialize(worldList);

            Response.ContentType = "application/json";
            Response.ContentLength = json.Length;

            return Content(json);
        }
    }
}
