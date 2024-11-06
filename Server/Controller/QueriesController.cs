using Dapper;
using Microsoft.AspNetCore.Mvc;
using Server.Database;
using Server.Main;
using System.Text;
using System.Text.Json;

namespace Server.Controller
{
    [Route("[controller]")]
    [ApiController]
    public class QueriesController : ControllerBase
    {
        private static Random Random { get; } = new Random();

        [HttpGet]
        public async Task<IActionResult> ExecuteMultipleQueries(int queries)
        {
            List<World> worldList = new List<World>();
            StringBuilder builder = new StringBuilder();

            var selectQuery = "SELECT * FROM World WHERE Id = {0};";

            for (int i = 0; i < Math.Clamp(queries, 1, 500); i++)
            {
                builder.Append(string.Format(selectQuery, Random.Next(0, 10_000) + 1));
            }

            using SqlMapper.GridReader multi = await Program.Connection!.QueryMultipleAsync(builder.ToString());
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
