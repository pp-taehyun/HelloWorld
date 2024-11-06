using Dapper;
using Microsoft.AspNetCore.Mvc;
using Server.Database;
using System.Text.Json;

namespace Server.Controller
{
    [Route("[controller]")]
    [ApiController]
    public class DbController : ControllerBase
    {
        private static Random Random { get; } = new Random();

        [HttpGet]
        public IActionResult ExecuteSingleQuery()
        {
            World selectedWorld = DatabaseManager.Connection.Query<World>("SELECT * FROM World WHERE Id = @Id",
                                                                        new
                                                                        {
                                                                            Id = Random.Next(0, 10_000) + 1
                                                                        }).First();

            string json = JsonSerializer.Serialize(selectedWorld);

            Response.ContentType = "application/json";
            Response.ContentLength = json.Length;

            return Content(json);
        }
    }
}
