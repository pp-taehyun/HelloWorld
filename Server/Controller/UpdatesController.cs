using Dapper;
using Microsoft.AspNetCore.Mvc;
using Server.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;

namespace Server.Controller
{
    [Route("[controller]")]
    [ApiController]
    public class UpdatesController : ControllerBase
    {
        private static Random Random { get; } = new();

        [HttpGet]
        public IActionResult UpdateData(int queries)
        {
            queries = Math.Clamp(queries, 1, 500);

            var worldList = new List<World>();

            var sql = "UPDATE World SET RandomNumber = @RandomNumber WHERE Id = @Id";

            for (var i = 0; i < queries; i++)
            {
                int selectId = Random.Next(0, 10_000) + 1;
                int newNumber = Random.Next(0, 10_000) + 1;

                var world = new World
                {
                    id = selectId,
                    randomNumber = newNumber
                };
                worldList.Add(world);

                DatabaseManager.Connection.Query<World>(sql, new
                {
                    Id = selectId,
                    RandomNumber = newNumber
                });
            }

            string json = JsonSerializer.Serialize(worldList);

            Response.ContentType = "application/json";
            Response.ContentLength = json.Length;

            return Content(json);
        }
    }
}
