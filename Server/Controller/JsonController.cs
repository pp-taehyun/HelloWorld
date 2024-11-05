using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Server.Controller
{
    [Route("[controller]")]
    [ApiController]
    public class JsonController : ControllerBase
    {
        class ResponseData
        {
            public required string message { get; set; }
        }

        [HttpGet]
        public string SerializeJson()
        {
            var data = new ResponseData { message = "Hello, World!" };
            var json = JsonSerializer.Serialize(data);

            Response.ContentType = "application/json";
            Response.ContentLength = json.Length;

            return json;
        }
    }
}
