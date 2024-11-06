using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Server.Database;
using System.Text;
using System.Text.Json;

namespace Server.Controller
{
    [Route("[controller]")]
    [ApiController]
    public class QueriesController : ControllerBase
    {
        private static Random Random { get; } = new();

        [HttpGet]
        public async Task<IActionResult> ExecuteMultipleQueries(int queries)
        {
            var worldList = new List<World>();
            var builder = new StringBuilder();

            var selectQuery = "SELECT * FROM World WHERE Id = {0};";

            for (var i = 0; i < Math.Clamp(queries, 1, 500); i++)
            {
                builder.Append(string.Format(selectQuery, Random.Next(0, 10_000) + 1));
            }

            using (MySqlConnection conn = DatabaseManager.GetConnection())
            {
                using SqlMapper.GridReader multi = await conn.QueryMultipleAsync(builder.ToString());
                while (!multi.IsConsumed)
                {
                    worldList.Add(multi.Read<World>().Single());
                }
            }

            string json = JsonSerializer.Serialize(worldList);

            Response.ContentType = "application/json";
            Response.ContentLength = json.Length;

            return Content(json);
        }
    }
}
