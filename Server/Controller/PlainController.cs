using Microsoft.AspNetCore.Mvc;
using Server;

namespace JsonSerialization.Controller
{
    [Route("[controller]")]
    [ApiController]
    public class PlainController : ControllerBase
    {
        [HttpGet]
        public string Plaintext()
        {
            var HelloWorld = "Hello, World!";
            return HelloWorld;
        }
    }
}
