using Dapper;
using Microsoft.AspNetCore.Mvc;
using Server.Main;
using System.Text.Json;

namespace Server.Controller
{
    [Route("[controller]")]
    [ApiController]
    public class DbController : ControllerBase
    {
        [HttpGet]
        public IActionResult ExecuteSingleQuery()
        {
            Random random = new Random();
            Database.World selectedWorld = Program.Connection!.Query<Database.World>("SELECT * FROM World WHERE Id = @Id",
                                                            new Database.World()
                                                            {
                                                                Id = random.Next(0, 10_000) + 1
                                                            }).First();

            string json = JsonSerializer.Serialize(selectedWorld);

            Response.ContentType = "application/json";
            Response.ContentLength = json.Length;

            return Content(json);
        }
    }
}
